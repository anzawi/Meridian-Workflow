namespace Meridian.Core.Exceptions;

/// <summary>
/// Thrown when workflow action execution fails
/// </summary>
public class WorkflowActionException : WorkflowException
{
    public string ActionName { get; }
    public string StateName { get; }
    public string DefinitionId { get; }

    public WorkflowActionException(string definitionId, string stateName, string actionName, string message)
        : base($"Action '{actionName}' in state '{stateName}' of workflow '{definitionId}': {message}")
    {
        this.ActionName = actionName;
        this.StateName = stateName;
        this.DefinitionId = definitionId;
    }
}
