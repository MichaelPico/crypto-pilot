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
using System.Threading.Tasks;
using Crypto.Pylot.Functions.Helpers;


namespace Crypto.Pylot.Functions.Endpoints
{
    public class GetCurrenciesFunction
    {
        private readonly ILogger<GetCurrenciesFunction> _logger;
        private readonly CoinGeckoHelper _coinGeckoHelper;

        public GetCurrenciesFunction(ILogger<GetCurrenciesFunction> logger, CoinGeckoHelper coinGeckoHelper)
        {
            _logger = logger;
            _coinGeckoHelper = coinGeckoHelper;
        }

        [Function("GetCurrencies")]
        [OpenApiOperation(operationId: "GetCurrencies", tags: new[] { "Currencies" }, Summary = "Get supported currencies", Description = "Returns a list of supported currency codes from CoinGecko.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<string>), Summary = "List of supported currencies", Description = "A list of supported currency codes.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Summary = "Bad request", Description = "Error fetching data from CoinGecko API.")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("Get Currencies Function HTTP trigger function processed a request.");

            try
            {
                var currenciesList = await _coinGeckoHelper.GetSupportedCurrenciesAsync();
                return new OkObjectResult(currenciesList);
            }
            catch
            {
                return new BadRequestObjectResult("Error fetching data from CoinGecko API.");
            }
        }
    }
}
