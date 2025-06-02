namespace Meridian.Core.Exceptions;

/// <summary>
/// Thrown when workflow persistence operations fail
/// </summary>
public class WorkflowPersistenceException : WorkflowException
{
    public string RequestId { get; }

    public WorkflowPersistenceException(string requestId, string message, Exception innerException)
        : base($"Workflow persistence error for request '{requestId}': {message}", innerException)
    {
        this.RequestId = requestId;
    }
}
