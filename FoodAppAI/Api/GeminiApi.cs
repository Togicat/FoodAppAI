using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FoodAppAI.Api
{
    public class GeminiApi
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _apiKey;

        public GeminiApi(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<string> RefineFoodJsonAsync(string rawJson)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new { text = $"You are a JSON formatter AI: take this raw JSON about a food and output a well structured, human-friendly JSON. Raw: {rawJson}" }
                        }
                    }
                }
            };

            string json = JsonConvert.SerializeObject(requestBody);
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key={_apiKey}"
            );
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}