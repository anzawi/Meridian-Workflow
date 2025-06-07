namespace Meridian.Core.Models;

using Contexts;
using Dtos;
using Extensions;
using Interfaces;
using Interfaces.AuthBuilder;

/// <summary>
/// Represents a single action within a workflow, including its properties, conditions,
/// and validation mechanisms. This class is generic and works with workflow data of type <typeparamref name="TData"/>.
/// </summary>
/// <typeparam name="TData">The type of workflow data associated with the action, which must implement the <see cref="IWorkflowData"/> interface.</typeparam>
public class WorkflowAction<TData>(string name) where TData : class, IWorkflowData
{
    /// <summary>
    /// Holds the conditions and their corresponding target states for determining
    /// state transitions within a workflow action. Each entry in the list is a
    /// tuple, where the first element is a function defining the transition condition
    /// based on the workflow data, and the second element specifies the target state
    /// reached when the condition evaluates to true.
    /// Read the Docs for <see cref="When"/> For more details and future versions plan.
    /// </summary>
    private readonly List<(Func<TData, bool> Condition, string TargetState)> _transitionConditions = [];

    /// <summary>
    /// Stores a collection of transition rules defining the conditions and target states
    /// for transitioning from one state to another within a workflow action. Each rule
    /// is represented as a <see cref="TransitionRule{TData}"/> object that specifies a
    /// condition to evaluate and the target state to transition to when the condition is met.
    /// </summary>
    private readonly List<TransitionRule<TData>> _transitionRules = [];

    /// <summary>
    /// Represents the rule that determines how users are assigned to a workflow action.
    /// This property holds an implementation of the <see cref="IAssignmentRule"/> interface,
    /// which encapsulates the logic for authorizing user assignments within the workflow.
    /// This rule can be configured to assign specific users, roles, or groups
    /// dynamically or statically based on the business requirements.
    /// </summary>
    private IAssignmentRule? AssignmentRule { get; set; }

    /// Determines the next workflow state based on the provided data and defined transition rules or conditions.
    /// <param name="data">The workflow data used to evaluate conditions and transition rules. Must not be null.</param>
    /// <returns>The name of the next state if a matching condition or rule is satisfied; otherwise, the default next state.</returns>
    internal string ResolveNextState(TData data)
    {
        foreach (var rule in this._transitionRules.Where(rule => rule.Condition(data)))
        {
            return rule.TargetState;
        }

        foreach (var (condition, targetState) in this._transitionConditions)
        {
            if (condition(data))
                return targetState;
        }

        return this.NextState;
    }

    /// <summary>
    /// Gets or sets the name of the workflow action.
    /// This property represents the human-readable identifier of the action.
    /// </summary>
    public string Name { get; private set; } = name;

    /// <summary>
    /// Gets the PascalCase representation of the <see cref="Name"/> property value.
    /// The conversion is performed using the `ToPascalCase` extension method.
    /// </summary>
    /// <value>
    /// A string containing the PascalCase representation of the <see cref="Name"/> property.
    /// If the <see cref="Name"/> is an empty string, the returned value is also an empty string.
    /// </value>
    public string Code => this.Name.ToPascalCase();

