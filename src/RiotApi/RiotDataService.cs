namespace RiotData {
    using System.Text;
    using System.Text.Json;

    public static class RiotDataService {
    public static async Task<string> GetLastGameInfo(string username, string apiKey) {
        var riotWebCaller = new RiotWebCaller(apiKey);
            var jsonSerializationOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var summoner = await JsonSerializer.DeserializeAsync<SummonerDTO>(
            await riotWebCaller.MakeRequest(RiotApiCalls.GetSummonerInfoByName(username)),
            jsonSerializationOptions);
        var matches = await JsonSerializer.DeserializeAsync<List<string>>(
            await riotWebCaller.MakeRequest(RiotApiCalls.GetMatchInfoByPuuid(summoner.Puuid)),
            jsonSerializationOptions);
        var message = string.Empty;
        var lastMatchId = matches?.FirstOrDefault();
        if (!string.IsNullOrEmpty(lastMatchId)) {
            var matchDetails = await JsonSerializer.DeserializeAsync<MatchDto>(
                    await riotWebCaller.MakeRequest(RiotApiCalls.GetMatchDetailsById(lastMatchId)),
                    jsonSerializationOptions);
            var currSummonerStats = matchDetails.Info.Participants.FirstOrDefault(x => x.Puuid == summoner.Puuid);
            var builder = new StringBuilder();
            if (currSummonerStats is not null) {
                builder.AppendLine($"Наш дурачек - {summoner.Name} закончил игру на {currSummonerStats.ChampionName} как дебил, а конкретно:");
                builder.AppendLine($"- Сдох как собака: {currSummonerStats.Deaths} раз.");
                builder.AppendLine($"- Получил в ебасос: {currSummonerStats.DamageSelfMitigated} урона");
                message = builder.ToString();
            }
        }
        else message = $"Обоссаться, не нашли игр для {username}";
        return message;
        }
    }
}
