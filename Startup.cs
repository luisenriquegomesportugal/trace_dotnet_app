using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace TraceApp
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_OTLP_ENDPOINT") ?? "http://localhost:4317";

            services.AddLogging(logging =>
            {
                logging.ClearProviders(); 
                logging.AddConsole(); 
                logging.AddDebug();    
                logging.SetMinimumLevel(LogLevel.Error);
                logging.AddOpenTelemetry(options =>
                {
                    options
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("TraceApp"))
                        .AddConsoleExporter()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(otlpEndpoint);
                        });
                });

            });

            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService("TraceApp"))
                .WithTracing(tracing =>
                {
                    tracing
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter()
                    .AddOtlpExporter(exporter =>
                    {
                        exporter.Endpoint = new Uri(otlpEndpoint);
                    });
                })
                .WithMetrics(metrics =>
                {
                    metrics
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter()
                    .AddOtlpExporter(exporter =>
                    {
                        exporter.Endpoint = new Uri(otlpEndpoint);
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
        }
    }
}
