namespace Meridian.Core.Exceptions;

/// <summary>
/// Thrown when a workflow definition validation fails
/// </summary>
public class WorkflowDefinitionException : WorkflowException
{
    public string DefinitionId { get; }

    public WorkflowDefinitionException(string definitionId, string message)
        : base($"Workflow definition '{definitionId}': {message}")
    {
        this.DefinitionId = definitionId;
    }
}