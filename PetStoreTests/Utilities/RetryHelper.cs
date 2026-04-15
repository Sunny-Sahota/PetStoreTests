using RestSharp;
using System.Net;

namespace PetStoreTests.Utilities
{
    public static class RetryHelper
    {
        // Shared helpers (Retry)
        public static T RetryUntil<T>(
        Func<T> action,
        Func<T, bool> condition,
        int retries = 5,
        int delayMs = 500)
        {
            for (int i = 0; i < retries; i++)
            {
                var result = action();

                if (condition(result))
                    return result;

                Thread.Sleep(delayMs);
            }

            throw new Exception("Retry condition was not met within the allowed attempts.");
        }
    }
}
