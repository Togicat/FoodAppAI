using System;
using System.Threading.Tasks;
using FoodAppAI.Api;

namespace FoodAppAI
{
    internal class Program
    {
        static async Task Main()
        {
            var foodApi = new FoodApi();
            
            Console.Clear();
            Console.Write("Zadej food_id (např. 33691): ");
            int id = int.Parse(Console.ReadLine());

            Console.WriteLine("\n⏳ info...");
            var json = await foodApi.GetFoodInfoAsync(id);

            Console.WriteLine("\n Vysledek:");
            Console.WriteLine(json);
        }
    }
}