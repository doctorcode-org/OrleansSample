using GrainsAbstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;

namespace WebClient
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddClusterService();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    var client = context.RequestServices.GetRequiredService<IClusterClient>();
                    var hello = client.GetGrain<IHelloWorldGrain>(0);
                    await context.Response.WriteAsync(await hello.SayHello("Alireza"));
                });
            });
        }
    }
}
