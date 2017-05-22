using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Narato.Correlations.Correlations.Interfaces;
using Narato.Correlations.SerilogMiddleware;
using Serilog;
using System;
using Xunit;
using System.IO;

namespace Narato.Correlations.SerilogMiddlewareTest.Middleware
{
    public class CorrelationSeriLogEnrichingMiddlewareTest
    {

        [Fact]
        public async void TestLogMessagesAreEnriched()
        {
            StringWriter _writer = new StringWriter();
            Guid guid;
            // Arrange
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseCorrelations();
                    app.UseCorrelationLogging();

                    // endpoint
                    app.Run(async (HttpContext context) =>
                    {
                        var provider = app.ApplicationServices.GetService<ICorrelationIdProvider>();
                        guid = provider.GetCorrelationId();
                        Log.Debug("test");
                        await context.Response.WriteAsync("meep");
                    });

                })
                .ConfigureServices(services =>
                {
                    Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().MinimumLevel.Debug().WriteTo.TextWriter(_writer, outputTemplate: "[{Level} {CorrelationId}] {Message}{NewLine}{Exception}").CreateLogger();
                    services.AddCorrelations();
                });

            var server = new TestServer(builder);

            // Act
            var response = await server.CreateClient().GetAsync("/");

            // Assert
            var debugMessage = _writer.ToString();
            Assert.Equal($"[Debug {guid}] test\r\n", debugMessage);
        }
    }
}
