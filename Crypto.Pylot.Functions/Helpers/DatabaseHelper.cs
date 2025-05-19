using Crypto.Pylot.Functions.Models.Options;
using Microsoft.Data.SqlClient;
using Crypto.Pylot.Functions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;


namespace Crypto.Pylot.Functions.Helpers
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IOptions<CryptoPilotDatabaseOptions> dbOptions)
        {
            var options = dbOptions.Value;
            _connectionString =
                $"Server=tcp:{options.Server},1433;Initial Catalog={options.Database};Persist Security Info=False;User ID={options.User};Password={options.Password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // USERS
        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            using (var conn = CreateConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT id, name, email, phone_number FROM crypto_pilot.users", conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    users.Add(new User
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Email = reader.GetString(2),
                        PhoneNumber = reader.GetString(3)
                    });
                }
            }
            return users;
        }

        public async Task UpsertUserAsync(User user)
        {
            using (var conn = CreateConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
MERGE crypto_pilot.users AS target
USING (SELECT @Id AS id) AS source
ON (target.id = source.id AND target.id != -1)
WHEN MATCHED THEN 
    UPDATE SET name = @Name, email = @Email, phone_number = @PhoneNumber
WHEN NOT MATCHED THEN
    INSERT (name, email, phone_number) VALUES (@Name, @Email, @PhoneNumber);", conn);

                cmd.Parameters.AddWithValue("@Id", user.Id);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        // CRYPTOCURRENCIES
        public async Task<List<Cryptocurrency>> GetAllCryptocurrenciesAsync()
        {
            var cryptos = new List<Cryptocurrency>();
            using (var conn = CreateConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT id, name, current_price FROM crypto_pilot.cryptocurrencies", conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    cryptos.Add(new Cryptocurrency
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        CurrentPrice = reader.GetDouble(2)
                    });
                }
            }
            return cryptos;
        }

        public async Task UpsertCryptocurrencyAsync(Cryptocurrency crypto)
        {
            using (var conn = CreateConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
MERGE crypto_pilot.cryptocurrencies AS target
USING (SELECT @Id AS id) AS source
ON (target.id = source.id AND target.id != -1)
WHEN MATCHED THEN 
    UPDATE SET name = @Name, current_price = @CurrentPrice
WHEN NOT MATCHED THEN
    INSERT (name, current_price) VALUES (@Name, @CurrentPrice);", conn);

                cmd.Parameters.AddWithValue("@Id", crypto.Id);
                cmd.Parameters.AddWithValue("@Name", crypto.Name);
                cmd.Parameters.AddWithValue("@CurrentPrice", crypto.CurrentPrice);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        // ALERTS
        public async Task<List<Alert>> GetAllAlertsAsync()
        {
            var alerts = new List<Alert>();
            using (var conn = CreateConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT id, user_id, cryptocurrency_id, target_price, notified, over_the_price FROM crypto_pilot.alerts", conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    alerts.Add(new Alert
                    {
                        Id = reader.GetInt32(0),
                        UserId = reader.GetInt32(1),
                        CryptocurrencyId = reader.GetInt32(2),
                        TargetPrice = reader.GetDouble(3),
                        Notified = reader.GetBoolean(4),
                        OverThePrice = reader.GetBoolean(5)
                    });
                }
            }
            return alerts;
        }

        public async Task UpsertAlertAsync(Alert alert)
        {
            using (var conn = CreateConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
MERGE crypto_pilot.alerts AS target
USING (SELECT @Id AS id) AS source
ON (target.id = source.id AND target.id != -1)
WHEN MATCHED THEN 
    UPDATE SET user_id = @UserId, cryptocurrency_id = @CryptocurrencyId, target_price = @TargetPrice, notified = @Notified, over_the_price = @OverThePrice
WHEN NOT MATCHED THEN
    INSERT (user_id, cryptocurrency_id, target_price, notified, over_the_price) VALUES (@UserId, @CryptocurrencyId, @TargetPrice, @Notified, @OverThePrice);", conn);

                cmd.Parameters.AddWithValue("@Id", alert.Id);
                cmd.Parameters.AddWithValue("@UserId", alert.UserId);
                cmd.Parameters.AddWithValue("@CryptocurrencyId", alert.CryptocurrencyId);
                cmd.Parameters.AddWithValue("@TargetPrice", alert.TargetPrice);
                cmd.Parameters.AddWithValue("@Notified", alert.Notified);
                cmd.Parameters.AddWithValue("@OverThePrice", alert.OverThePrice);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
