namespace Meridian.Core.Exceptions;

/// <summary>
/// Thrown when workflow hook execution fails
/// </summary>
public class WorkflowHookException(string hookName, string message, bool isCritical, Exception? innerException = null)
    : WorkflowException($"Hook '{hookName}' execution failed: {message}", innerException ?? new Exception(message))
{
    public string HookName { get; } = hookName;
    public bool IsCritical { get; } = isCritical;
}