namespace LeaveRequestSample.Hooks.ResusableHooks;

using Meridian.Core.Enums;
using Meridian.Core.Interfaces.DslBuilder;
using Models;
using NormalHooks;

public static class HooksTemplate
{
    public static IActionBuilder<LeaveRequestData> NotifyEmployee(
        this IActionBuilder<LeaveRequestData> action, string source, bool fromState = true)
    
    {
        action.AddHook(
            new SendNotification(source, fromState), 
            config =>
        {
            config.Mode = HookExecutionMode.Parallel;
            config.IsAsync = true;
            config.LogExecutionHistory = false;
        });
        return action;
    }
}