namespace LeaveRequestSample.Hooks.NormalHooks;

using Meridian.Core;
using Meridian.Core.Contexts;
using Meridian.Core.Interfaces;
using Models;

public class SendNotification(string source, bool fromState = true) : IWorkflowHook<LeaveRequestData>
{
    public async Task ExecuteAsync(WorkflowContext<LeaveRequestData> context)
    {
        // here you can do something with the workflow context
        Console.WriteLine($"Send Notification to {context.InputData?.EmployeeName} based on {source}");
        await Task.CompletedTask;
    }
}