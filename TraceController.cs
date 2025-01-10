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

        public TraceController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<IActionResult> GetTrace()
        {
            // External API Call
            var externalUrl = "https://jsonplaceholder.typicode.com/posts/1";
            var externaResponse = await _httpClient.GetAsync(externalUrl);
            
            externaResponse.EnsureSuccessStatusCode();
            var externalResponseBody = await externaResponse.Content.ReadAsStringAsync();

            // Internal API Call
            var internalUrl = "http://companhias-api-openbanking-services-hml.apps.ocp.desenv.com/open-banking/companies/v1";
            var internalResponse = await _httpClient.GetAsync(internalUrl);
            
            internalResponse.EnsureSuccessStatusCode();
            var internalResponseBody = await internalResponse.Content.ReadAsStringAsync();

            return Ok(new { internalResponseBody, externalResponseBody, status = "OK" });
        }
    }
}