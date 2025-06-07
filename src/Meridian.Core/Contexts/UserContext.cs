namespace Meridian.Core.Contexts;

/// <summary>
/// Represents the contextual information of a user within the system.
/// </summary>
/// <remarks>
/// This class encapsulates user-related data, including the user's unique identifier,
/// assigned roles, and groups. It can be utilized to track user context during authorization
/// checks, application workflows, or data filtering mechanisms.
/// </remarks>
public class UserContext
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    /// <remarks>
    /// This property represents the primary identifier for a user within the system,
    /// used to reference and distinguish individual users in various contexts such as
    /// authentication, authorization, and user management.
    /// </remarks>
    public string UserId { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of roles assigned to the user.
    /// </summary>
    /// <remarks>
    /// This property represents the collection of roles associated with the user
    /// in the context of authorization and access control. It can be used for role-based
    /// evaluation to determine whether the user has the requisite permissions or access rights.
    /// </remarks>
    public List<string> Roles { get; internal set; } = [];

    /// <summary>
    /// Gets or sets the list of groups associated with the user.
    /// </summary>
    /// <remarks>
    /// Groups represent collections or categories to which a user belongs.
    /// These groups can be used for access control, organizing users, or enforcing business rules.
    /// </remarks>
    public List<string> Groups { get; internal set; } = [];
}