namespace Meridian.Core.Exceptions;

using Enums;

/// <summary>
/// Thrown when workflow file operations fail
/// </summary>
public class WorkflowFileException : WorkflowException
{
    public string FileReference { get; }
    public WorkflowFileOperation Operation { get; }

    public WorkflowFileException(string fileReference, WorkflowFileOperation operation, string message,
        Exception? innerException = null)
        : base($"File operation '{operation}' failed for '{fileReference}': {message}", innerException)
    {
        this.FileReference = fileReference;
        this.Operation = operation;
    }
}