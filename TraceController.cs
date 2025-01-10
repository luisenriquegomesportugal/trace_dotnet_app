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
            _httpClient = new HttpClient { };
        }

        [HttpGet]
        public async Task<IActionResult> GetTrace()
        {
            // Internal API Call
            var internalUrl = "http://companhias-api-openbanking-services-hml.apps.ocp.desenv.com/open-banking/companies/v1";
            var internalResponse = await _httpClient.GetAsync(internalUrl);

            var internalResponseBody = "error";
            if (internalResponse.IsSuccessStatusCode)
            {
                internalResponseBody = await internalResponse.Content.ReadAsStringAsync();
            }

            return Ok(new { internalResponseBody, status = "OK" });
        }
    }
}