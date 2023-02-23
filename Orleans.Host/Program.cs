using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.GrainClasses;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Serialization;

namespace Orleans.Host;

class Program
{
    static async Task Main()
    {
        var host = new HostBuilder()
            .UseOrleans(builder => {
                builder.Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "cluster";
                    options.ServiceId = "MyOrleansService";
                });
                builder.ConfigureLogging(log =>
                {
                    log.AddConsole();
                    log.SetMinimumLevel(LogLevel.Warning);
                });
                builder.UseDashboard();
                builder.AddMemoryGrainStorage("robotStateStore");
                builder.AddMemoryGrainStorage("PubSubStore");
                builder.AddMemoryStreams("SMSProvider");
                builder.UseLocalhostClustering();
            })
            .Build();
        await host.StartAsync();
        Console.WriteLine("Press enter to stop the Silo...");
        Console.ReadLine();
        await host.StopAsync();
    }
}