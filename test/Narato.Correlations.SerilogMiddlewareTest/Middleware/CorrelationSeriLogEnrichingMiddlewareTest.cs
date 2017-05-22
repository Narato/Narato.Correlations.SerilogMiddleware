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

        [Fact]
        public async void TestWrongConfigShouldThrowException()
        {
            StringWriter _writer = new StringWriter();
            Guid guid;
            // Arrange
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    
                    app.UseCorrelationLogging();
                    // notice they're in wrong order here...
                    app.UseCorrelations();

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

            // Act + assert
            var ex = await Assert.ThrowsAsync<Exception>(async () => await server.CreateClient().GetAsync("/"));

            // Assert
            Assert.Equal("No correlation ID was found on the response headers. Did you set up Narato.Correlations correctly?", ex.Message);
        }
    }
}
