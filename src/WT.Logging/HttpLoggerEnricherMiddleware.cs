using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace WT.Logging
{
    public class HttpLoggerEnricherMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpLoggerEnricherMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (LogContext.Push(new DefaultHttpLoggerEnricher(context)))
            {
                await _next(context);
            }
        }
    }
}
