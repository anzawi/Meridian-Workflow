namespace Meridian.Infrastructure.Hooks.WorkflowHooksExtensions;

using Application.Extensions;
using Core;
using Core.Enums;
using Core.Interfaces;

/// <summary>
/// Provides extension methods to enhance the functionality of workflow definitions
/// by adding specific hooks for workflows.
/// </summary>
public static class WorkflowHookExtensions
{
    /// <summary>
    /// Adds a hook to compare workflow data and log its execution to the workflow definition.
    /// </summary>
    /// <typeparam name="TData">The type of the workflow data implementing the <see cref="IWorkflowData"/> interface.</typeparam>
    /// <param name="definition">The workflow definition to which the hook is added.</param>
    /// <returns>The updated workflow definition.</returns>
    public static WorkflowDefinition<TData> AddCompareDataAndLogHistory<TData>(
        this WorkflowDefinition<TData> definition)
        where TData : class, IWorkflowData
    {

        definition.AddHook(new WorkflowHookDescriptor<TData>
        {
            Hook = new CompareDataAndLogHook<TData>(),
            Mode = HookExecutionMode.Parallel,
            IsAsync = false,
            LogExecutionHistory = false,
        }, WorkflowHookType.OnRequestTransition);
        return definition;
    }
}