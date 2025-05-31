namespace Meridian.Core.Tests;

using Core.Interfaces;

public class MockWorkflowHook : IWorkflowHook<MockWorkflowData>
{
    public bool WasExecuted { get; private set; }
    public WorkflowContext<MockWorkflowData>? LastContext { get; private set; }

    public Task ExecuteAsync(WorkflowContext<MockWorkflowData> context)
    {
        WasExecuted = true;
        LastContext = context;
        return Task.CompletedTask;
    }
}