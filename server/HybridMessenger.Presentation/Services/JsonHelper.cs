using System.Dynamic;
using System.Text.Json;

namespace HybridMessenger.Presentation.Services
{
    public class JsonHelper
    {
        public static object GetDynamicValue(string jsonItem, string fieldName)
        {
            try
            {
                var item = JsonSerializer.Deserialize<ExpandoObject>(jsonItem, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (item is IDictionary<string, object> dictionary && dictionary.ContainsKey(fieldName))
                {
                    return dictionary[fieldName] ?? "N/A";
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON parsing error: {ex.Message}");
            }
            return "N/A";
        }
    }
}
