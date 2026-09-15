
using Microsoft.Extensions.Configuration;

namespace PetStoreTests.Config
{
    public static class ApiConfig
    {
        //public static string BaseUrl => "https://petstore.swagger.io/v2";
        
        private const string DefaultBaseUrl = "https://petstore.swagger.io/v2";

        public static string BaseUrl {get;} = LoadBaseUrl();

        private static string LoadBaseUrl()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json",optional:true)
                .AddEnvironmentVariables()
                .Build();

            var baseUrl = config["BaseUrl"];
            return string.IsNullOrWhiteSpace(baseUrl) ? DefaultBaseUrl : baseUrl;
        }
    }
}
