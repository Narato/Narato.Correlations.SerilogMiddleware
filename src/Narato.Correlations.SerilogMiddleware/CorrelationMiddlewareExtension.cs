using Microsoft.AspNetCore.Builder;
using Narato.Correlations.SerilogMiddleware.Middleware;
using System;

namespace Narato.Correlations.SerilogMiddleware
{
    public static class CorrelationMiddlewareExtension
    {
        public static IApplicationBuilder UseCorrelationLogging(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<CorrelationSeriLogEnrichingMiddleware>();
        }
    }
}
