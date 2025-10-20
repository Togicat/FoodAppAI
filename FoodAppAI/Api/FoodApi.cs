using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace FoodAppAI.Api
{
    public class FoodApi
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _appId;
        private readonly string _appKey;

        public FoodApi(string appId, string appKey)
        {
            _appId = appId;
            _appKey = appKey;
        }

        /// <summary>
        /// Vrátí surový JSON string z Edamam pro daný ingredienci/jídlo.
        /// </summary>
        public async Task<string> GetFoodDataRawAsync(string foodName)
        {
            // endpoint Edamam Food (Food Database API)
            // Dokumentace Edamam: https://developer.edamam.com/edamam-docs-food-database-api  
            // Použij parametr "ingr" pro hledání
            string url = $"https://api.edamam.com/api/food-database/v2/parser?app_id={_appId}&app_key={_appKey}&ingr={Uri.EscapeDataString(foodName)}";
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        /// <summary>
        /// Parse JSON z Edamam a vrátí instanci FoodInfo (první nalezený záznam).
        /// </summary>
        public FoodAppAI.Models.FoodInfo ParseFoodInfo(string rawJson)
        {
            var j = JObject.Parse(rawJson);

            // Edamam parser vrací objekty "hints" – seznam možných potravin
            var hints = j["hints"];
            if (hints != null && hints.HasValues)
            {
                var first = hints[0];
                var food = first["food"];
                var nutrients = first["food"]?["nutrients"];
                if (food != null && nutrients != null)
                {
                    var info = new FoodAppAI.Models.FoodInfo
                    {
                        Label = (string)food["label"],
                        Calories = (double?)(nutrients["ENERC_KCAL"] ?? 0) ?? 0,
                        Fat = (double?)(nutrients["FAT"] ?? 0) ?? 0,
                        Carbs = (double?)(nutrients["CHOCDF"] ?? 0) ?? 0,
                        Protein = (double?)(nutrients["PROCNT"] ?? 0) ?? 0,
                        TotalWeight = (double?)(first["measure"]?["weight"] ?? 0) ?? 0
                    };
                    return info;
                }
            }

            return null;
        }
    }
}
