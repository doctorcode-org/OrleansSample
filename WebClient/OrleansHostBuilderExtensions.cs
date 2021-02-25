using GrainsAbstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Threading.Tasks;

namespace WebClient
{
    public static class OrleansHostBuilderExtensions
    {
        public static IHostBuilder UseOrleansClusterClient(this IHostBuilder builder)
        {
            IClusterClient client = null;

            builder
                .ConfigureServices(async (context, services) =>
                {
                    // TODO get from context.config
                    int remainingAttempts = 3;
                    int retryDelaySeconds = 15;

                    while (remainingAttempts-- > 0 && client is null)
                    {
                        try
                        {
                            client = new ClientBuilder()
                                .Configure<ClusterOptions>(options =>
                                {
                                    options.ClusterId = "local-cluster";
                                    options.ServiceId = "test-silo";
                                })
                                .UseAdoNetClustering(options =>
                                {
                                    options.Invariant = AdoNetGrainStorageOptions.DEFAULT_ADONET_INVARIANT;
                                    options.ConnectionString = "Data Source=localhost;Initial Catalog=Orleans;Integrated Security=True;Pooling=False;Max Pool Size=200;MultipleActiveResultSets=True";
                                })
                                .ConfigureApplicationParts(manager =>
                                {
                                    manager.AddApplicationPart(typeof(IHelloWorldGrain).Assembly);
                                })
                                //.AddSimpleMessageStreamProvider("SMS")
                                .Build();

                            await client.Connect();
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex);

                            client?.Dispose();

                            if (remainingAttempts == 0)
                            {
                                throw;
                            }

                            await Task.Delay(retryDelaySeconds * 1000);
                        }
                    }

                })
                .ConfigureServices(services => services.AddSingleton(client));

            return builder;
        }
    }
}
