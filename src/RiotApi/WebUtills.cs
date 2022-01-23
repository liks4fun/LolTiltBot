using System.Text.Json;

namespace RiotData {
    public class RiotWebCaller {
        private readonly string apiKey;
        public RiotWebCaller(string apiKey)
        {
            this.apiKey = apiKey;
        }
        public async Task<Stream> MakeRequest(string uri) {
            using (var client = new HttpClient()) {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Riot-Token", apiKey);
            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
            }
        }
    }
}