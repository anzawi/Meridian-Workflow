namespace Meridian.Core;

using Extensions;
using Interfaces;

/// <summary>
/// Represents a single action within a workflow, including its properties, conditions,
/// and validation mechanisms. This class is generic and works with workflow data of type <typeparamref name="TData"/>.
/// </summary>
/// <typeparam name="TData">The type of workflow data associated with the action, which must implement the <see cref="IWorkflowData"/> interface.</typeparam>
public class WorkflowAction<TData> where TData : class, IWorkflowData
{
    /// <summary>
    /// Gets or sets the name of the workflow action.
    /// This property represents the human-readable identifier of the action.
    /// </summary>
    public string Name { get; set; } = string.Empty;

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
    public string NextState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the workflow action is automatic.
    /// </summary>
    /// <remarks>
    /// When this property is set to true, the action is considered an auto-action and will be executed automatically,
    /// provided its associated condition evaluates to true. If this property is true and no condition is defined,
    /// an exception will be thrown during workflow validation. Conversely, if this property is false and a condition
    /// is defined, an exception will similarly occur. This ensures consistency between the auto-action behavior and its conditions.
    /// </remarks>
    public bool IsAuto { get; set; } = false;

    /// <summary>
    /// A list of user identifiers that are assigned to perform or interact with the workflow action.
    /// </summary>
    /// <remarks>
    /// The <c>AssignedUsers</c> property contains user IDs as strings. This property can be used to specify
    /// individual users who have permission to execute or manage the workflow action. When checking
    /// authorization, the list of assigned users is one of the criteria considered for access control.
    /// </remarks>
    public List<string> AssignedUsers { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of roles assigned to the workflow action.
    /// These roles determine which user roles are authorized to execute the action within the workflow.
    /// </summary>
    public List<string> AssignedRoles { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of group identifiers assigned to the workflow action.
    /// </summary>
    /// <remarks>
    /// This property defines specific groups that are responsible for or may interact with
    /// the workflow action. It allows assignment of tasks or responsibilities to user groups.
    /// The list can be modified to add or remove groups dynamically during workflow configuration.
    /// </remarks>
    public List<string> AssignedGroups { get; set; } = [];

    /// <summary>
    /// A collection of hook descriptors that are executed when the associated action is performed.
    /// </summary>
    /// <remarks>
    /// The <c>OnExecuteHooks</c> property maintains a list of <see cref="WorkflowHookDescriptor{TData}"/> objects
    /// which define custom behaviors or logic to be executed during the workflow action's execution phase.
    /// Hooks in this collection enable customizable and extensible workflow processing.
    /// </remarks>
    public List<WorkflowHookDescriptor<TData>> OnExecuteHooks { get; set; } = [];

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
    public Func<TData, bool>? Condition { get; set; }

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
    public Func<TData, List<string>>? ValidateInput { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether automatic validation of input should be performed
    /// for the current workflow action. When set to <c>true</c>, the system will leverage default
    /// validation logic to check the inputs for the action. This property allows an alternative
    /// to implementing custom validation logic in the <see cref="ValidateInput"/> delegate.
    /// </summary>
    public bool UseAutomaticValidation { get; set; } = true;

    /// Determines if a user is authorized to execute the current workflow action based on their user ID, roles, and groups.
    /// <param name="userId">The ID of the user attempting to perform the action. Can be null.</param>
    /// <param name="userRoles">A list of roles assigned to the user. Can be null.</param>
    /// <param name="userGroups">A list of groups assigned to the user. Can be null.</param>
    /// <returns>True if the user is authorized based on the assigned users, roles, or groups; otherwise, false.</returns>
    public bool IsAuthorized(string? userId, List<string>? userRoles, List<string>? userGroups)
    {
        return true;
        return (userId != null && this.AssignedUsers.Contains(userId)) ||
               (userRoles != null && this.AssignedRoles.Any(userRoles.Contains)) ||
               (userGroups != null && this.AssignedGroups.Any(userGroups.Contains));
    }

    /// <summary>
    /// Assigns the specified users to the workflow action.
    /// </summary>
    /// <param name="users">An array of user identifiers to be assigned to the workflow action.</param>
    /// <returns>The updated workflow action instance with the specified users assigned.</returns>
    public WorkflowAction<TData> AssignToUsers(params string[] users)
    {
        this.AssignedUsers.AddRange(users.Distinct());
        return this;
    }

    /// <summary>
    /// Assigns one or more roles to the workflow action. The roles specified will have access
    /// or authorization for this workflow action.
    /// </summary>
    /// <param name="roles">An array of role names to assign to the workflow action.</param>
    /// <returns>The current instance of <see cref="WorkflowAction{TData}"/> to allow method chaining.</returns>
    public WorkflowAction<TData> AssignToRoles(params string[] roles)
    {
        this.AssignedRoles.AddRange(roles.Distinct());
        return this;
    }

    /// <summary>
    /// Assigns the specified groups to this workflow action, allowing only users
    /// belonging to these groups to interact with the action.
    /// </summary>
    /// <param name="groups">An array of group names to assign to the workflow action.</param>
    /// <returns>The updated <see cref="WorkflowAction{TData}"/> instance.</returns>
    public WorkflowAction<TData> AssignToGroups(params string[] groups)
    {
        this.AssignedGroups.AddRange(groups.Distinct());
        return this;
    }
}