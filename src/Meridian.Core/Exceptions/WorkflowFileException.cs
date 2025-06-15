namespace Meridian.Core.Exceptions;

using Enums;

/// <summary>
/// Thrown when workflow file operations fail
/// </summary>
public class WorkflowFileException(
    string fileReference,
    WorkflowFileOperation operation,
    string message,
    Exception? innerException = null)
    : WorkflowException($"File operation '{operation}' failed for '{fileReference}': {message}", innerException ?? new Exception(message))
{
    public string FileReference { get; } = fileReference;
    public WorkflowFileOperation Operation { get; } = operation;
}