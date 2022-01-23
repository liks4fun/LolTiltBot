namespace RiotData {
    public static class RiotApiCalls {
        private const string BaseEuwUrl = "https://euw1.api.riotgames.com";
        private const string BaseEuropeUrl = "https://europe.api.riotgames.com";
        private const string SummonerInfoByName = BaseEuwUrl + "/lol/summoner/v4/summoners/by-name/";
        public static string GetSummonerInfoByName(string userName) => SummonerInfoByName + userName;
        private const string MatchInfoByPuuid = BaseEuropeUrl + "/lol/match/v5/matches/by-puuid/{puuid}/ids";
        public static string GetMatchInfoByPuuid(string puuid) => MatchInfoByPuuid.Replace("{puuid}", puuid);
        private const string MatchDetailsById = BaseEuropeUrl + "/lol/match/v5/matches/{matchId}";
        public static string GetMatchDetailsById(string matchId) => MatchDetailsById.Replace("{matchId}", matchId);
    }
}