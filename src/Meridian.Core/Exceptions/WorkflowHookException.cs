namespace Meridian.Core.Exceptions;

/// <summary>
/// Thrown when workflow hook execution fails
/// </summary>
public class WorkflowHookException : WorkflowException
{
    public string HookName { get; }
    public bool IsCritical { get; }

    public WorkflowHookException(string hookName, string message, bool isCritical, Exception? innerException = null)
        : base($"Hook '{hookName}' execution failed: {message}", innerException)
    {
        this.HookName = hookName;
        this.IsCritical = isCritical;
    }
}