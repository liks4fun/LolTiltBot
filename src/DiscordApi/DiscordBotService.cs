namespace DiscordBot {
    using Discord;
    using Discord.WebSocket;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;

    public class DiscordBotService {
        private readonly MemoryCache cache;
        private readonly ulong LolLauncherAppId;
        private readonly ulong GuildId;
        private readonly ulong ChannelId;
        private readonly string BotKey;
        private readonly string RiotApiKey;
        private const string CACHED_USERS_KEY = "Cached_Users";
        public DiscordBotService(IConfiguration config, MyMemoryCache cache) {
            BotKey = config.GetValue<string>("DiscordConfig:BotKey");
            GuildId = config.GetValue<ulong>("DiscordConfig:GuildId");
            ChannelId = config.GetValue<ulong>("DiscordConfig:ChannelId");
            LolLauncherAppId = config.GetValue<ulong>("DiscordConfig:LolLauncherAppId");
            RiotApiKey = config.GetValue<string>("RiotConfig:DevApiKey");
            this.cache = cache.Cache;
        }

        public async Task CheckDiscordUsers() {
            var discordBot = new DiscordSocketClient(new DiscordSocketConfig { GatewayIntents = GatewayIntents.All });
            discordBot.Log += Log;
            await discordBot.LoginAsync(Discord.TokenType.Bot, BotKey);
            await discordBot.StartAsync();
            discordBot.Ready += async () => {
                var guild = discordBot.GetGuild(GuildId);
                var chanel = discordBot.GetChannel(ChannelId) as IMessageChannel;
                await guild.DownloadUsersAsync();
                var users = guild.Users.Where(x => x.Activities.Any(x => x.Name.Contains("League of legends", StringComparison.InvariantCultureIgnoreCase)));
                foreach (var user in users) {
                    if (!this.cache.TryGetValue<List<string>>(CACHED_USERS_KEY, out var usersWithOpenGameClient)) {
                        usersWithOpenGameClient = new List<string>();
                    }
                    if (usersWithOpenGameClient.Any(x => x.Equals(user.Username)) && user.Activities.Any(x => (x as RichGame).ApplicationId == LolLauncherAppId)) {
                        await chanel.SendMessageAsync(await RiotData.RiotDataService.GetLastGameInfo(user.Username, RiotApiKey));
                        usersWithOpenGameClient.Remove(user.Username);
                    }
                    else if (!usersWithOpenGameClient.Any(x => x.Equals(user.Username))) {
                        usersWithOpenGameClient.Add(user.Username);
                    }

                    this.cache.Set<List<string>>(CACHED_USERS_KEY, usersWithOpenGameClient);
                }
                await discordBot.StopAsync();
            };
        }
        private static Task Log(LogMessage arg)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }
    }
}