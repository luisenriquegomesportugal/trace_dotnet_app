using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace TraceApp
{
    [ApiController]
    [Route("[controller]")]
    public class TraceController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly Tracer _tracer;

        public TraceController(IHttpClientFactory httpClientFactory, TracerProvider tracerProvider)
        {
            _httpClient = httpClientFactory.CreateClient();
            _tracer = tracerProvider.GetTracer("MyCustomTracer");
        }

        [HttpGet]
        public async Task<IActionResult> GetTrace()
        {
            using (var span = _tracer.StartActiveSpan("TrackOperation"))
            {
                // Defina atributos ou outros detalhes no span
                span.SetAttribute("http.method", "GET");
                span.SetAttribute("http.url", "https://jsonplaceholder.typicode.com/posts/1");


                // Simule alguma operação
                var externalUrl = "https://jsonplaceholder.typicode.com/posts/1";
                var response = await _httpClient.GetAsync(externalUrl);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                return Ok(new { responseBody, status = "OK" });

            }
        }
    }
}
