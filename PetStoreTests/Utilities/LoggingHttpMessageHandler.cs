using System.Diagnostics;
using System.Text;

namespace PetStoreTests.Utilities
{
    public class LoggingHttpMessageHandler : DelegatingHandler
    {
        private readonly Action<string> _log;
        private readonly int _bodyLimit;

        public LoggingHttpMessageHandler(HttpMessageHandler innerHandler, Action<string> log, int bodyLimit = 2000)
        {
            InnerHandler = innerHandler ?? throw new ArgumentNullException(nameof(innerHandler));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _bodyLimit = bodyLimit;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var elapsed = Stopwatch.StartNew();

            try
            {
                await LogRequestAsync(request);
                var response = await base.SendAsync(request, cancellationToken);

                await LogResponseAsync(request,response,elapsed.ElapsedMilliseconds);
                return response;
            }
            catch (Exception ex)
            {
                // Network-level failure (no HTTP response at all)
                _log($"[HTTP] << {request.Method} {request.RequestUri} FAILED: {ex.GetType().Name}: \"{ex.Message}\" after {elapsed.ElapsedMilliseconds} ms");
                throw;
            }
        }

        private async Task LogRequestAsync(HttpRequestMessage request)
        {
            var line = $">> {request.Method} {request.RequestUri}";

            if(request.Content is null) // GET , DELETE have no body
            {
                _log(line);
                return;
            }
            if(request.Content is MultipartFormDataContent or StreamContent)
            {
                _log($"{line} [binary/multipart omitted ({request.Content.Headers.ContentType})]");
                return;
            }
            var (body, restored) = await ReadAndRestoreAsync(request.Content);
            request.Content = restored;         // put the bytes back for RestSharp
            _log(WithBody(line,body));
        }

        private async Task LogResponseAsync(HttpRequestMessage request, HttpResponseMessage response, long elapsedMs)
        {
            var line = $"<< {(int)response.StatusCode} {response.StatusCode}  {request.Method} {request.RequestUri}  in {elapsedMs} ms";

            if (response.Content is null)
            {
                _log(line);
                return;
            }

            var (body, restored) = await ReadAndRestoreAsync(response.Content);
            response.Content = restored;        // so RestSharp can still deserialize
            _log(WithBody(line, body));
        }

        // HttpContent is single-use. Read it now for logging, then swap in a fresh
        // ByteArrayContent holding the SAME bytes (and headers), keeping the body
        // readable by RestSharp afterwards.
        private static async Task<(string Body, HttpContent Restored)> ReadAndRestoreAsync(HttpContent content)
        {
            var bytes = await content.ReadAsByteArrayAsync();
            var restored = new ByteArrayContent(bytes);

            if (content.Headers.ContentType is { } type)
                restored.Headers.ContentType = type;

            return (Encoding.UTF8.GetString(bytes), restored);
        }

        private string WithBody(string headerLine, string body) => string.IsNullOrWhiteSpace(body)
                ? headerLine
                : headerLine + Environment.NewLine + Indent(Truncate(body));

        private string Truncate(string value) => value.Length <= _bodyLimit
                ? value
                : value[.._bodyLimit] + $"... ({value.Length} total chars)";

        private static string Indent(string value) =>
            string.Join(Environment.NewLine, value.Split('\n').Select(l => "  " + l));
    }
}