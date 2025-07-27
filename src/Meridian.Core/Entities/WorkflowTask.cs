namespace Meridian.Core.Entities;

using Enums;

/// <summary>
/// Represents a task associated with a workflow request, linking a specific action that needs to be performed
/// and a state within the workflow process. Tasks can be assigned to users, roles, or groups and
/// maintain their status throughout their lifecycle.
/// </summary>
public class WorkflowRequestTask
{
    /// <summary>
    /// Represents the name of the database table associated with the <see cref="WorkflowRequestTask"/> entity.
    /// </summary>
    /// <remarks>
    /// This constant is used for defining the table name in the database context and to ensure consistency
    /// when referencing the table in the application.
    /// </remarks>
    public const string TableName = "WorkflowRequestTasks";

    /// <summary>
    /// Gets or sets the unique identifier for the workflow request task.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Represents the unique identifier of the request associated with the workflow task.
    /// </summary>
    /// <remarks>
    /// It is a mandatory property that links the workflow task to the specific workflow request instance.
    /// This property is used to maintain consistency and traceability across workflow actions and states.
    /// </remarks>
    public Guid RequestId { get; set; }

    /// <summary>
    /// Gets or sets the name of the current workflow state.
    /// </summary>
    /// <remarks>
    /// This property represents the current state of the workflow task. It should be a non-null value
    /// corresponding to the state in the workflow process, such as "Pending", "Approved", or "Rejected".
    /// </remarks>
    public string State { get; set; } = null!;

    /// <summary>
    /// Represents the action associated with a specific workflow task.
    /// This property defines the name of the action that can be taken within a given state of a workflow.
    /// </summary>
    /// <remarks>
    /// The value of this property is critical for identifying and performing specific operations, such as creating or completing tasks
    /// within the workflow system. It is used to correlate the task's action with defined workflow transitions or behaviors.
    /// </remarks>
    public string Action { get; set; } = null!;

    /// <summary>
    /// Represents a collection of user identifiers to whom the workflow task
    /// is assigned. These users are directly responsible for handling the task.
    /// </summary>
    public List<string> AssignedToUsers { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of roles assigned to the task.
    /// These roles define which groups of users are eligible to perform the task.
    /// </summary>
    public List<string> AssignedToRoles { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of groups that the task is assigned to.
    /// </summary>
    /// <remarks>
    /// This property is used to define the groups responsible for handling the task.
    /// The groups are represented as a list of strings, where each string corresponds to a unique group identifier.
    /// </remarks>
    public List<string> AssignedToGroups { get; set; } = [];

    /// <summary>
    /// Gets or sets the identifier of the user who has taken or completed the task.
    /// </summary>
    /// <remarks>
    /// This property stores the ID of the user that performed the associated action
    /// on the workflow task. It is typically set when a task is marked as completed.
    /// </remarks>
    public string? TakenByUserId { get; set; }

    /// <summary>
    /// Represents the status of a workflow task.
    /// </summary>
    /// <remarks>
    /// The property indicates the current state of the workflow task.
    /// Possible values are defined in the <see cref="WorkflowTaskStatus"/> enum:
    /// Pending, InProgress, Completed, and Skipped.
    /// </remarks>
    public WorkflowTaskStatus Status { get; set; } = WorkflowTaskStatus.Pending;

    /// <summary>
    /// Gets or sets the timestamp when the workflow task was created.
    /// </summary>
    /// <remarks>
    /// This property is initialized to the current UTC time upon creation of the task.
    /// It is required and cannot be null in the database.
    /// </remarks>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the workflow task was completed.
    /// </summary>
    /// <remarks>
    /// This property is set to <c>null</c> until the task's status transitions to <c>WorkflowTaskStatus.Completed</c>.
    /// It is updated to the UTC timestamp representing the time of completion when the task is marked as completed.
    /// </remarks>
    public DateTime? CompletedAt { get; set; }

    public Dictionary<string, object?> Metadata { get; set; } = [];
}