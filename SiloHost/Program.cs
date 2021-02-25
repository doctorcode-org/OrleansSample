using Grains;
using GrainsAbstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.ApplicationParts;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Statistics;
using System;
using System.Net;

namespace SiloHost
{
    //https://dotnet.github.io/orleans/docs/host/configuration_guide/configuring_ADO.NET_providers.html
    //https://dotnet.github.io/orleans/docs/host/configuration_guide/adonet_configuration.html

    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var connection = "Data Source=localhost;Initial Catalog=Orleans;Integrated Security=True;Pooling=False;Max Pool Size=200;MultipleActiveResultSets=True";

                var siloBuilder = new SiloHostBuilder()
                    .Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "local-cluster";
                        options.ServiceId = "test-silo";
                    })
                    .Configure<EndpointOptions>(options =>
                    {
                        options.AdvertisedIPAddress = IPAddress.Loopback;
                        options.SiloListeningEndpoint = new IPEndPoint(IPAddress.Loopback, 1515);
                        options.GatewayListeningEndpoint = new IPEndPoint(IPAddress.Loopback, 30000);
                    })
                    .UsePerfCounterEnvironmentStatistics()
                    .UseDashboard(options =>
                    {

                    })
                    .UseAdoNetClustering(options =>
                    {
                        options.ConnectionString = connection;
                        options.Invariant = AdoNetGrainStorageOptions.DEFAULT_ADONET_INVARIANT;
                    })
                    .UseAdoNetReminderService(options =>
                    {
                        options.ConnectionString = connection;
                        options.Invariant = AdoNetGrainStorageOptions.DEFAULT_ADONET_INVARIANT;
                    })
                    .AddAdoNetGrainStorageAsDefault(options =>
                    {
                        options.ConnectionString = connection;
                        options.Invariant = AdoNetGrainStorageOptions.DEFAULT_ADONET_INVARIANT;
                    })
                    .ConfigureLogging(builder =>
                    {
                        builder.SetMinimumLevel(LogLevel.Warning);
                        builder.AddConsole();
                    })
                    .ConfigureServices(services =>
                    {
                        services.TryAddSingleton<IHostEnvironmentStatistics, CrossPlatformHostEnvironmentStatistics>();
                    })
                    .ConfigureApplicationParts(parts =>
                    {
                        parts.AddApplicationPart(new AssemblyPart(typeof(HelloWorldGrain).Assembly));
                        parts.AddApplicationPart(new AssemblyPart(typeof(IHelloWorldGrain).Assembly));
                    });

                using (var host = siloBuilder.Build())
                {
                    host.StartAsync().GetAwaiter().GetResult();
                    Console.ReadLine();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }
    }
}
