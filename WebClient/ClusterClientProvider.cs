using GrainsAbstractions;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;

namespace WebClient
{
    public class ClusterClientProvider
    {
        private IClusterClient _client;
        private readonly static object _sync = new object();
        private readonly ILogger<ClusterClientProvider> _logger;

        public ClusterClientProvider(ILogger<ClusterClientProvider> logger)
        {
            _logger = logger;
        }

        public IClusterClient Client
        {
            get
            {
                lock (_sync)
                {
                    if (_client is null || !_client.IsInitialized)
                    {
                        try
                        {
                            _client = new ClientBuilder()
                                .Configure<ClusterOptions>(options =>
                                {
                                    options.ClusterId = "local-cluster";
                                    options.ServiceId = "test-silo";
                                })
                                .Configure<GatewayOptions>(options =>
                                {
                                    options.GatewayListRefreshPeriod = TimeSpan.FromSeconds(1);
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

                            _client.Connect().GetAwaiter().GetResult();
                        }
                        catch (Exception ex)
                        {
                            _client?.Dispose();
                            _client = null;
                            _logger.LogError(ex, "Can not provide a cluster client.");

                            throw;
                        }
                    }
                }

                return _client;
            }
        }


    }
}
