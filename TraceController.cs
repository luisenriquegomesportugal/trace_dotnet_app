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
            _tracer = tracerProvider.GetTracer("TraceApp.MyCustomTracer");
        }

        [HttpGet]
        public IActionResult GetTrace()
        {
            using (var span = _tracer.StartActiveSpan("TrackOperation"))
            {
                // Defina atributos ou outros detalhes no span
                span.SetAttribute("http.method", "GET");
                span.SetAttribute("http.url", "https://jsonplaceholder.typicode.com/posts/1");

                return Ok(new { Status = "OK" });

            }
        }
    }
}
