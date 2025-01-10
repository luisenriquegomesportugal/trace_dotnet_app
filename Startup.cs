using System;
using System.Net.Http;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace TraceApp
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        private readonly string _otlpEndpoint;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            _otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_OTLP_ENDPOINT") ?? "http://localhost:4317";
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(logging =>
            {
                logging.ClearProviders(); 
                logging.AddConsole(); 
                logging.AddDebug();    
                logging.SetMinimumLevel(LogLevel.Debug);
            });

            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService("TraceApp"))
                .WithTracing(tracing =>
                {
                    tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter()
                    .AddOtlpExporter(exporter =>
                    {
                        exporter.Endpoint = new Uri(this._otlpEndpoint);
                        exporter.HttpClientFactory = () => new HttpClient(new SocketsHttpHandler
                        {
                            UseProxy = false
                        });
                    });
                });

            services.AddHttpClient();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            logger.LogInformation("Application started and running in {Environment} environment", env.EnvironmentName);
            logger.LogInformation("Application connected on Grpc {Host} environment", this._otlpEndpoint);
        }
    }
}
