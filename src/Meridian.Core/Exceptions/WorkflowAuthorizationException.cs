namespace Meridian.Core.Exceptions;

/// <summary>
/// Thrown when workflow authorization fails
/// </summary>
public class WorkflowAuthorizationException : WorkflowException
{
    public string UserId { get; }
    public IReadOnlyList<string> RequiredRoles { get; }
    public IReadOnlyList<string> RequiredGroups { get; }

    public WorkflowAuthorizationException(string userId, IEnumerable<string> requiredRoles,
        IEnumerable<string> requiredGroups)
        : base($"User '{userId}' is not authorized to perform this workflow action")
    {
        this.UserId = userId;
        this.RequiredRoles = requiredRoles.ToList().AsReadOnly();
        this.RequiredGroups = requiredGroups.ToList().AsReadOnly();
    }
}
