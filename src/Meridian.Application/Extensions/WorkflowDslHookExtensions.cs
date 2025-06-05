namespace Meridian.Application.Extensions;

using Core;
using Core.Delegates;
using Core.Enums;
using Core.Interfaces;

public static class WorkflowDslHookExtensions
{
    #region Hooks to be added to the workflow definition

    /// <summary>
    /// Adds a hook to a workflow definition with the specified descriptor and hook type.
    /// </summary>
    /// <typeparam name="TData">
    /// The type of the workflow data, which must implement <see cref="IWorkflowData"/>.
    /// </typeparam>
    /// <param name="def">The workflow definition to which the hook will be added.</param>
    /// <param name="descriptor">The hook descriptor defining the hook's properties and behavior.</param>
    /// <param name="hookType">
    /// Optional. Specifies the type of hook to be added. Default value is <see cref="WorkflowHookType.OnRequestCreated"/>.
    /// </param>
    /// <returns>
    /// The updated workflow definition with the added hook.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the specified hook type is not recognized.
    /// </exception>
    public static WorkflowDefinition<TData> AddHook<TData>(
        this WorkflowDefinition<TData> def,
        WorkflowHookDescriptor<TData> descriptor,
        WorkflowHookType hookType = WorkflowHookType.OnRequestCreated)
        where TData : class, IWorkflowData
    {
        switch (hookType)
        {
            case WorkflowHookType.OnRequestCreated:
                def.OnCreateHooks.Add(descriptor);
                break;
            case WorkflowHookType.OnRequestTransition:
                def.OnTransitionHooks.Add(descriptor);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(hookType), hookType, null);
        }

