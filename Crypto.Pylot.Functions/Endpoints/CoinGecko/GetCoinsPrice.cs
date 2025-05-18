using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Crypto.Pylot.Functions.Models.Options;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

namespace Crypto.Pylot.Functions.Endpoints
{
    public class GetCoinsPrice(ILogger<GetCoinsPrice> logger, IOptions<CoinGeckoOptions> coinGeckoOptions)
    {
        private readonly ILogger<GetCoinsPrice> _logger = logger;
        public readonly CoinGeckoOptions coinGeckoOptions = coinGeckoOptions.Value;

        [Function("GetCoinsPrice")]
        [OpenApiOperation(operationId: "GetCoinsPrice", tags: new[] { "Price" }, Summary = "Get coin prices", Description = "Gets prices for specified coins from CoinGecko.")]
        [OpenApiParameter(name: "vs_currencies", In = ParameterLocation.Query, Required = false, Type = typeof(string), Summary = "Comma-separated list of vs currencies", Description = "e.g. eur,usd")]
        [OpenApiParameter(name: "ids", In = ParameterLocation.Query, Required = false, Type = typeof(string), Summary = "Comma-separated list of coin ids", Description = "e.g. bitcoin,solana")]
        [OpenApiParameter(name: "include_market_cap", In = ParameterLocation.Query, Required = false, Type = typeof(bool), Summary = "Include market cap", Description = "true/false")]
        [OpenApiParameter(name: "include_24hr_vol", In = ParameterLocation.Query, Required = false, Type = typeof(bool), Summary = "Include 24hr volume", Description = "true/false")]
        [OpenApiParameter(name: "include_24hr_change", In = ParameterLocation.Query, Required = false, Type = typeof(bool), Summary = "Include 24hr change", Description = "true/false")]
        [OpenApiParameter(name: "include_last_updated_at", In = ParameterLocation.Query, Required = false, Type = typeof(bool), Summary = "Include last updated at", Description = "true/false")]
        [OpenApiParameter(name: "precision", In = ParameterLocation.Query, Required = false, Type = typeof(int), Summary = "Decimal precision", Description = "e.g. 2")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Dictionary<string, object>), Summary = "Coin price data", Description = "Returns coin price data")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("Get Coins Price Function HTTP trigger function processed a request.");

            // Default values
            string vsCurrencies = req.Query["vs_currencies"];
            if (string.IsNullOrWhiteSpace(vsCurrencies)) vsCurrencies = "eur";

            string ids = req.Query["ids"];
            if (string.IsNullOrWhiteSpace(ids)) ids = "bitcoin,solana";

            bool includeMarketCap = bool.TryParse(req.Query["include_market_cap"], out var imc) ? imc : false;
            bool include24hrVol = bool.TryParse(req.Query["include_24hr_vol"], out var i24v) ? i24v : false;
            bool include24hrChange = bool.TryParse(req.Query["include_24hr_change"], out var i24c) ? i24c : false;
            bool includeLastUpdatedAt = bool.TryParse(req.Query["include_last_updated_at"], out var ilua) ? ilua : false;
            int precision = int.TryParse(req.Query["precision"], out var p) ? p : 2;

            var url = $"{coinGeckoOptions.BaseUrl}/simple/price" +
                $"?vs_currencies={vsCurrencies}" +
                $"&ids={ids}" +
                $"&include_market_cap={includeMarketCap.ToString().ToLower()}" +
                $"&include_24hr_vol={include24hrVol.ToString().ToLower()}" +
                $"&include_24hr_change={include24hrChange.ToString().ToLower()}" +
                $"&include_last_updated_at={includeLastUpdatedAt.ToString().ToLower()}" +
                $"&precision={precision}";

            var client = new HttpClient();
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new BadRequestObjectResult("Error fetching data from CoinGecko API.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var priceData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);

            return new OkObjectResult(priceData);
        }
    }
}
