namespace LeaveRequestSample.Hooks.NormalHooks;

using Meridian.Core;
using Meridian.Core.Contexts;
using Meridian.Core.Interfaces;
using Models;

public class NewLeaveRequestCreated : IWorkflowHook<LeaveRequestData>
{
    public async Task ExecuteAsync(WorkflowContext<LeaveRequestData> context)
    {
        // here you can do something with the workflow context
        Console.WriteLine($"New leave request created, {context.InputData?.EmployeeName}");
        await Task.CompletedTask;
    }
}