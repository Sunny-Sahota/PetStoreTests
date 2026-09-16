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
        int delayMs = 500,
        string? context = null)
        {
            for (int i = 0; i < retries; i++)
            {
                var result = action();

                if (condition(result))
                    return result;

                Thread.Sleep(delayMs);
            }

            var message = string.IsNullOrEmpty(context)
                ? $"Retry condition was not met after {retries} attempts (delay: {delayMs})."
                : $"Retry condition was not met after {retries} attempts (delay: {delayMs}). Context: {context}";
            
            throw new InvalidOperationException(message);
        }
    }
}
