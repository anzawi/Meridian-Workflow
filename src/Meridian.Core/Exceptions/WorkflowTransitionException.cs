namespace Meridian.Core.Exceptions;

/// <summary>
/// Thrown when workflow transition between states fails
/// </summary>
public class WorkflowTransitionException : WorkflowException
{
    public string FromState { get; }
    public string ToState { get; }
    public string DefinitionId { get; }

    public WorkflowTransitionException(string definitionId, string fromState, string toState, string message)
        : base($"Cannot transition from '{fromState}' to '{toState}' in workflow '{definitionId}': {message}")
    {
        this.FromState = fromState;
        this.ToState = toState;
        this.DefinitionId = definitionId;
    }
}
