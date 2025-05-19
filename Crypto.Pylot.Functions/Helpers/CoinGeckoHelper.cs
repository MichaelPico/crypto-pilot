using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Crypto.Pylot.Functions.Models;
using Crypto.Pylot.Functions.Models.Options;
using Microsoft.Extensions.Options;

namespace Crypto.Pylot.Functions.Helpers
{
    public class CoinGeckoHelper
    {
        private readonly HttpClient _httpClient;
        private readonly CoinGeckoOptions _options;

        public CoinGeckoHelper(IHttpClientFactory httpClientFactory, IOptions<CoinGeckoOptions> options)
        {
            _httpClient = httpClientFactory.CreateClient();
            _options = options.Value;
        }

        public async Task<List<CoinGeckoCoin>> GetCoinsAsync()
        {
            var url = $"{_options.BaseUrl}/coins/list";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CoinGeckoCoin>>(content);
        }

        public async Task<List<string>> GetSupportedCurrenciesAsync()
        {
            var url = $"{_options.BaseUrl}/simple/supported_vs_currencies";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<string>>(content);
        }

        public async Task<Dictionary<string, object>> GetPricesAsync(
            string vsCurrencies, string ids, bool includeMarketCap, bool include24hrVol,
            bool include24hrChange, bool includeLastUpdatedAt, int precision)
        {
            var url = $"{_options.BaseUrl}/simple/price" +
                $"?vs_currencies={vsCurrencies}" +
                $"&ids={ids}" +
                $"&include_market_cap={includeMarketCap.ToString().ToLower()}" +
                $"&include_24hr_vol={include24hrVol.ToString().ToLower()}" +
                $"&include_24hr_change={include24hrChange.ToString().ToLower()}" +
                $"&include_last_updated_at={includeLastUpdatedAt.ToString().ToLower()}" +
                $"&precision={precision}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Dictionary<string, object>>(content);
        }
    }
}
