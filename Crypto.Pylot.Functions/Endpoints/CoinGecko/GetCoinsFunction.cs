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
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;


namespace Crypto.Pylot.Functions.Endpoints
{
    public class GetCoinsFunction(ILogger<GetCoinsFunction> logger, IOptions<CoinGeckoOptions> coinGeckoOptions)
    {
        private readonly ILogger<GetCoinsFunction> _logger = logger;
        public readonly CoinGeckoOptions coinGeckoOptions = coinGeckoOptions.Value;

        /// <summary>
        /// Gets the list of all coins from CoinGecko.
        /// </summary>
        /// <remarks>
        /// Returns a list of all coins with their ids, symbols, and names.
        /// </remarks>
        [Function("GetCoins")]
        [OpenApiOperation(operationId: "GetCoins", tags: new[] { "Coins" }, Summary = "Get all coins", Description = "Returns a list of all coins from CoinGecko.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Crypto.Pylot.Functions.Models.CoinGeckoCoin>), Summary = "List of coins", Description = "A list of all coins.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Summary = "Bad request", Description = "Error fetching data from CoinGecko API.")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("Get Coins Function HTTP trigger function processed a request.");

            var listCoinsUrl = $"{coinGeckoOptions.BaseUrl}/coins/list";

            var client = new HttpClient();
            var response = client.GetAsync(listCoinsUrl).Result;

            if (!response.IsSuccessStatusCode)
            {
                return new BadRequestObjectResult("Error fetching data from CoinGecko API.");
            }

            var responseContent = response.Content.ReadAsStringAsync().Result;
            var coinsList = JsonSerializer.Deserialize<List<CoinGeckoCoin>>(responseContent);

            return new OkObjectResult(coinsList);
        }
    }
}
