namespace Meridian.Infrastructure.Hooks.WorkflowHooksExtensions;

using Core;
using Core.Dtos;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.DslBuilder;

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
    public static IWorkflowDefinitionBuilder<TData> AddCompareDataAndLogHistory<TData>(
        this IWorkflowDefinitionBuilder<TData> definition)
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