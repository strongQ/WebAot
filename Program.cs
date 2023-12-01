
using FreeScheduler;
using System.Data;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebAOT;
using WebAOT.Entities;
using XT.Common.Config;
using XT.FeSql;
using XT.FeSql.Extensions;
using XT.FeSql.Models;
using XT.Task;
using XT.Task.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);



builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

XTDbContext.AopSqlEvent += (o, e) =>
{
    Console.WriteLine(e);
};
var config = new XT.FeSql.Models.XTDbConfig
{
    IsSqlAOP = true,
    Dbs = new List<XT.FeSql.Models.DataBaseOperate>
    {
        new XT.FeSql.Models.DataBaseOperate
        {
            DbType=FreeSql.DataType.PostgreSQL,
            ConnectionString="Host=xx; Port=xx; Database=xx; Username=xx; Password=xx",
            IsMain=true,
            Enabled=true
        }

    }
};
builder.Services.AddXTDbSetup(config);

builder.Services.AddXTTaskSetup(true,config);

//使用 FreeSql 持久化，以下两行必须执行（FreeRedis 无要求）
Enum.GetValues(typeof(TaskInterval));
Enum.GetValues(typeof(FreeScheduler.TaskStatus));




var app = builder.Build();

var sampleTodos = new Todo[] {
    new(1, "Walk the dog"),
    new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
    new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
    new(4, "Clean the bathroom"),
    new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
};

var todosApi = app.MapGroup("/todos");
todosApi.MapGet("/", () => sampleTodos);
todosApi.MapGet("/{id}", (int id) =>
    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? Results.Ok(todo)
        : Results.NotFound());

var sqlapi = app.MapGroup("/sqls");
sqlapi.MapGet("/", async () =>
{
   var dbcontext= app.Services.GetService<XTDbContext>();
    var db = dbcontext.GetDb();
   var rep= db.GetRepository<SysUser>();
   var users=await rep.Select.ToListAsync();
   
    return users.ToArray();
});

sqlapi.MapGet("/{id}", (int id) =>
{
    var dbcontext = app.Services.GetService<XTDbContext>();
    var db = dbcontext.GetSqlDb<TwoFlag>();
    var dta= db.Ado.QuerySingle<SysUser>("select * from net_sysuser");
   return Results.Ok(dta);
});


//var applicationLifeTime = app.Services.GetService<IHostApplicationLifetime>();
//applicationLifeTime.ApplicationStopping.Register(() =>
//{
//    scheduler.Dispose();
//    //redis.Dispose();
//    fsql.Dispose();
//});
var scheduler=app.Services.GetService<Scheduler>();
scheduler.AddDefaultTask();

app.UseFreeSchedulerUI("/task/");

TaskAction.ExecuteEvent += TaskAction_ExecuteEvent;

void TaskAction_ExecuteEvent(object? sender, TaskInfo e)
{
    Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fff")}] {e.Topic} 被执行");
    e.Remark("log");
}

app.Run("http://0.0.0.0:20071");

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
[JsonSerializable(typeof(SysUser[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}


