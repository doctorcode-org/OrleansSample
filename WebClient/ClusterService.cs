using GrainsAbstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WebClient
{
    public class ClusterService : IHostedService
    {
        private readonly ILogger<ClusterService> _logger;

        public ClusterService(ILogger<ClusterService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            Client = new ClientBuilder()
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
        }

        public IClusterClient Client { get; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Client.Connect(async error =>
            {
                _logger.LogError(error, error.Message);
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                return true;
            });
        }

        public Task StopAsync(CancellationToken cancellationToken) =>
            Client.Close();
    }

    public static class ClusterServiceBuilderExtensions
    {
        public static IServiceCollection AddClusterService(this IServiceCollection services)
        {
            services.TryAddSingleton<ClusterService>();
            services.AddSingleton<IHostedService>(_ => _.GetService<ClusterService>());
            services.AddTransient(_ => _.GetService<ClusterService>().Client);
            return services;
        }
    }
}
