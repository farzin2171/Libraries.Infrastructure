using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog.Sinks.Http;

namespace WT.Logging.LoggingService
{
    public class HttpLoggingClient : IHttpClient
    {
        private readonly HttpClient _client;

        public HttpLoggingClient(string apiKey, string clientId)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("X-Client-Id", clientId);
            _client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        }

        public void Configure(IConfiguration configuration) { }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, Stream contentStream)
        {
            using (var content = new StreamContent(contentStream))
            {
                content.Headers.Add("Content-Type", "application/json");
                return await _client.PostAsync(requestUri, content);
            }

        }

        public void Dispose() => _client.Dispose();
    }
}
