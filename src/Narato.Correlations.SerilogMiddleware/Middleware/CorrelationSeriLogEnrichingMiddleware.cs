using Microsoft.AspNetCore.Http;
using Narato.Correlations.Correlations;
using Serilog.Context;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Narato.Correlations.SerilogMiddleware.Middleware
{
    public class CorrelationSeriLogEnrichingMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationSeriLogEnrichingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            if (!context.Response.Headers.ContainsKey(CorrelationIdProvider.CORRELATION_ID_HEADER_NAME))
            {
                throw new Exception("No correlation ID was found on the response headers. Did you set up Narato.Correlations correctly?");
            }

            using (LogContext.PushProperty("CorrelationId", context.Response.Headers[CorrelationIdProvider.CORRELATION_ID_HEADER_NAME].First()))
            {
                return _next(context);
            }
        }
    }
}
