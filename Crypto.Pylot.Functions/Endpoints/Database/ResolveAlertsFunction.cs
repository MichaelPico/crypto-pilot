using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Timer;
using Microsoft.Extensions.Logging;
using Crypto.Pylot.Functions.Helpers;
using Crypto.Pylot.Functions.Models;

namespace Crypto.Pylot.Functions.Endpoints.Database
{
    public class ResolveAlertsFunction
    {
        private readonly ILogger<ResolveAlertsFunction> _logger;
        private readonly DatabaseHelper _dbHelper;
        private readonly CoinGeckoHelper _coinGeckoHelper;
        private readonly EmailServiceHelper _emailHelper;

        public ResolveAlertsFunction(
            ILogger<ResolveAlertsFunction> logger,
            DatabaseHelper dbHelper,
            CoinGeckoHelper coinGeckoHelper,
            EmailServiceHelper emailHelper)
        {
            _logger = logger;
            _dbHelper = dbHelper;
            _coinGeckoHelper = coinGeckoHelper;
            _emailHelper = emailHelper;
        }

        [Function("ResolveAlerts")]
        public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"ResolveAlerts function executed at: {DateTime.UtcNow}");

            // 1. Get all alerts
            var alerts = await _dbHelper.GetAllAlertsAsync();

            // 2. Filter alerts where Notified == false
            var pendingAlerts = alerts.Where(a => !a.Notified).ToList();
            if (!pendingAlerts.Any())
                return;

            // 3. Get distinct cryptocurrency IDs
            var cryptoIds = pendingAlerts.Select(a => a.CryptocurrencyId).Distinct().ToList();

            // 4. Get all cryptocurrencies from DB to map id->name
            var allCryptos = await _dbHelper.GetAllCryptocurrenciesAsync();
            var cryptoIdToName = allCryptos.ToDictionary(c => c.Id, c => c.Name);

            // 5. Prepare CoinGecko ids (assume name is CoinGecko id)
            var coinGeckoIds = cryptoIds.Select(id => cryptoIdToName.ContainsKey(id) ? cryptoIdToName[id] : null)
                                        .Where(n => !string.IsNullOrEmpty(n))
                                        .Distinct()
                                        .ToList();
            if (!coinGeckoIds.Any())
                return;

            // 6. Fetch prices from CoinGecko
            var prices = await _coinGeckoHelper.GetPricesAsync(
                "eur", // or use your default currency
                string.Join(",", coinGeckoIds),
                false, false, false, false, 2
            );

            // 7. For each alert, check if target price is reached
            foreach (var alert in pendingAlerts)
            {
                if (!cryptoIdToName.TryGetValue(alert.CryptocurrencyId, out var coinGeckoId))
                    continue;

                if (!prices.TryGetValue(coinGeckoId, out var priceObj))
                    continue;

                var priceDict = priceObj as System.Text.Json.JsonElement?;
                double currentPrice = 0;
                if (priceDict.HasValue && priceDict.Value.TryGetProperty("eur", out var priceVal))
                {
                    currentPrice = priceVal.GetDouble();
                }
                else if (priceObj is System.Text.Json.JsonElement elem && elem.TryGetProperty("eur", out var priceVal2))
                {
                    currentPrice = priceVal2.GetDouble();
                }
                else if (priceObj is Dictionary<string, object> dict && dict.TryGetValue("eur", out var val) && val is double d)
                {
                    currentPrice = d;
                }
                else
                {
                    continue;
                }

                bool triggered = alert.OverThePrice
                    ? currentPrice >= alert.TargetPrice
                    : currentPrice <= alert.TargetPrice;

                if (triggered)
                {
                    // Get user
                    var users = await _dbHelper.GetAllUsersAsync();
                    var user = users.FirstOrDefault(u => u.Id == alert.UserId);
                    if (user == null)
                        continue;

                    // Send email
                    await _emailHelper.SendCryptoAlertAsync(
                        user.Email,
                        coinGeckoId,
                        coinGeckoId,
                        alert.TargetPrice,
                        "EUR",
                        currentPrice,
                        DateTime.UtcNow
                    );

                    // Mark as notified
                    alert.Notified = true;
                    await _dbHelper.UpsertAlertAsync(alert);
                }
            }

            // 8. Update prices in DB
            foreach (var cryptoId in cryptoIds)
            {
                if (!cryptoIdToName.TryGetValue(cryptoId, out var coinGeckoId))
                    continue;
                if (!prices.TryGetValue(coinGeckoId, out var priceObj))
                    continue;

                double currentPrice = 0;
                var priceDict = priceObj as System.Text.Json.JsonElement?;
                if (priceDict.HasValue && priceDict.Value.TryGetProperty("eur", out var priceVal))
                {
                    currentPrice = priceVal.GetDouble();
                }
                else if (priceObj is System.Text.Json.JsonElement elem && elem.TryGetProperty("eur", out var priceVal2))
                {
                    currentPrice = priceVal2.GetDouble();
                }
                else if (priceObj is Dictionary<string, object> dict && dict.TryGetValue("eur", out var val) && val is double d)
                {
                    currentPrice = d;
                }
                else
                {
                    continue;
                }

                await _dbHelper.UpsertCryptocurrencyAsync(new Models.Cryptocurrency
                {
                    Id = cryptoId,
                    Name = coinGeckoId,
                    CurrentPrice = currentPrice
                });
            }
        }
    }
}
