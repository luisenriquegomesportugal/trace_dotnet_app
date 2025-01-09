using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            // Obt�m o endpoint do coletor OTLP da vari�vel de ambiente ou usa o valor padr�o
            var otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_OTLP_ENDPOINT") ?? "http://localhost:4317";

            services.AddLogging(logging =>
            {
                logging.ClearProviders(); // Remove provedores padr�o
                logging.AddConsole();    // Adiciona sa�da no console
                logging.AddDebug();      // Adiciona sa�da no Debug
                logging.SetMinimumLevel(LogLevel.Trace); // Define n�vel de log m�nimo
            });

            services.AddOpenTelemetry()
                .WithTracing(builder =>
                {
                    builder
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("TraceApp"))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(otlpEndpoint);
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
