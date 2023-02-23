using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.GrainInterfaces;
using Orleans.Runtime;
using Orleans.Streams;

namespace Orleans.GrainClasses;

public class RobotGrain : Grain, IRobotGrain
{
    private ILogger<RobotGrain> _logger;
    IPersistentState<RobotState> _state;
    string _key;
    IAsyncStream<InstructionMessage> _stream;

    public RobotGrain(ILogger<RobotGrain> logger, [PersistentState("robotState", "robotStateStore")]
        IPersistentState<RobotState> state)
    {
        _logger = logger;
        _state = state;
        _key = this.GetPrimaryKeyString();

        _stream = this
            .GetStreamProvider("SMSProvider")
            .GetStream<InstructionMessage>(
                Guid.Empty);
    }
    public async Task AddInstruction(string instruction)
    {

        _logger.LogWarning($"{_key} adding '{instruction}'");

        _state.State.Instructions.Enqueue(instruction);
        await _state.WriteStateAsync();
    }
    public Task<int> GetInstructionCount()
    {
        return Task.FromResult(_state.State.Instructions.Count);
    }
    public async Task<string> GetNextInstruction()
    {
        if (_state.State.Instructions.Count == 0)
        {
            return null;
        }
        var instruction = _state.State.Instructions.Dequeue();

        _logger.LogWarning($"{_key} returning '{instruction}'");

        await this.Publish(instruction);

        await _state.WriteStateAsync();
        return instruction;
    }

    private Task Publish(string instruction)
    {
        var message = new InstructionMessage(
            instruction, _key);
        return _stream.OnNextAsync(message);
    }
}


#region version 1
//public class RobotGrain : Grain, IRobotGrain
//{
//    private ILogger<RobotGrain> _logger;
//    private Queue<string> instructions = new Queue<string>();


//    public RobotGrain(ILogger<RobotGrain> logger)
//    {
//        _logger = logger;
//    }
//    public Task AddInstruction(string instruction)
//    {
//        var key = this.GetPrimaryKeyString();
//        _logger.LogWarning($"{key} adding '{instruction}'");
//        this.instructions.Enqueue(instruction);
//        return Task.CompletedTask;
//    }
//    public Task<int> GetInstructionCount()
//    {
//        return Task.FromResult(this.instructions.Count);
//    }
//    public Task<string> GetNextInstruction()
//    {
//        if (this.instructions.Count == 0)
//        {
//            return Task.FromResult<string>(null);
//        }
//        var instruction = this.instructions.Dequeue();

//        var key = this.GetPrimaryKeyString();
//        _logger.LogWarning($"{key} adding '{instruction}'");

//        return Task.FromResult(instruction);
//    }
//}


#endregion

