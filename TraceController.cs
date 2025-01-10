using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TraceApp
{
    [ApiController]
    [Route("[controller]")]
    public class TraceController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public TraceController()
        {
            _httpClient = new HttpClient(new HttpClientHandler());
        }

        [HttpGet]
        public async Task<IActionResult> GetTrace()
        {
            var externalUrl = "https://jsonplaceholder.typicode.com/posts/1";
            var response = await _httpClient.GetAsync(externalUrl);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return Ok(new { responseBody, status = "OK" });
        }
    }
}