namespace Meridian.Core.Interfaces.DslBuilder.Hooks;

using Contexts;
using Dtos;

/// <summary>
/// Represents a generic builder interface for configuring and registering hooks
/// within a workflow component.
/// </summary>
/// <typeparam name="TBuilderType">
/// The specific type of the builder interface that is returned for method chaining.
/// </typeparam>
/// <typeparam name="TData">
/// The type of data or context associated with the workflow, implementing <see cref="IWorkflowData"/>.
/// </typeparam>
/// <typeparam name="THookType">
/// The type of hook being added, defining the classification or context of the hook.
/// </typeparam>
public interface IHookBuilder<TBuilderType, TData, in THookType>
    where TData : class, IWorkflowData
{
    /// <summary>
    /// Adds a hook to the workflow or state definition with the specified descriptor and hook type.
    /// </summary>
    /// <typeparam name="TBuilderType">The type of the builder being returned (e.g., IActionBuilder, IStateBuilder, etc.).</typeparam>
    /// <typeparam name="TData">The type of the workflow data associated with the hook.</typeparam>
    /// <typeparam name="THookType">The type of the hook being added (e.g., ActionHookType, StateHookType, WorkflowHookType).</typeparam>
    /// <param name="descriptor">The descriptor that provides details about the hook to be added.</param>
    /// <param name="hookType">The type of the hook that determines where and how it will be executed within the workflow.</param>
    /// <returns>The current builder instance to support method chaining.</returns>
    HookBuilder<TBuilderType, TData> AddHook(WorkflowHookDescriptor<TData> descriptor,
        THookType hookType);

    /// <summary>
    /// Adds a hook to the workflow process. Hooks are used to introduce custom logic at specific points in the workflow lifecycle.
    /// </summary>
    /// <typeparam name="TBuilderType">The type of the builder implementing the method.</typeparam>
    /// <typeparam name="TData">The type of the workflow data, which must implement <see cref="IWorkflowData"/>.</typeparam>
    /// <typeparam name="THookType">The type of the hook being added.</typeparam>
    /// <param name="hook">The Hook function.</param>
    /// <param name="setup">The descriptor containing information about the hook being added.</param>
    /// <param name="hookType">The type of the hook, indicating its purpose or the phase in which it will be executed.</param>
    /// <returns>The builder instance, allowing for chained method calls.</returns>
    HookBuilder<TBuilderType, TData> AddHook(Func<WorkflowContext<TData>, Task> hook,
        Action<WorkflowHookDescriptor<TData>>? setup = null,
        THookType hookType = default!);

    /// <summary>
    /// Adds a workflow hook to the builder with optional configuration and hook type.
    /// </summary>
    /// <typeparam name="TBuilderType">The type of the builder implementing the hook addition.</typeparam>
    /// <typeparam name="TData">The type of the workflow data.</typeparam>
    /// <typeparam name="THookType">The type of the hook.</typeparam>
    /// <param name="hook">The workflow hook to be added.</param>
    /// <param name="setup">An optional configuration action for the workflow hook descriptor.</param>
    /// <param name="hookType">An optional hook type specifying the context for the hook.</param>
    /// <returns>The builder instance of type <typeparamref name="TBuilderType"/>.</returns>
    HookBuilder<TBuilderType, TData> AddHook(IWorkflowHook<TData> hook,
        Action<WorkflowHookDescriptor<TData>>? setup = null,
        THookType hookType = default!);
}