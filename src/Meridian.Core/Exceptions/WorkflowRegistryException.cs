namespace Meridian.Core.Exceptions;

/// <summary>
/// Thrown when workflow registry operations fail
/// </summary>
public class WorkflowRegistryException : WorkflowException
{
    public string DefinitionId { get; }
    public Type? DataType { get; }

    public WorkflowRegistryException(string definitionId, Type? dataType, string message)
        : base($"Registry operation failed for workflow '{definitionId}' with data type '{dataType?.Name}': {message}")
    {
        this.DefinitionId = definitionId;
        this.DataType = dataType;
    }
}