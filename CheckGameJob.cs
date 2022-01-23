using Microsoft.Extensions.DependencyInjection;
using Quartz;

public class CheckGameJob : IJob {
    private IServiceProvider services;
    public CheckGameJob(IServiceProvider services) => this.services = services;
    public async Task Execute(IJobExecutionContext ctx) {
        System.Console.WriteLine($"Executed job at: {DateTime.Now.TimeOfDay}");
        using var scope = this.services.CreateScope();
        var botService = this.services.GetService<DiscordBot.DiscordBotService>();
        await botService.CheckDiscordUsers();
    }
}