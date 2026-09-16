using Newtonsoft.Json;

namespace PetStoreTests.Utilities
{
    public static class JsonHelper
    {
        // Shared helpers (JSON)
        public static T DeserializeOrThrow<T>(string? content)
        {
            if (string.IsNullOrEmpty(content))
                throw new InvalidOperationException($"Cannot deserialize {typeof(T).Name} : Response Content was null or empty.");

            return JsonConvert.DeserializeObject<T>(content)
                   ?? throw new InvalidOperationException($"Failed to deserialize {typeof(T).Name} from response content: \"{Truncate(content,200)}\"");
        }

        private static string Truncate(string value, int maxLength)
        {
            return value.Length <= maxLength ? value : value[..maxLength] + "...";
        }
    }
}
