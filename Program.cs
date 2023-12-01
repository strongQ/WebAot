
using FreeScheduler;
using System.Text.Json.Serialization;
using WebAOT;
using WebAOT.Entities;

var builder = WebApplication.CreateSlimBuilder(args);



builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

//ʹ�� FreeSql �־û����������б���ִ�У�FreeRedis ��Ҫ��
Enum.GetValues(typeof(TaskInterval));
Enum.GetValues(typeof(FreeScheduler.TaskStatus));
var fsql = new FreeSql.FreeSqlBuilder()
    .UseConnectionString(FreeSql.DataType.PostgreSQL, $"Host=xx; Port=xx; Database=xx; Username=xx; Password=xx")
    .UseAutoSyncStructure(true)
    .UseNoneCommandParameter(true)
    .UseMonitorCommand(cmd => Console.WriteLine(cmd.CommandText + "\r\n"))
    .Build();
Scheduler scheduler = new FreeSchedulerBuilder()
    .OnExecuting(task =>
    {
        Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fff")}] {task.Topic} ��ִ��");
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
    scheduler.AddTask("[ϵͳԤ��]������������", "86400", -1, 3600);
    scheduler.AddTaskRunOnWeek("����һ�����ִ��", "json", -1, "1:12:00:00");
    scheduler.AddTaskRunOnWeek("�����գ����ӻ", "json", -1, "0:00:00:00");
    scheduler.AddTaskRunOnWeek("���������罻�", "json", -1, "6:00:00:00");
    scheduler.AddTaskRunOnMonth("��β���һ��", "json", -1, "-1:16:00:00");
    scheduler.AddTaskRunOnMonth("�³���һ��", "json", -1, "1:00:00:00");
    scheduler.AddTask("��ʱ20��", "json", 10, 20);
    scheduler.AddTask("��������1", "json", new[] { 10, 30, 60, 100, 150, 200 });
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
sqlapi.MapGet("/", () =>
{
    var users = SqlStartup.Fsql.Select<SysUser>().ToList();
    return users.ToArray();
});

sqlapi.MapGet("/{id}", (int id) =>
{
   var dta= SqlStartup.Fsql.Ado.QuerySingle<SysUser>("select * from net_sysuser");
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


