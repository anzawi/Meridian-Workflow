namespace Meridian.Core.Exceptions;

/// <summary>
/// Base exception for all workflow-related errors
/// </summary>
public abstract class WorkflowException : Exception
{
    /// <summary>
    /// Base exception for all workflow-related exceptions.
    /// </summary>
    protected WorkflowException(string message) : base(message)
    {
    }

    /// <summary>
    /// Represents the base exception class for all exceptions related to workflows.
    /// </summary>
    protected WorkflowException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}