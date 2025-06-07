namespace Meridian.Core.Models;

/// <summary>
/// Represents a workflow transition that occurs during the lifecycle of a request.
/// </summary>
/// <remarks>
/// A transition captures the movement of a workflow from one state to another, along with
/// metadata related to the action, timestamp, and user performing the transition. It can
/// be used to track the history of changes applied in workflows.
/// </remarks>
public class WorkflowTransition
{
    /// <summary>
    /// Represents the constant name of the database table used to store workflow transitions in the system.
    /// </summary>
    public const string TableName = "REQUEST_TRANSITION";

    /// <summary>
    /// Represents the unique identifier for a workflow transition.
    /// This property serves as the primary key for the WorkflowTransition entity
    /// and is used to uniquely identify each transition in the workflow process.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the timestamp indicating the date and time of a specific workflow transition event.
    /// </summary>
    /// <remarks>
    /// The Timestamp property typically represents when the transition occurred
    /// and is stored as a <see cref="DateTime"/>. It is often set to the current
    /// universal time (UTC) during the creation of a new transition.
    /// </remarks>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Represents the initial state from which a workflow transition originates.
    /// </summary>
    /// <remarks>
    /// This property stores the name of the state where the transition begins before
    /// moving to the next state. It is expected to be set during the execution of
    /// workflow actions to capture the state history within the workflow.
    /// </remarks>
    public string? FromState { get; set; } = string.Empty;

    /// <summary>
    /// Represents the specific operation or task being performed during a workflow transition.
    /// </summary>
    /// <remarks>
    /// The Action property denotes the name or identifier of the activity carried out
    /// when transitioning between workflow states. This property is typically used to
    /// log or track the activities performed and is integral to maintaining the history
    /// of workflow movements.
    /// </remarks>
    public string? Action { get; set; } = string.Empty;

    /// <summary>
    /// Represents the target state of a workflow transition.
    /// </summary>
    /// <remarks>
    /// This property denotes the state to which the workflow should transition
    /// as a result of a specific action being performed. It is typically used
    /// to track the destination state when a transition occurs within a workflow.
    /// </remarks>
    public string? ToState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the user or system that performed the action associated with the workflow transition.
    /// </summary>
    /// <remarks>
    /// Typically, this property captures the identifier of the user or process responsible for initiating the transition
    /// from one state to another within the workflow. It is used for auditing and tracking purposes.
    /// </remarks>
    /// <seealso cref="WorkflowTransition"/>
    public string? PerformedBy { get; set; } = string.Empty;

    /// <summary>
    /// Represents the type of a transition or operation occurring within the workflow.
    /// This property defines the nature of the action or transition being logged.
    /// </summary>
    /// <remarks>
    /// Commonly used to classify or categorize workflow transitions, such as "Event",
    /// "AuditLog", "DataChanged", or other custom types to provide context for history logging.
    /// Defaults to an empty string if not explicitly set.
    /// </remarks>
    public string? Type { get; set; } = string.Empty;

    /// <summary>
    /// Represents the source from which the workflow transition originated.
    /// This property is used to provide information about the origin or initiator
    /// of a transition within a workflow, for example, identifying a system
    /// or component responsible for the transition.
    /// </summary>
    public string? Source { get; set; } = string.Empty;

    /// <summary>
    /// A property that contains additional metadata associated with a workflow transition.
    /// </summary>
    /// <remarks>
    /// The <c>Metadata</c> property is a dictionary that allows storing key-value pairs where
    /// the keys are strings and the values can be any object type. This enables flexible and
    /// dynamic storage of extra information pertaining to a specific workflow transition.
    /// </remarks>
    public Dictionary<string, object?> Metadata { get; set; } = new();
}