        return def;
    }

    /// <summary>
    /// Adds a hook to the workflow definition with the specified hook function, optional descriptor setup, and hook type.
    /// </summary>
    /// <typeparam name="TData">
    /// The type of the workflow data, which must implement <see cref="IWorkflowData"/>.
    /// </typeparam>
    /// <param name="def">The workflow definition to which the hook will be added.</param>
    /// <param name="hook">A delegate function to be executed as the workflow hook.</param>
    /// <param name="setup">
    /// Optional. An action to configure the hook descriptor, allowing customization of the hook's properties and behavior.
    /// </param>
    /// <param name="hookType">
    /// Optional. Specifies the type of hook to be added. Default value is <see cref="WorkflowHookType.OnRequestCreated"/>.
    /// </param>
    /// <returns>
    /// The workflow definition with the newly added hook.
    /// </returns>
    public static WorkflowDefinition<TData> AddHook<TData>(this WorkflowDefinition<TData> def,
        Func<WorkflowContext<TData>, Task> hook,
        Action<WorkflowHookDescriptor<TData>>? setup = null,
        WorkflowHookType hookType = WorkflowHookType.OnRequestCreated)
        where TData : class, IWorkflowData
    {
        def.AddHook(new DelegateWorkflowHook<TData>(hook), setup, hookType);
        return def;
    }

    /// <summary>
    /// Adds a workflow hook to the specified workflow definition, allowing customization of its behavior and type.
    /// </summary>
    /// <typeparam name="TData">
    /// The type of the workflow data, constrained to implement <see cref="IWorkflowData"/>.
    /// </typeparam>
    /// <param name="def">The workflow definition to which the hook will be added.</param>
    /// <param name="hook">The workflow hook containing the logic to be executed.</param>
    /// <param name="setup">
    /// Optional. An action to configure and modify the hook descriptor before it is added to the workflow.
    /// </param>
    /// <param name="hookType">
    /// Specifies the type of the hook being added, determining at which stage of the workflow it will execute.
    /// Default value is <see cref="WorkflowHookType.OnRequestCreated"/>.
    /// </param>
    /// <returns>
    /// The updated workflow definition instance that includes the newly added hook.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the specified hook type is unrecognized.
    /// </exception>
    public static WorkflowDefinition<TData> AddHook<TData>(
        this WorkflowDefinition<TData> def,
        IWorkflowHook<TData> hook,
        Action<WorkflowHookDescriptor<TData>>? setup = null,
        WorkflowHookType hookType = WorkflowHookType.OnRequestCreated)
        where TData : class, IWorkflowData
    {
        var descriptor = new WorkflowHookDescriptor<TData>
        {
            Hook = hook,
        };

        setup?.Invoke(descriptor);

        switch (hookType)
        {
            case WorkflowHookType.OnRequestCreated:
                def.OnCreateHooks.Add(descriptor);
                break;
            case WorkflowHookType.OnRequestTransition:
                def.OnTransitionHooks.Add(descriptor);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(hookType), hookType, null);
        }

        return def;
    }

    #endregion

    #region Hooks to be added to the states definition

    /// <summary>
    /// Adds a workflow hook to the specified workflow state. The hook will be triggered based on the provided hook type (OnStateEnter or OnStateExit).
    /// </summary>
    /// <typeparam name="TData">The type of workflow data associated with the workflow state.</typeparam>
    /// <param name="state">The workflow state to which the hook will be added.</param>
    /// <param name="descriptor">The descriptor of the hook specifying its behavior, execution mode, and configuration.</param>
    /// <param name="hookType">The type of state hook to determine when the hook should be executed (e.g., OnStateEnter, OnStateExit).</param>
    /// <returns>The updated workflow state with the added hook.</returns>
    public static WorkflowState<TData> AddHook<TData>(
        this WorkflowState<TData> state,
        WorkflowHookDescriptor<TData> descriptor,
        StateHookType hookType)
        where TData : class, IWorkflowData
    {
        switch (hookType)
        {
            case StateHookType.OnStateEnter:
                state.OnEnterHooks.Add(descriptor);
                break;
            case StateHookType.OnStateExit:
                state.OnExitHooks.Add(descriptor);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(hookType), hookType, null);
        }

        return state;
    }

    /// <summary>
    /// Adds a hook to the workflow state with the specified hook function, optional descriptor setup, and hook type.
    /// </summary>
    /// <typeparam name="TData">
    /// The type of the workflow data, which must implement <see cref="IWorkflowData"/>.
    /// </typeparam>
    /// <param name="state">The workflow state to which the hook will be added.</param>
    /// <param name="hook">A delegate function to be executed as the workflow hook.</param>
    /// <param name="setup">
    /// Optional. An action to configure the hook descriptor, allowing customization of the hook's properties and behavior.
    /// </param>
    /// <param name="hookType">
    /// Specifies the type of state hook, determining at which stage it will execute (OnStateEnter or OnStateExit).
    /// </param>
    /// <returns>
    /// The workflow state with the newly added hook.
    /// </returns>
    public static WorkflowState<TData> AddHook<TData>(
        this WorkflowState<TData> state,
        Func<WorkflowContext<TData>, Task> hook,
        Action<WorkflowHookDescriptor<TData>>? setup = null,
        StateHookType hookType = StateHookType.OnStateEnter)
        where TData : class, IWorkflowData
    {
        state.AddHook(new DelegateWorkflowHook<TData>(hook), setup, hookType);
        return state;
    }

    /// <summary>
    /// Adds a workflow hook to the specified workflow state, allowing customization of its behavior and type.
    /// </summary>
    /// <typeparam name="TData">
    /// The type of the workflow data, constrained to implement <see cref="IWorkflowData"/>.
    /// </typeparam>
    /// <param name="state">The workflow state to which the hook will be added.</param>
    /// <param name="hook">The workflow hook containing the logic to be executed.</param>
    /// <param name="setup">
    /// Optional. An action to configure and modify the hook descriptor before it is added to the workflow state.
    /// </param>
    /// <param name="hookType">
    /// Specifies the type of the state hook, determining at which stage it will execute (OnStateEnter or OnStateExit).
    /// </param>
    /// <returns>
    /// The updated workflow state instance that includes the newly added hook.
    /// </returns>
    public static WorkflowState<TData> AddHook<TData>(
        this WorkflowState<TData> state,
        IWorkflowHook<TData> hook,
        Action<WorkflowHookDescriptor<TData>>? setup = null,
        StateHookType hookType = StateHookType.OnStateEnter)
        where TData : class, IWorkflowData
    {
        var descriptor = new WorkflowHookDescriptor<TData>
        {
            Hook = hook,
        };

        setup?.Invoke(descriptor);

        switch (hookType)
        {
            case StateHookType.OnStateEnter:
                state.OnEnterHooks.Add(descriptor);
                break;
            case StateHookType.OnStateExit:
                state.OnExitHooks.Add(descriptor);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(hookType), hookType, null);
        }

        return state;
    }

    #endregion

    #region Hooks to be added to the actions definition

    /// Adds a hook to execute during the workflow action's execution phase.
    /// <param name="action">The workflow action to which the hook will be added.</param>
    /// <param name="descriptor">The descriptor that contains the hook definition and execution details.</param>
    /// <typeparam name="TData">The type of the workflow data associated with the action.</typeparam>
    /// <returns>The updated workflow action with the added hook.</returns>
    public static WorkflowAction<TData> AddHook<TData>(
        this WorkflowAction<TData> action,
        WorkflowHookDescriptor<TData> descriptor)
        where TData : class, IWorkflowData
    {
        action.OnExecuteHooks.Add(descriptor);
        return action;
    }

    /// <summary>
    /// Adds a hook to the workflow action with the specified hook function, optional descriptor setup, and hook type.
    /// </summary>
    /// <typeparam name="TData">
    /// The type of the workflow data, which must implement <see cref="IWorkflowData"/>.
    /// </typeparam>
    /// <param name="action">The workflow action to which the hook will be added.</param>
    /// <param name="hook">A delegate function to be executed as the workflow hook.</param>
    /// <param name="setup">
    /// Optional. An action to configure the hook descriptor, allowing customization of the hook's properties and behavior.
    /// </param>
    /// <returns>
    /// The workflow action with the newly added hook.
    /// </returns>
    public static WorkflowAction<TData> AddHook<TData>(
        this WorkflowAction<TData> action,
        Func<WorkflowContext<TData>, Task> hook,
        Action<WorkflowHookDescriptor<TData>>? setup = null)
        where TData : class, IWorkflowData
    {
        action.AddHook(new DelegateWorkflowHook<TData>(hook), setup);
        return action;
    }

    /// <summary>
    /// Adds a workflow hook to the specified workflow action, allowing customization of its behavior and type.
    /// </summary>
    /// <typeparam name="TData">
    /// The type of the workflow data, constrained to implement <see cref="IWorkflowData"/>.
    /// </typeparam>
    /// <param name="action">The workflow action to which the hook will be added.</param>
    /// <param name="hook">The workflow hook containing the logic to be executed.</param>
    /// <param name="setup">
    /// Optional. An action to configure and modify the hook descriptor before it is added to the workflow action.
    /// </param>
    /// <returns>
    /// The updated workflow action instance that includes the newly added hook.
    /// </returns>
    public static WorkflowAction<TData> AddHook<TData>(
        this WorkflowAction<TData> action,
        IWorkflowHook<TData> hook,
        Action<WorkflowHookDescriptor<TData>>? setup = null)
        where TData : class, IWorkflowData
    {
        var descriptor = new WorkflowHookDescriptor<TData>
        {
            Hook = hook,
        };

        setup?.Invoke(descriptor);

        action.OnExecuteHooks.Add(descriptor);

        return action;
    }

    #endregion
}