using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Crypto.Pylot.Functions.Models.Options;
using System.Net.Http;
using System.Text.Json;
using Crypto.Pylot.Functions.Models;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System.Net;


namespace Crypto.Pylot.Functions.Endpoints
{
    public class GetCurrenciesFunction(ILogger<GetCurrenciesFunction> logger, IOptions<CoinGeckoOptions> coinGeckoOptions)
    {
        private readonly ILogger<GetCurrenciesFunction> _logger = logger;
        public readonly CoinGeckoOptions coinGeckoOptions = coinGeckoOptions.Value;
        
        [Function("GetCurrencies")]
        [OpenApiOperation(operationId: "GetCurrencies", tags: new[] { "Currencies" }, Summary = "Get supported currencies", Description = "Returns a list of supported currency codes from CoinGecko.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<string>), Summary = "List of supported currencies", Description = "A list of supported currency codes.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Summary = "Bad request", Description = "Error fetching data from CoinGecko API.")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("Get Currencies Function HTTP trigger function processed a request.");

            var listCoinsUrl = $"{coinGeckoOptions.BaseUrl}/simple/supported_vs_currencies";

            var client = new HttpClient();
            var response = client.GetAsync(listCoinsUrl).Result;

            if (!response.IsSuccessStatusCode)
            {
                return new BadRequestObjectResult("Error fetching data from CoinGecko API.");
            }

            var responseContent = response.Content.ReadAsStringAsync().Result;
            var currenciesList = JsonSerializer.Deserialize<List<string>>(responseContent);

            return new OkObjectResult(currenciesList);
        }
    }
}
