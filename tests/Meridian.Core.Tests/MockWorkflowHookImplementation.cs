namespace Meridian.Core.Tests;

using Contexts;
using Interfaces;

public class MockWorkflowHook : IWorkflowHook<MockWorkflowData>
{
    public bool WasExecuted { get; private set; }
    public WorkflowContext<MockWorkflowData>? LastContext { get; private set; }

    public Task ExecuteAsync(WorkflowContext<MockWorkflowData> context)
    {
        this.WasExecuted = true;
        this.LastContext = context;
        return Task.CompletedTask;
    }
}