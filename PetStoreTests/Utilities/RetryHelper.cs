using RestSharp;
using System.Net;

namespace PetStoreTests.Utilities
{
    public static class RetryHelper
    {
        public static RestResponse RetryUntilSuccess(Func<RestResponse> action,int retries = 5,int delayMs = 500)
        {
            for (int i = 0; i < retries; i++)
            {
                var response = action();
                // Have to check if response is OK 
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response.Content))                
                    return response;
                
                Thread.Sleep(delayMs);
            }

            throw new Exception("Request did not succeed after retries");
        }

        public static T RetryUntil<T>(Func<T?> action,int retries = 5,int delayMs = 500)
        {
            for (int i = 0; i < retries; i++)
            {
                var result = action();
                if (result != null)
                    return result;

                Thread.Sleep(delayMs);
            }

            throw new Exception("Condition not met after retries");
        }
    }
}
