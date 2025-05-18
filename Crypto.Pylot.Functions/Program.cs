using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Crypto.Pylot.Functions.Models.Options;  
using Crypto.Pylot.Functions.Helpers;  

namespace Crypto.Pylot.Functions;

internal class Program
{
    static void Main(string[] args)
    {

        var host = new HostBuilder()
            .ConfigureFunctionsWebApplication()
            .ConfigureServices(services =>
            {

                services.AddHttpClient();
                services.AddApplicationInsightsTelemetryWorkerService();
                services.ConfigureFunctionsApplicationInsights();
                    
                services.AddOptions<CoinGeckoOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("CoinGeckoOptions").Bind(settings);
                });

                services.AddOptions<CryptoPilotDatabaseOptions>()
                    .Configure<IConfiguration>((settings, configuration) =>
                    {
                        configuration.Bind("CryptoPilotDatabase", settings);
                    });

                services.AddSingleton<DatabaseHelper>();

                services.AddLogging();
            })
            .Build();

        host.Run();
    }
}

