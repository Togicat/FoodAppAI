using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FoodAppAI.Api
{
    public class FoodApi
    {
        private readonly string _clientId = "9798d3eba2ff4f9e9d4cf5e449918449"; // consumer key
        private readonly string _clientSecret = "b3cb78091581438485bb92a5bca3ca96"; // consumer secret
        private readonly HttpClient _httpClient;

        public FoodApi()
        {
            _httpClient = new HttpClient();
        }

        
        private async Task<string> GetAccessTokenAsync()
        {
            var tokenUrl = "https://oauth.fatsecret.com/connect/token";

            var clientCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
            var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", clientCredentials);
            request.Content = new StringContent("grant_type=client_credentials&scope=basic", Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Chyba při získávání tokenu: {response.StatusCode}\n{error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("access_token").GetString();
        }

        
        public async Task<string> GetFoodInfoAsync(int foodId)
        {
            var token = await GetAccessTokenAsync();

            var requestUrl = $"https://platform.fatsecret.com/rest/food/v5?food_id={foodId}&format=json";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Chyba při získávání dat o jídle: {response.StatusCode}\n{error}");
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
