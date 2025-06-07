namespace Meridian.Core.Contexts;

using Meridian.Core.Interfaces;
using Models;

/// <summary>
/// Represents the context of a workflow request, containing information about
/// the user, input data, request instance, and history of transitions.
/// </summary>
/// <typeparam name="TData">The type of workflow data implementing <see cref="IWorkflowData"/>.</typeparam>
public class WorkflowContext<TData> where TData : class, IWorkflowData
{
    /// <summary>
    /// Represents the current instance of a workflow request within a workflow context.
    /// This property provides access to the workflow request instance and its associated
    /// data, state, and transitions during the execution of the workflow.
    /// </summary>
    /// <typeparam name="TData">
    /// The type of the workflow data associated with the request. Must adhere to the
    /// <see cref="IWorkflowData"/> contract.
    /// </typeparam>
    public WorkflowRequestInstance<TData> Request { get; set; } = null!;

    /// <summary>
    /// Represents the input data provided to the workflow context during its execution.
    /// </summary>
    /// <remarks>
    /// This property is used to store temporary user-supplied or external data
    /// relevant for the current step in the workflow process.
    /// It can be null if no input data is provided.
    /// </remarks>
    public TData? InputData { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user performing the action within the workflow context.
    /// </summary>
    /// <remarks>
    /// This property is used to track the user responsible for executing specific actions
    /// in the workflow process. It is typically set as part of the workflow execution flow
    /// and can be utilized for auditing or authorization purposes.
    /// </remarks>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Represents the collection of roles associated with the user in the workflow context.
    /// </summary>
    /// <remarks>
    /// This property is utilized to determine the roles of a user when performing actions
    /// within a workflow. It is primarily used for authorization purposes to ensure
    /// that users have the necessary permissions to execute specific workflow actions.
    /// </remarks>
    public List<string> UserRoles { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of user groups associated with the current workflow context.
    /// </summary>
    /// <remarks>
    /// This property contains the groups to which the user performing the current workflow action belongs.
    /// It is used for authorization and workflow transitions where group-level restrictions or permissions are applied.
    /// </remarks>
    public List<string> UserGroups { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of workflow transitions representing the history of changes
    /// and actions performed during the workflow execution. Each entry in the history contains
    /// details such as the timestamp, source, state transitions, and metadata about the specific
    /// action or change.
    /// </summary>
    public List<WorkflowTransition> History { get; set; } = [];
}