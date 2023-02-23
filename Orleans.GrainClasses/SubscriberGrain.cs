using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans.GrainInterfaces;
using Orleans.Streams;

namespace Orleans.GrainClasses;

[ImplicitStreamSubscription("StartingInstruction")]
public class SubscriberGrain : Grain, ISubscriberGrain, IAsyncObserver<InstructionMessage>
{
    public override async Task OnActivateAsync(CancellationToken token)
    {
        var key = this.GetPrimaryKey();

        await this.GetStreamProvider("SMSProvider")
            .GetStream<InstructionMessage>(key)
            .SubscribeAsync(this);

        await base.OnActivateAsync(token);
    }
    public Task OnNextAsync(
        InstructionMessage instruction,
        StreamSequenceToken token = null)
    {
        var msg = $"{instruction.Robot} starting {instruction.Instruction}";
        Console.WriteLine(msg);
        return Task.CompletedTask;
    }
    public Task OnCompletedAsync() =>
        Task.CompletedTask;

    public Task OnErrorAsync(System.Exception ex) =>
        Task.CompletedTask;
}


