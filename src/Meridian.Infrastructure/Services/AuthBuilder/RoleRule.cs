namespace Meridian.Infrastructure.Services.AuthBuilder;

using Core;
using Core.Interfaces.AuthBuilder;

/// <summary>
/// Represents an authorization rule based on user roles.
/// </summary>
/// <remarks>
/// This rule checks whether a user possesses at least one of the specified roles
/// to determine if they are authorized. The evaluation is performed by intersecting
/// the roles provided during class initialization with the roles assigned to the user
/// in the provided <see cref="UserContext"/>.
/// </remarks>
internal class RoleRule(params string[] roles) : IAssignmentRule
{
    /// <summary>
    /// Determines whether the user is authorized based on the defined criteria.
    /// </summary>
    /// <param name="user">The user context containing relevant information such as roles and user ID.</param>
    /// <returns>True if the user meets the authorization criteria; otherwise, false.</returns>
    public bool IsUserAuthorized(UserContext user) =>
        user.Roles.Intersect(roles).Any();
}