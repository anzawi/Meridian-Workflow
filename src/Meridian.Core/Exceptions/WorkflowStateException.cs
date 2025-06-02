namespace Meridian.Core.Exceptions;

/// <summary>
/// Thrown when workflow state-related operations fail
/// </summary>
public class WorkflowStateException : WorkflowException
{
    public string StateName { get; }
    public string DefinitionId { get; }

    public WorkflowStateException(string definitionId, string stateName, string message)
        : base($"State '{stateName}' in workflow '{definitionId}': {message}")
    {
        this.StateName = stateName;
        this.DefinitionId = definitionId;
    }
}
