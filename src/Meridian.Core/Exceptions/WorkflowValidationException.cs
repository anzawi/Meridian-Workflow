namespace Meridian.Core.Exceptions;

/// <summary>
/// Thrown when workflow data validation fails
/// </summary>
public class WorkflowValidationException : WorkflowException
{
    public IReadOnlyList<string> ValidationErrors { get; }

    public WorkflowValidationException(List<string> errors)
        : base($"Workflow validation failed: {string.Join("; ", errors)}")
    {
        this.ValidationErrors = errors.ToList().AsReadOnly();
    }
}