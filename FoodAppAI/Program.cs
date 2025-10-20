using System;
using System.IO;
using System.Threading.Tasks;
using FoodAppAI.Api;
using FoodAppAI.Models;
using Newtonsoft.Json;

namespace FoodAppAI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Zadej název jídla / ingredience: ");
            string foodName = Console.ReadLine();

            // Načti klíče (z prostředí nebo z konfigurace)
            string edamamAppId = Environment.GetEnvironmentVariable("EDAMAM_APP_ID");
            string edamamAppKey = Environment.GetEnvironmentVariable("EDAMAM_APP_KEY");
            string geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");

            if (string.IsNullOrEmpty(edamamAppId) || string.IsNullOrEmpty(edamamAppKey) || string.IsNullOrEmpty(geminiApiKey))
            {
                Console.WriteLine("Nejsou nastaveny všechny klíče (EDAMAM_APP_ID, EDAMAM_APP_KEY, GEMINI_API_KEY).");
                return;
            }

            var foodApi = new FoodApi(edamamAppId, edamamAppKey);

            try
            {
                string rawJson = await foodApi.GetFoodDataRawAsync(foodName);
                Console.WriteLine("\n--- RAW JSON z Food API ---");
                Console.WriteLine(rawJson);

                // Volitelně: parse do objektu
                FoodInfo info = foodApi.ParseFoodInfo(rawJson);
                if (info != null)
                {
                    Console.WriteLine("\n--- Parsed FoodInfo ---");
                    Console.WriteLine($"Label: {info.Label}");
                    Console.WriteLine($"Calories: {info.Calories}");
                    Console.WriteLine($"Fat: {info.Fat}");
                    Console.WriteLine($"Carbs: {info.Carbs}");
                    Console.WriteLine($"Protein: {info.Protein}");
                }

                // Pošleme raw JSON (nebo parsed JSON) do Gemini pro formátování
                var gemini = new GeminiApi(geminiApiKey);
                string refined = await gemini.RefineFoodJsonAsync(rawJson);

                Console.WriteLine("\n--- REFINED JSON from Gemini ---");
                Console.WriteLine(refined);

                // Uložit do souboru
                string outputPath = "output.json";
                File.WriteAllText(outputPath, refined);
                Console.WriteLine($"\nUloženo do souboru: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Chyba při volání API: " + ex.Message);
            }
        }
    }
}
