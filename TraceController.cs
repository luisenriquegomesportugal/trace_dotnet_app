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
            var internalResponseBody = "";
            var externalResponseBody = "";

            var callExternal = Environment.GetEnvironmentVariable("TRACE_APP_EXTERNAL_CALL");

            // Internal API Call
            var internalUrl = "http://companhias-api-openbanking-services-hml.apps.ocp.desenv.com/open-banking/companies/v1";
            var internalResponse = await _httpClient.GetAsync(internalUrl);

            if (internalResponse.IsSuccessStatusCode)
            {
                internalResponseBody = await internalResponse.Content.ReadAsStringAsync();
            }
            else
            {
                internalResponseBody = "error";
            }

            if (callExternal != null)
            {
                // External API Call
                var externalResponse = await _httpClient.GetAsync(callExternal);
                if (externalResponse.IsSuccessStatusCode)
                {
                    externalResponseBody = await externalResponse.Content.ReadAsStringAsync();
                }
                else
                {
                    externalResponseBody = "error";
                }
            }

            return Ok(new { internalResponseBody, externalResponseBody, status = "OK" });
        }
    }
}