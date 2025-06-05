namespace LeaveRequestSample.Hooks.ResusableHooks;

using Meridian.Application.Extensions;
using Meridian.Core;
using Meridian.Core.Enums;
using Meridian.Core.Interfaces;
using Models;
using NormalHooks;

public static class HooksTemplate
{
    public static WorkflowAction<LeaveRequestData> NotifyEmployee(
        this WorkflowAction<LeaveRequestData> action, string source, bool fromState = true)
    
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