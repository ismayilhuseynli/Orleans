using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.ClientObservers;
using Orleans.Configuration;
using Orleans.GrainInterfaces;



namespace Orleans.Client;

class Program
{
    static async Task Main()
    {

        using IHost host = Host.CreateDefaultBuilder()
            .UseOrleansClient((context, client) =>
            {
                client.Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "cluster";
                    options.ServiceId = "MyOrleansService";
                }).UseLocalhostClustering();

            })
            .UseConsoleLifetime()
            .Build();


        await host.StartAsync();

        IGrainFactory client = host.Services.GetRequiredService<IGrainFactory>();

        while (true)
        {
            Console.WriteLine("Please enter a robot name...");
            var grainId = Console.ReadLine();
            var grain = client.GetGrain<IRobotGrain>(grainId);
            Console.WriteLine("Please enter an instruction...");
            var instruction = Console.ReadLine();
            await grain.AddInstruction(instruction);
            var count = await grain.GetInstructionCount();
            Console.WriteLine($"{grainId} has {count} instruction(s)");
        }

    }
}

