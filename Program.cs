
using FreeScheduler;
using System.Data;
using System.Text.Json.Serialization;
using WebAOT;
using WebAOT.Entities;
using XT.Common.Config;
using XT.FeSql;
using XT.FeSql.Extensions;
using XT.FeSql.Models;

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

//使用 FreeSql 持久化，以下两行必须执行（FreeRedis 无要求）
Enum.GetValues(typeof(TaskInterval));
Enum.GetValues(typeof(FreeScheduler.TaskStatus));
var fsql = new FreeSql.FreeSqlBuilder()
    .UseConnectionString(config.Dbs[0].DbType, config.Dbs[0].ConnectionString)
    .UseAutoSyncStructure(true)
    .UseNoneCommandParameter(true)
    .UseMonitorCommand(cmd => Console.WriteLine(cmd.CommandText + "\r\n"))
    .Build();
Scheduler scheduler = new FreeSchedulerBuilder()
    .OnExecuting(task =>
    {
        Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fff")}] {task.Topic} 被执行");
        task.Remark("log..");
    })
    .UseStorage(fsql)
    //.UseCluster(redis, new ClusterOptions
    //{
    //    Name = Environment.GetCommandLineArgs().FirstOrDefault(a => a.StartsWith("--name="))?.Substring(7),
    //    HeartbeatInterval = 2,
    //    OfflineSeconds = 5,
    //})
    .Build();
if (Datafeed.GetPage(scheduler, null, null, null, null).Total == 0)
{
    scheduler.AddTask("[系统预留]清理任务数据", "86400", -1, 3600);
    scheduler.AddTaskRunOnWeek("（周一）武林大会", "json", -1, "1:12:00:00");
    scheduler.AddTaskRunOnWeek("（周日）亲子活动", "json", -1, "0:00:00:00");
    scheduler.AddTaskRunOnWeek("（周六）社交活动", "json", -1, "6:00:00:00");
    scheduler.AddTaskRunOnMonth("月尾最后一天", "json", -1, "-1:16:00:00");
    scheduler.AddTaskRunOnMonth("月初第一天", "json", -1, "1:00:00:00");
    scheduler.AddTask("定时20秒", "json", 10, 20);
    scheduler.AddTask("测试任务1", "json", new[] { 10, 30, 60, 100, 150, 200 });
}
builder.Services.AddSingleton(fsql);
builder.Services.AddSingleton(scheduler);

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


var applicationLifeTime = app.Services.GetService<IHostApplicationLifetime>();
applicationLifeTime.ApplicationStopping.Register(() =>
{
    scheduler.Dispose();
    //redis.Dispose();
    fsql.Dispose();
});

app.UseFreeSchedulerUI("/task/");

app.Run("http://0.0.0.0:20071");

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
[JsonSerializable(typeof(SysUser[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}


