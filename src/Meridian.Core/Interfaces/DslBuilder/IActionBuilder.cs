namespace Meridian.Core.Interfaces.DslBuilder;

using AuthBuilder;
using Enums;
using Hooks;

/// <summary>
/// Represents a builder interface for defining actions in a DSL workflow.
/// This interface is responsible for configuring various aspects of an action,
/// including its conditions, transitions, validation, and assignment rules.
/// </summary>
/// <typeparam name="TData">The type of workflow data being used, which must implement <see cref="IWorkflowData"/>.</typeparam>
public interface IActionBuilder<TData> : IHookBuilder<IActionBuilder<TData>, TData, ActionHookType>
    where TData : class, IWorkflowData
{
    /// <summary>
    /// Marks the current action as automatic. This typically means the action will
    /// be executed without requiring manual intervention.
    /// </summary>
    /// <returns>The current instance of <see cref="IActionBuilder{TData}"/> to allow method chaining.</returns>
    IActionBuilder<TData> IsAuto();

    /// <summary>
    /// Specifies a condition that must be met for the action to be performed.
    /// </summary>
    /// <param name="condition">A function that defines the condition to evaluate.
    /// The function takes an instance of <typeparamref name="TData"/> and returns a boolean indicating whether the condition is met.</param>
    /// <returns>The current instance of <see cref="IActionBuilder{TData}"/> for method chaining.</returns>
    IActionBuilder<TData> WithCondition(Func<TData, bool> condition);

    /// <summary>
    /// Adds a validation step for the action being built. The specified validation function
    /// is executed to validate the data during the action's execution.
    /// </summary>
    /// <param name="validation">A function that validates the provided data and returns a list of string errors if validation fails.</param>
    /// <returns>Returns the action builder for further configuration.</returns>
    IActionBuilder<TData> WithValidation(Func<TData, List<string>> validation);

    /// Disables automatic validation for the current action.
    /// <returns>Returns the current instance of IActionBuilder for chaining additional methods.</returns>
    IActionBuilder<TData> WithoutAutoValidation();

    /// <summary>
    /// Assigns the action to the specified users.
    /// </summary>
    /// <param name="users">An array of user identifiers to assign the action to.</param>
    /// <returns>The current instance of <see cref="IActionBuilder{TData}"/> for method chaining.</returns>
    IActionBuilder<TData> AssignToUsers(params string[] users);

    /// <summary>
    /// Assigns the specified roles to the action, allowing those roles to interact with or execute the action.
    /// </summary>
    /// <param name="roles">An array of role names to be assigned to the action.</param>
    /// <returns>The current instance of <see cref="IActionBuilder{TData}"/> for method chaining.</returns>
    IActionBuilder<TData> AssignToRoles(params string[] roles);

    /// <summary>
    /// Assigns the specified groups to the action, allowing the action to target specific groups.
    /// </summary>
    /// <param name="groups">An array of strings representing group identifiers to be assigned.</param>
    /// <returns>An instance of <see cref="IActionBuilder{TData}"/> to allow method chaining.</returns>
    IActionBuilder<TData> AssignToGroups(params string[] groups);

    /// <summary>
    /// Assigns the current action using a set of assignment rules.
    /// Allows defining complex rules based on roles, groups, and users, or using logical conditions.
    /// </summary>
    /// <param name="ruleBuilder">
    /// A delegate that defines the assignment rules using the <see cref="IAssignmentRuleBuilder"/> interface.
    /// </param>
    /// <returns>
    /// Returns the modified action builder instance to allow method chaining.
    /// </returns>
    IActionBuilder<TData> AssignTo(Action<IAssignmentRuleBuilder> ruleBuilder);

    /// Specifies a condition and target state for a workflow transition.
    /// <param name="condition">
    /// A function that takes an instance of <typeparamref name="TData"/> and evaluates to a boolean.
    /// This condition determines whether the workflow transitions to the specified target state.
    /// </param>
    /// <param name="targetState">
    /// The target state to transition to if the condition is met.
    /// </param>
    /// <returns>
    /// The current instance of <see cref="IActionBuilder{TData}"/> to allow method chaining.
    /// </returns>
    IActionBuilder<TData> When(Func<TData, bool> condition, string targetState);

    /// <summary>
    /// Specifies the transition rules for the workflow. Each rule includes a condition and a target state.
    /// </summary>
    /// <param name="transitionRules">
    /// An array of tuples where each tuple contains:
    /// - <c>When</c>: A condition that determines whether the transition is applicable.
    /// - <c>Target</c>: The state to transition to if the condition is satisfied.
    /// </param>
    /// <returns>
    /// Returns the current instance of <see cref="IActionBuilder{TData}"/> for method chaining.
    /// </returns>
    IActionBuilder<TData> TransitionTo(
        params (Func<TData, bool> When, string Target)[] transitionRules);

    /// <summary>
    /// Defines state transition rules for the workflow by specifying conditions, target states, and optional labels.
    /// </summary>
    /// <param name="transitionRules">
    /// A collection of tuples where each tuple contains:
    /// - A condition function (<see cref="Func{TData, bool}"/> When) defining when the transition is valid.
    /// - A string (Target) specifying the target state to transition to.
    /// - A string (Label) representing an optional label for the transition.
    /// </param>
    /// <returns>
    /// An instance of <see cref="IActionBuilder{TData}"/> for further configuration.
    /// </returns>
    IActionBuilder<TData> TransitionTo(
        params (Func<TData, bool> When, string Target, string Label)[] transitionRules);
}