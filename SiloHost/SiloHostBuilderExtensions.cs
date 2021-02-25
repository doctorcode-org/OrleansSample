using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Orleans.Hosting
{
    public static class SiloHostBuilderExtensions
    {
        /// <summary>
        /// Runs an application and block the calling thread until host shutdown.
        /// </summary>
        /// <param name="host">The <see cref="ISiloHost"/> to run.</param>
        public static void Run(this ISiloHost host)
        {
            host.StartAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns a Task that completes when shutdown is triggered via the given token.
        /// </summary>
        /// <param name="host">The running <see cref="IHost"/>.</param>
        /// <param name="token">The token to trigger shutdown.</param>
        public static async Task WaitForShutdownAsync(this ISiloHost host, CancellationToken token = default)
        {
            //var applicationLifetime = host.Services.GetRequiredService<IApplicationLifetime>();

            //token.Register(state =>
            //{
            //    ((IApplicationLifetime)state).StopApplication();
            //},
            //applicationLifetime);

            var waitForStop = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            //applicationLifetime.ApplicationStopping.Register(obj =>
            //{
            //    var tcs = (TaskCompletionSource<object>)obj;
            //    tcs.TrySetResult(null);
            //}, waitForStop);

            await waitForStop.Task;

            // Host will use its default ShutdownTimeout if none is specified.
            await host.StopAsync();
        }
    }
}
