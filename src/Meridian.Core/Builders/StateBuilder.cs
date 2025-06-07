namespace Meridian.Core.Builders;

using Contexts;
using Delegates;
using Dtos;
using Enums;
using Interfaces;
using Interfaces.DslBuilder;
using Models;

/// <inheritdoc />
internal class StateBuilder<TData> : IStateBuilder<TData>
    where TData : class, IWorkflowData
{
    /// <summary>
    /// Represents the internal state of the workflow being constructed by the StateBuilder.
    /// This variable is an instance of <see cref="WorkflowState{TData}"/>, which holds configuration
    /// for a specific state in a workflow, including its actions, hooks, and type.
    /// </summary>
    /// <typeparam name="TData">
    /// The type of data that the workflow operates upon. This must implement the <see cref="IWorkflowData"/> interface.
    /// </typeparam>
    private readonly WorkflowState<TData> _state;

    /// <summary>
    /// Class responsible for building and configuring the states within a workflow.
    /// Enables the addition of hooks, state roles, and actions through a fluent API.
    /// </summary>
    /// <typeparam name="TData">The type of workflow data, constrained to implement <see cref="IWorkflowData"/>.</typeparam>
    internal StateBuilder(WorkflowState<TData> state) => this._state = state;

    /// <inheritdoc />
    public IStateBuilder<TData> AddHook(WorkflowHookDescriptor<TData> descriptor,
        StateHookType hookType = StateHookType.OnStateEnter)
    {
        switch (hookType)
        {
            case StateHookType.OnStateEnter:
                this._state.OnEnterHooks.Add(descriptor);
                break;
            case StateHookType.OnStateExit:
                this._state.OnExitHooks.Add(descriptor);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(hookType), hookType, null);
        }

        return this;
    }

    /// <inheritdoc />
    public IStateBuilder<TData> AddHook(Func<WorkflowContext<TData>, Task> hook,
        Action<WorkflowHookDescriptor<TData>>? setup = null, StateHookType hookType = StateHookType.OnStateEnter)
    {
        this.AddHook(new DelegateWorkflowHook<TData>(hook), setup, hookType);
        return this;
    }

    /// <inheritdoc />
    public IStateBuilder<TData> AddHook(IWorkflowHook<TData> hook, Action<WorkflowHookDescriptor<TData>>? setup = null,
        StateHookType hookType = StateHookType.OnStateEnter)
    {
        var descriptor = new WorkflowHookDescriptor<TData>
        {
            Hook = hook,
        };

        setup?.Invoke(descriptor);

        switch (hookType)
        {
            case StateHookType.OnStateEnter:
                this._state.OnEnterHooks.Add(descriptor);
                break;
            case StateHookType.OnStateExit:
                this._state.OnExitHooks.Add(descriptor);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(hookType), hookType, null);
        }

        return this;
    }

    /// <inheritdoc />
    public IStateBuilder<TData> IsStart()
    {
        this._state.WithType(StateType.Start);
        return this;
    }

    /// <inheritdoc />
    public IStateBuilder<TData> IsCompleted()
    {
        this._state.WithType(StateType.Completed);
        return this;
    }

    /// <inheritdoc />
    public IStateBuilder<TData> IsRejected()
    {
        this._state.WithType(StateType.Rejected);
        return this;
    }

    /// <inheritdoc />
    public IStateBuilder<TData> IsCanceled()
    {
        this._state.WithType(StateType.Cancelled);
        return this;
    }

    /// <inheritdoc />
    public IStateBuilder<TData> Action(string name, string nextState,
        Action<IActionBuilder<TData>>? actionBuilder = null)
    {
        if (this._state.Actions.Any(a => a.Name == name))
            throw new ArgumentException($"Action with name '{name}' already exists in state '{this._state.Name}'");

        var action = new WorkflowAction<TData>(name) { NextState = nextState };
        var builder = new ActionBuilder<TData>(action);

        actionBuilder?.Invoke(builder);
        this._state.Actions.Add(action);
        return this;
    }
}