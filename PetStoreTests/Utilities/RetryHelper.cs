using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace PetStoreTests.Utilities
{
    public static class RetryHelper
    {
        // Shared helpers (Retry) - Polly v8 resilience pipeline 
        public static T RetryUntil<T>(Func<T> action, Func<T, bool> condition, int retries = 5, int delayMs = 500, string? context = null)
        {
            var failureMessage = string.IsNullOrEmpty(context)
                ? $"Retry condition was not met after {retries} attempts (delay: {delayMs})"
                : $"Retry condition was not met after {retries} attempts (delay: {delayMs}), context: {context}";

            // Same "handled outcome" definition for BOTH strategies, so the breaker and the retry agree on what a failure is:
            //   - a result that fails the caller's condition, or
            //   - a thrown network/transport exception.
            // NOTE: RestSharp's sync Execute() does not throw (it sets ResponseStatus/ErrorException),
            // so the exception clauses are mostly future-proofing for async/throw clients.
            PredicateBuilder<T> HandledOutcomes() => new PredicateBuilder<T>()
                .HandleResult(result => !condition(result))
                .Handle<HttpRequestException>()
                .Handle<TimeoutException>();

            var pipeline = new ResiliencePipelineBuilder<T>().AddRetry(new RetryStrategyOptions<T>
            {
                //  subtract 1 to preserve the old "retries = total attempts"
                MaxRetryAttempts = Math.Max(retries - 1, 0),
                Delay = TimeSpan.FromMilliseconds(delayMs),

                // Constant matches the old Thread.Sleep(delayMs) behavior exactly. Swap the next two lines for:
                    //   BackoffType = DelayBackoffType.Exponential,
                    //   MaxDelay = TimeSpan.FromSeconds(2),

                BackoffType = DelayBackoffType.Constant,

                // ±25% randomization so retries don't stampede the shared API
                UseJitter = true,

                // Retry while the caller's condition is NOT satisfied.
                ShouldHandle = HandledOutcomes(),

                // Structured hook — replaces the old silent Thread.Sleep.
                OnRetry = args =>
                {
                    Console.WriteLine(
                        $"[RetryHelper] {context ?? "operation"}: condition not met, " +
                        $"retry {args.AttemptNumber + 1} of {retries - 1}.");
                    return ValueTask.CompletedTask;
                },
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<T>
            {
                ShouldHandle = HandledOutcomes(),
                FailureRatio = 0.5,                             // open when >=50% of sampled attempts fail
                MinimumThroughput = 4,                          // lower from 100 so a 5-attempt chain can trip it
                SamplingDuration = TimeSpan.FromSeconds(10),
                BreakDuration = TimeSpan.FromSeconds(12),
                OnOpened = args =>
                {
                    Console.WriteLine($"[Retry Helper] { context ?? "operation"} : Circuit OPEN - failing fast");
                    return ValueTask.CompletedTask;
                },
                OnClosed = args =>
                {
                    Console.WriteLine($"[Retry Helper] { context ?? "operation"} : Circuit CLOSED.");
                    return ValueTask.CompletedTask;
                },
            })
            .Build();

            T result;
            try
            {
                result = pipeline.Execute(() => action());
            }
            catch (BrokenCircuitException ex)
            {
                // Breaker opened: fail loud
                throw new InvalidOperationException(failureMessage, ex);
            }

            // Result-based pipelines RETURN the last result when retries are exhausted
            if (!condition(result))
            {
                throw new InvalidOperationException(failureMessage);
            }

            return result;            
        }
    }
}