    /// <summary>
    /// Represents the next state to transition to when the workflow action is executed.
    /// </summary>
    internal string NextState { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the workflow action is automatic.
    /// </summary>
    /// <remarks>
    /// When this property is set to true, the action is considered an auto-action and will be executed automatically,
    /// provided its associated condition evaluates to true. If this property is true and no condition is defined,
    /// an exception will be thrown during workflow validation. Conversely, if this property is false and a condition
    /// is defined, an exception will similarly occur. This ensures consistency between the auto-action behavior and its conditions.
    /// </remarks>
    internal bool IsAuto { get; set; } = false;

    /// <summary>
    /// A list of user identifiers that are assigned to perform or interact with the workflow action.
    /// </summary>
    /// <remarks>
    /// The <c>AssignedUsers</c> property contains user IDs as strings. This property can be used to specify
    /// individual users who have permission to execute or manage the workflow action. When checking
    /// authorization, the list of assigned users is one of the criteria considered for access control.
    /// </remarks>
    public List<string> AssignedUsers { get; private set; } = [];

    /// <summary>
    /// Gets or sets the list of roles assigned to the workflow action.
    /// These roles determine which user roles are authorized to execute the action within the workflow.
    /// </summary>
    public List<string> AssignedRoles { get; private set; } = [];

    /// <summary>
    /// Gets or sets the list of group identifiers assigned to the workflow action.
    /// </summary>
    /// <remarks>
    /// This property defines specific groups that are responsible for or may interact with
    /// the workflow action. It allows assignment of tasks or responsibilities to user groups.
    /// The list can be modified to add or remove groups dynamically during workflow configuration.
    /// </remarks>
    public List<string> AssignedGroups { get; private set; } = [];

    /// <summary>
    /// A collection of hook descriptors that are executed when the associated action is performed.
    /// </summary>
    /// <remarks>
    /// The <c>OnExecuteHooks</c> property maintains a list of <see cref="WorkflowHookDescriptor{TData}"/> objects
    /// which define custom behaviors or logic to be executed during the workflow action's execution phase.
    /// Hooks in this collection enable customizable and extensible workflow processing.
    /// </remarks>
    public List<WorkflowHookDescriptor<TData>> OnExecuteHooks { get; internal set; } = [];

    /// <summary>
    /// Gets or sets a condition that determines whether a specific workflow action
    /// can be executed. The condition is a delegate function that evaluates a set
    /// of logic based on the workflow data provided at runtime.
    /// </summary>
    /// <remarks>
    /// - The condition is optional and can be null.
    /// - If the action is marked as automatic (IsAuto = true), the condition must be defined.
    /// - If the action is not automatic (IsAuto = false), the condition must be null.
    /// - The result of the condition is expected to be a boolean, where `true` indicates
    /// that the action is valid for execution, and `false` indicates otherwise.
    /// - The condition is invoked with the specific workflow data instance as input.
    /// </remarks>
    /// <typeparam name="TData">The type of the workflow data that implements <see cref="IWorkflowData"/>.</typeparam>
    public Func<TData, bool>? Condition { get; internal set; }

    /// <summary>
    /// A function delegate used to define custom validation logic for a workflow action.
    /// </summary>
    /// <remarks>
    /// This property allows you to specify a custom validation function that takes a workflow data object as input
    /// and returns a list of error messages if any validation rules are violated.
    /// </remarks>
    /// <typeparam name="TData">
    /// Represents the type of the workflow data object that implements the <see cref="IWorkflowData"/> interface.
    /// </typeparam>
    /// <returns>
    /// A list of strings representing validation error messages. If no errors are found, the list should be empty.
    /// </returns>
    internal Func<TData, List<string>>? ValidateInput { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether automatic validation of input should be performed
    /// for the current workflow action. When set to <c>true</c>, the system will leverage default
    /// validation logic to check the inputs for the action. This property allows an alternative
    /// to implementing custom validation logic in the <see cref="ValidateInput"/> delegate.
    /// </summary>
    internal bool UseAutomaticValidation { get; set; } = true;

    /// Determines if a user is authorized to execute the current workflow action based on their user ID, roles, and groups.
    /// <param name="userId">The ID of the user attempting to perform the action. Can be null.</param>
    /// <param name="userRoles">A list of roles assigned to the user. Can be null.</param>
    /// <param name="userGroups">A list of groups assigned to the user. Can be null.</param>
    /// <returns>True if the user is authorized based on the assigned users, roles, or groups; otherwise, false.</returns>
    internal bool IsAuthorized(string? userId, List<string>? userRoles, List<string>? userGroups)
    {
        var listBased =
            (userId != null && this.AssignedUsers.Contains(userId)) ||
            (userRoles != null && userRoles.Any(role => this.AssignedRoles.Contains(role))) ||
            (userGroups != null && userGroups.Any(group => this.AssignedGroups.Contains(group)));

        var ruleBased = this.AssignmentRule?.IsUserAuthorized(new UserContext
        {
            UserId = userId ?? string.Empty,
            Roles = userRoles ?? [],
            Groups = userGroups ?? []
        }) ?? false;

        return listBased || ruleBased;
    }

    /// <summary>
    /// Defines a conditional transition for the workflow action. This is a simplified version for the current release.
    /// </summary>
    /// <param name="condition">
    /// A function that evaluates the workflow data and returns true if this transition should be taken.
    /// The conditions are evaluated in the order they are added, and the first matching condition determines
    /// the target state.
    /// </param>
    /// <param name="targetState">
    /// The name of the state to transition to when the condition evaluates to true.
    /// </param>
    /// <returns>The current WorkflowAction instance to allow method chaining.</returns>
    /// <remarks>
    /// This is a simplified implementation for the current version. Future versions will introduce:
    /// <list type="bullet">
    /// <item><description>Explicit Transition objects with their own validation and hooks</description></item>
    /// <item><description>Transition priorities and conflict resolution</description></item>
    /// <item><description>Guard conditions and transition constraints</description></item>
    /// <item><description>Transition history and audit logging</description></item>
    /// <item><description>Visual transition representation for workflow diagrams</description></item>
    /// </list>
    /// 
    /// Example usage:
    /// <code>
    /// state.Action("Approve", "PendingManagerApproval",
    ///     action => action
    ///         .AssignToRoles("Manager")
    ///         .When(data => data.Amount > 10000, "PendingDirectorApproval")
    ///         .When(data => data.Amount > 5000, "PendingSupervisorApproval")
    /// );
    /// </code>
    /// </remarks>
    internal void When(Func<TData, bool> condition, string targetState)
    {
        this._transitionConditions.Add((condition, targetState));
    }

    /// <summary>
    /// Adds transition rules to the workflow action with custom labels.
    /// </summary>
    /// <param name="transitionRules">Array of transition rules containing conditions, target states, and labels.</param>
    /// <returns>The current workflow action instance for method chaining.</returns>
    internal WorkflowAction<TData> TransitionTo(
        params (Func<TData, bool> When, string Target, string Label)[] transitionRules)
    {
        foreach (var (condition, targetState, label) in transitionRules)
        {
            ValidateTransitionRule(condition, targetState);
            this._transitionRules.Add(new TransitionRule<TData>(condition, targetState, label));
        }

        return this;
    }

    /// <summary>
    /// Adds transition rules to the workflow action with auto-generated labels.
    /// </summary>
    /// <param name="transitionRules">Array of transition rules containing conditions and target states.</param>
    /// <returns>The current workflow action instance for method chaining.</returns>
    internal WorkflowAction<TData> TransitionTo(
        params (Func<TData, bool> When, string Target)[] transitionRules)
    {
        return this.TransitionTo(transitionRules.Select(rule =>
            (rule.When, rule.Target, Label: $"Transition to [{rule.Target}]")).ToArray());
    }
    
    /// Assigns the specified assignment rule to the workflow action, determining which users, roles, or groups are assigned to the action based on the rule logic.
    /// <param name="assignmentRule">The assignment rule to be applied. Must not be null.</param>
    /// <returns>The current workflow action with the updated assignment rule applied.</returns>
    internal WorkflowAction<TData> AssignTo(IAssignmentRule assignmentRule)
    {
        this.AssignmentRule = assignmentRule;
        return this;
    }

    /// Validates a transition rule by ensuring the condition and target state are properly defined.
    /// <param name="condition">The condition function that determines whether the transition is valid. Must not be null.</param>
    /// <param name="targetState">The target state to transition to if the condition is satisfied. Must not be null or empty.</param>
    /// <exception cref="ArgumentNullException">Thrown if the condition is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the target state is null, empty, or consists only of whitespace.</exception>
    private static void ValidateTransitionRule(Func<TData, bool>? condition, string? targetState)
    {
        if (condition is null)
            throw new ArgumentNullException(nameof(condition), "Condition cannot be null");

        if (string.IsNullOrWhiteSpace(targetState))
            throw new ArgumentException("Target state cannot be null or empty");
    }
}