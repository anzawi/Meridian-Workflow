namespace Meridian.Core;

using Enums;
using Interfaces;

/// <summary>
/// Represents an instance of a workflow request, encapsulating the state and
/// metadata associated with a workflow process.
/// </summary>
/// <typeparam name="TData">
/// The type of workflow data that implements the <see cref="IWorkflowData"/> interface.
/// </typeparam>
public class WorkflowRequestInstance<TData> where TData : class, IWorkflowData
{
    /// <summary>
    /// Gets or sets the unique identifier of the workflow request instance.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the identifier of the workflow definition.
    /// This is used to associate a workflow request instance with a specific workflow definition.
    /// </summary>
    public string DefinitionId { get; set; } = string.Empty;

    /// <summary>
    /// Represents the current state of the workflow request instance.
    /// </summary>
    /// <remarks>
    /// This property indicates the current state name within the workflow lifecycle.
    /// It is updated as the workflow progresses through different states, either through
    /// actions performed by a user or system-driven transitions.
    /// </remarks>
    public string CurrentState { get; set; } = string.Empty;

    /// <summary>
    /// Represents the typed data associated with a workflow request instance.
    /// </summary>
    /// <remarks>
    /// The <c>Data</c> property serves as a container for the workflow-specific data used during
    /// the lifecycle of a workflow request. This property is generic and bound by the <c>IWorkflowData</c>
    /// interface to ensure type safety and contract adherence.
    /// </remarks>
    /// <typeparam name="TData">The type of the workflow data that must inherit from <c>IWorkflowData</c>.</typeparam>
    /// <value>
    /// Gets or sets the instance of the data associated with the workflow request.
    /// This value may be null if the data has not been assigned.
    /// </value>
    public TData? Data { get; set; }

    /// <summary>
    /// Represents the collection of workflow transitions associated with a workflow request instance.
    /// </summary>
    /// <remarks>
    /// This property holds the list of transitions that record the changes or movements
    /// between states in the workflow for a given request instance.
    /// </remarks>
    public List<WorkflowTransition> Transitions { get; set; } = [];

    /// <summary>
    /// Gets or sets the current status of the workflow request instance.
    /// </summary>
    /// <remarks>
    /// The status represents the current lifecycle state of the workflow request instance,
    /// which could be one of the predefined values in the <c>StateType</c> enumeration
    /// such as Start, Normal, Completed, Cancelled, or Rejected.
    /// </remarks>
    public StateType Status { get; set; } = StateType.Normal;
}
