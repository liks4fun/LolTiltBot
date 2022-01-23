using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

var host = CreateDefaultBuilder().Build();

await host.RunAsync();

static IHostBuilder CreateDefaultBuilder() {
    return Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((ctx, cfg) => {
            cfg.AddJsonFile("appsettings.json");
            cfg.Build();
        })
        .ConfigureServices((ctx, svc) => 
        {
            svc.AddSingleton<IConfiguration>(provider => ctx.Configuration);
            svc.AddTransient<DiscordBot.DiscordBotService>();
            svc.Configure<QuartzOptions>(ctx.Configuration.GetSection("Quartz"));
            svc.AddQuartz(q => {
                q.SchedulerId = "Scheduler-Core";
                q.UseMicrosoftDependencyInjectionJobFactory();
                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });
                q.ScheduleJob<CheckGameJob>(trigger => {
                    trigger.StartNow().WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever().Build());
                });
            });
            svc.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);
            svc.AddSingleton<MyMemoryCache>();
        });
}