using Newtonsoft.Json;

namespace PetStoreTests.Utilities
{
    public static class JsonHelper
    {
        public static T DeserializeOrThrow<T>(string? content)
        {
            if (string.IsNullOrEmpty(content))
                throw new Exception("Response content was null or empty");

            return JsonConvert.DeserializeObject<T>(content)
                   ?? throw new Exception("Failed to deserialize response");
        }
    }
}
