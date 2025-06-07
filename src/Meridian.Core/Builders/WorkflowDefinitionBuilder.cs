namespace Meridian.Core.Builders;

using Contexts;
using Delegates;
using Dtos;
using Enums;
using Interfaces;
using Interfaces.DslBuilder;
using Models;

/// <inheritdoc />
internal class WorkflowDefinitionBuilder<TData> : IWorkflowDefinitionBuilder<TData>
    where TData : class, IWorkflowData
{
    /// <summary>
    /// Represents the current workflow definition being built.
    /// </summary>
    /// <remarks>
    /// This variable stores the instance of <see cref="WorkflowDefinition{TData}"/>
    /// being configured using the builder. It holds details about states,
    /// hooks, and other workflow-specific configurations.
    /// </remarks>
    private readonly WorkflowDefinition<TData> _definition;

    /// <summary>
    /// Provides functionality to define and build workflow definitions for a specified workflow data type.
    /// </summary>
    /// <typeparam name="TData">
    /// The type of data used by the workflow. Must implement the <see cref="IWorkflowData"/> interface.
    /// </typeparam>
    private WorkflowDefinitionBuilder(string name)
    {
        this._definition = new WorkflowDefinition<TData>(name);
    }

    /// <summary>
    /// Creates a new workflow definition with the specified name and configuration settings.
    /// </summary>
    /// <typeparam name="TData">
    /// The type of data to be used by the workflow. This type must implement the <see cref="IWorkflowData"/> interface.
    /// </typeparam>
    /// <param name="name">
    /// The name of the workflow definition to be created.
    /// </param>
    /// <param name="config">
    /// An action to configure the workflow definition using an instance of <see cref="IWorkflowDefinitionBuilder{TData}"/>.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="WorkflowDefinition{TData}"/> representing the configured workflow definition.
    /// </returns>
    public static WorkflowDefinition<TData> Create(string name, Action<IWorkflowDefinitionBuilder<TData>> config)
    {
        var builder = new WorkflowDefinitionBuilder<TData>(name);
        config(builder);
        var build = builder.Build();
        build.Validate();
        return build;
    }

    /// <inheritdoc />
    public IWorkflowDefinitionBuilder<TData> AddHook(WorkflowHookDescriptor<TData> descriptor,
        WorkflowHookType hookType = WorkflowHookType.OnRequestCreated)
    {
        switch (hookType)
        {
            case WorkflowHookType.OnRequestCreated:
                this._definition.OnCreateHooks.Add(descriptor);
                break;
            case WorkflowHookType.OnRequestTransition:
                this._definition.OnTransitionHooks.Add(descriptor);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(hookType), hookType, null);
        }

        return this;
    }

    /// <inheritdoc />
    public IWorkflowDefinitionBuilder<TData> AddHook(Func<WorkflowContext<TData>, Task> hook,
        Action<WorkflowHookDescriptor<TData>>? setup = null,
        WorkflowHookType hookType = WorkflowHookType.OnRequestCreated)
    {
        this.AddHook(new DelegateWorkflowHook<TData>(hook), setup, hookType);
        return this;
    }

    /// <inheritdoc />
    public IWorkflowDefinitionBuilder<TData> AddHook(IWorkflowHook<TData> hook,
        Action<WorkflowHookDescriptor<TData>>? setup = null, WorkflowHookType hookType = default)
    {
        var descriptor = new WorkflowHookDescriptor<TData>
        {
            Hook = hook,
        };

        setup?.Invoke(descriptor);

        switch (hookType)
        {
            case WorkflowHookType.OnRequestCreated:
                this._definition.OnCreateHooks.Add(descriptor);
                break;
            case WorkflowHookType.OnRequestTransition:
                this._definition.OnTransitionHooks.Add(descriptor);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(hookType), hookType, null);
        }

        return this;
    }

    /// <inheritdoc />
    public IWorkflowDefinitionBuilder<TData> OverrideOnCreateHistory(WorkflowTransition initialHistory)
    {
        this._definition.OverrideOnCreateHistory(initialHistory);
        return this;
    }

    /// <inheritdoc />
    public IWorkflowDefinitionBuilder<TData> State(string name, Action<IStateBuilder<TData>> stateBuilder)
    {
        var state = new WorkflowState<TData>(name);
        var builder = new StateBuilder<TData>(state);
        stateBuilder(builder);
        this._definition.States.Add(state);
        return this;
    }

    /// <inheritdoc />
    public void PrintToConsole()
    {
    }

    /// <inheritdoc />
    public WorkflowDefinition<TData> Build() => this._definition;
}