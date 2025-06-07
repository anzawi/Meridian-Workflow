namespace Meridian.Core.Builders;

using AuthBuilder;
using Contexts;
using Delegates;
using Dtos;
using Enums;
using Interfaces;
using Interfaces.AuthBuilder;
using Interfaces.DslBuilder;
using Models;

/// <inheritdoc />
internal class ActionBuilder<TData> : IActionBuilder<TData>
    where TData : class, IWorkflowData
{
    /// <summary>
    /// Represents the workflow action associated with the current instance of <see cref="ActionBuilder{TData}"/>.
    /// Encapsulates details and behaviors such as hooks, conditions, assignments, and auto-execution flags
    /// for a specific workflow action within a workflow context.
    /// </summary>
    private readonly WorkflowAction<TData> _action;

    /// <summary>
    /// Provides functionality to configure workflow actions for a specific state within a workflow.
    /// </summary>
    /// <typeparam name="TData">The type of data used within the workflow that implements <see cref="IWorkflowData"/>.</typeparam>
    internal ActionBuilder(WorkflowAction<TData> action) => this._action = action;

    /// <inheritdoc />
    public IActionBuilder<TData> AddHook(WorkflowHookDescriptor<TData> descriptor, ActionHookType hookType)
    {
        this._action.OnExecuteHooks.Add(descriptor);
        return this;
    }

    /// <inheritdoc />
    public IActionBuilder<TData> AddHook(Func<WorkflowContext<TData>, Task> hook,
        Action<WorkflowHookDescriptor<TData>>? setup = null, ActionHookType hookType = default)
    {
        this.AddHook(new DelegateWorkflowHook<TData>(hook), setup);
        return this;
    }

    /// <inheritdoc />
    public IActionBuilder<TData> AddHook(IWorkflowHook<TData> hook, Action<WorkflowHookDescriptor<TData>>? setup = null,
        ActionHookType hookType = default)
    {
        var descriptor = new WorkflowHookDescriptor<TData>
        {
            Hook = hook,
        };

        setup?.Invoke(descriptor);

        this._action.OnExecuteHooks.Add(descriptor);

        return this;
    }

    /// <inheritdoc />
    public IActionBuilder<TData> IsAuto()
    {
        this._action.IsAuto = true;
        return this;
    }

    /// <inheritdoc />
    public IActionBuilder<TData> WithCondition(Func<TData, bool> condition)
    {
        this._action.Condition = condition;
        return this;
    }

    /// <inheritdoc />
    public IActionBuilder<TData> WithValidation(Func<TData, List<string>> validation)
    {
        this._action.ValidateInput = validation;
        return this;
    }

    /// <inheritdoc />
    public IActionBuilder<TData> WithoutAutoValidation()
    {
        this._action.UseAutomaticValidation = false;
        return this;
    }

    /// <inheritdoc />
    public IActionBuilder<TData> AssignToUsers(params string[] users)
    {
        this._action.AssignedUsers.AddRange(users.Distinct());
        return this;
    }

    /// <inheritdoc />
    public IActionBuilder<TData> AssignToRoles(params string[] roles)
    {
        this._action.AssignedRoles.AddRange(roles.Distinct());
        return this;
    }

    /// <inheritdoc />
    public IActionBuilder<TData> AssignToGroups(params string[] groups)
    {
        this._action.AssignedGroups.AddRange(groups.Distinct());
        return this;
    }

    /// <inheritdoc />
    public IActionBuilder<TData> AssignTo(Action<IAssignmentRuleBuilder> ruleBuilder)
    {
        var builder = new AssignmentRuleBuilder();
        ruleBuilder(builder);
        this._action.AssignTo(builder.Build());
        return this;
    }

    /// <inheritdoc />
    public IActionBuilder<TData> When(Func<TData, bool> condition, string targetState)
    {
        this._action.When(condition, targetState);

        return this;
    }

    /// <inheritdoc />
    public IActionBuilder<TData> TransitionTo(params (Func<TData, bool> When, string Target)[] transitionRules)
    {
        this._action.TransitionTo(transitionRules);
        return this;
    }

    /// <inheritdoc />
    public IActionBuilder<TData> TransitionTo(
        params (Func<TData, bool> When, string Target, string Label)[] transitionRules)
    {
        this._action.TransitionTo(transitionRules);
        return this;
    }
}