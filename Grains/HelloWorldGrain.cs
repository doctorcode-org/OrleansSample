using GrainsAbstractions;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public class HelloWorldGrain : Grain, IHelloWorldGrain
    {
        private readonly ILogger _logger;

        public HelloWorldGrain(ILogger<HelloWorldGrain> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<string> SayHello(string name)
        {
            _logger.LogInformation($"SayHello to: {name}");

            return Task.FromResult("HelloWorldGrain result");
        }

        public override Task OnActivateAsync()
        {
            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            return base.OnDeactivateAsync();
        }
    }
}
