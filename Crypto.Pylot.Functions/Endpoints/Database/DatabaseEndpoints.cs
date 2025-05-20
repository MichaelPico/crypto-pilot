using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Crypto.Pylot.Functions.Helpers;
using Crypto.Pylot.Functions.Models;
using Crypto.Pylot.Functions.Models.Options;

namespace Crypto.Pylot.Functions.Endpoints.Database
{
    public class DatabaseEndpoints
    {
        private readonly ILogger<DatabaseEndpoints> _logger;
        private readonly DatabaseHelper _dbHelper;

        public DatabaseEndpoints(
            ILogger<DatabaseEndpoints> logger,
            DatabaseHelper dbHelper)
        {
            _logger = logger;
            _dbHelper = dbHelper;
        }

        // USERS

        [Function("GetAllUsers")]
        [OpenApiOperation(operationId: "GetAllUsers", tags: new[] { "Database", "Users" }, Summary = "Get all users", Description = "Returns all users from the database.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<User>), Summary = "List of users", Description = "A list of all users.")]
        public async Task<IActionResult> GetAllUsers([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "database/users")] HttpRequest req)
        {
            var users = await _dbHelper.GetAllUsersAsync();
            return new OkObjectResult(users);
        }

        [Function("UpsertUser")]
        [OpenApiOperation(operationId: "UpsertUser", tags: new[] { "Database", "Users" }, Summary = "Upsert user", Description = "Insert or update a user.")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(User), Required = true, Description = "User object")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Summary = "Success", Description = "User upserted.")]
        public async Task<IActionResult> UpsertUser([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "database/users")] HttpRequest req)
        {
            try
            {
                _logger.LogInformation("UpsertUser function triggered.");
                var user = await System.Text.Json.JsonSerializer.DeserializeAsync<User>(req.Body);

                if (user == null)
                {
                    _logger.LogError("Invalid user object received.");
                    return new BadRequestObjectResult("Invalid user object.");
                }

                _logger.LogInformation("Upserting user: {User}", user);
                await _dbHelper.UpsertUserAsync(user);

                _logger.LogInformation("User upserted successfully.");
                return new OkObjectResult("User upserted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting user.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        // CRYPTOCURRENCIES

        [Function("GetAllCryptocurrencies")]
        [OpenApiOperation(operationId: "GetAllCryptocurrencies", tags: new[] { "Database", "Cryptocurrencies" }, Summary = "Get all cryptocurrencies", Description = "Returns all cryptocurrencies from the database.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Cryptocurrency>), Summary = "List of cryptocurrencies", Description = "A list of all cryptocurrencies.")]
        public async Task<IActionResult> GetAllCryptocurrencies([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "database/cryptocurrencies")] HttpRequest req)
        {
            var cryptos = await _dbHelper.GetAllCryptocurrenciesAsync();
            return new OkObjectResult(cryptos);
        }

        [Function("UpsertCryptocurrency")]
        [OpenApiOperation(operationId: "UpsertCryptocurrency", tags: new[] { "Database", "Cryptocurrencies" }, Summary = "Upsert cryptocurrency", Description = "Insert or update a cryptocurrency.")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Cryptocurrency), Required = true, Description = "Cryptocurrency object")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Summary = "Success", Description = "Cryptocurrency upserted.")]
        public async Task<IActionResult> UpsertCryptocurrency([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "database/cryptocurrencies")] HttpRequest req)
        {
            var crypto = await System.Text.Json.JsonSerializer.DeserializeAsync<Cryptocurrency>(req.Body);
            await _dbHelper.UpsertCryptocurrencyAsync(crypto);
            return new OkObjectResult("Cryptocurrency upserted.");
        }

        // ALERTS

        [Function("GetAllAlerts")]
        [OpenApiOperation(operationId: "GetAllAlerts", tags: new[] { "Database", "Alerts" }, Summary = "Get all alerts", Description = "Returns all alerts from the database.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Alert>), Summary = "List of alerts", Description = "A list of all alerts.")]
        public async Task<IActionResult> GetAllAlerts([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "database/alerts")] HttpRequest req)
        {
            var alerts = await _dbHelper.GetAllAlertsAsync();
            return new OkObjectResult(alerts);
        }

        [Function("UpsertAlert")]
        [OpenApiOperation(operationId: "UpsertAlert", tags: new[] { "Database", "Alerts" }, Summary = "Upsert alert", Description = "Insert or update an alert.")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Alert), Required = true, Description = "Alert object")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Summary = "Success", Description = "Alert upserted.")]
        public async Task<IActionResult> UpsertAlert([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "database/alerts")] HttpRequest req)
        {
            var alert = await System.Text.Json.JsonSerializer.DeserializeAsync<Alert>(req.Body);
            await _dbHelper.UpsertAlertAsync(alert);
            return new OkObjectResult("Alert upserted.");
        }
    }
}
