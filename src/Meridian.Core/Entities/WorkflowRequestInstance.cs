namespace Meridian.Core.Entities;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Enums;

/// <summary>
/// Represents an instance of a workflow request within the system.
/// </summary>
public class WorkflowRequestInstance
{
    /// <summary>
    /// Represents the constant name of the database table associated with the WorkflowRequestInstance entity.
    /// </summary>
    public const string TableName = "REQUEST_INSTANCE";

    /// <summary>
    /// Gets or sets the unique identifier for the workflow request instance.
    /// </summary>
    /// <remarks>
    /// This property is used as the primary key in the database table to uniquely identify each workflow request instance.
    /// The identifier is a string representation of a GUID and is automatically generated when a new instance is created.
    /// </remarks>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the identifier of the workflow definition associated with the request instance.
    /// </summary>
    /// <remarks>
    /// The <c>DefinitionId</c> property uniquely links a workflow request instance to its respective workflow definition.
    /// It is utilized throughout the workflow engine to associate, query, and perform operations specific to the defined workflow.
    /// </remarks>
    public string DefinitionId { get; set; } = string.Empty;

    /// <summary>
    /// Represents the current state of the workflow request instance within a defined workflow.
    /// This property identifies the specific stage or status the workflow is in and can be used for tracking
    /// and filtering workflow requests within the system.
    /// </summary>
    public string CurrentState { get; set; } = string.Empty;

    /// <summary>
    /// Represents a JSON-encoded string used to store arbitrary data associated with a workflow request instance.
    /// Intended to serialize and deserialize structured data for storage or processing in workflow operations.
    /// </summary>
    public string DataJson { get; set; } = "{}";

    /// <summary>
    /// Represents the current operational state of the workflow request instance.
    /// </summary>
    /// <remarks>
    /// The <c>Status</c> property is defined using the <c>StateType</c> enumeration.
    /// It indicates the current state of a workflow, such as <c>Start</c>, <c>Normal</c>, <c>Completed</c>, <c>Cancelled</c>, or <c>Rejected</c>.
    /// This property is used to track and manage the workflow's life cycle.
    /// </remarks>
    public StateType Status { get; set; } = StateType.Normal;

    /// <summary>
    /// Represents a deserialized view of the JSON data stored in the `DataJson` property.
    /// Provides a dictionary structure for client code to interact with the underlying data.
    /// Returns an empty dictionary if the JSON data is null or invalid.
    /// </summary>
    [NotMapped]
    public Dictionary<string, object?> Data => 
        JsonSerializer.Deserialize<Dictionary<string, object?>>(this.DataJson) ?? new Dictionary<string, object?>();

    /// <summary>
    /// Sets the data for the workflow request instance by serializing the provided object to JSON format.
    /// </summary>
    /// <typeparam name="T">The type of the input data object.</typeparam>
    /// <param name="data">The object to be serialized and set as the workflow request instance data.</param>
    public void SetData<T>(T data)
    {
        this.DataJson = JsonSerializer.Serialize(data);
    }

    /// <summary>
    /// Represents the list of transitions associated with the workflow request instance.
    /// Each transition captures the state change, the action performed, and other relevant metadata.
    /// </summary>
    /// <remarks>
    /// This property provides a historical log of all workflow state transformations.
    /// It includes details such as the originating state, target state, action performed,
    /// timestamp, and metadata related to the transition.
    /// </remarks>
    public List<WorkflowTransition> Transitions { get; set; } = [];

    /// Deserializes the stored JSON data into the specified type.
    /// <typeparam name="T">The type to which the data will be deserialized.</typeparam>
    /// <returns>The deserialized data of type T.</returns>
    public T GetData<T>()
    {
        return JsonSerializer.Deserialize<T>(this.DataJson)!;
    }
}
