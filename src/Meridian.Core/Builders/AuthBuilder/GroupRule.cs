namespace Meridian.Core.Builders.AuthBuilder;

using Contexts;
using Core;
using Meridian.Core.Interfaces.AuthBuilder;

/// <summary>
/// Represents an authorization rule based on user group membership.
/// This rule checks if a user belongs to one or more specified groups to determine authorization.
/// </summary>
/// <remarks>
/// The <see cref="GroupRule"/> is used to validate if a user is authorized based on their group memberships.
/// It compares the list of groups assigned to the user against the specified groups in the rule.
/// </remarks>
/// <param name="groups">An array of group names used for authorization validation.</param>
/// <example>
/// This rule can be combined with other rules in the <see cref="AssignmentRuleBuilder"/> to form complex authorization logic.
/// </example>
internal class GroupRule(params string[] groups) : IAssignmentRule
{
    /// <summary>
    /// Determines whether the specified user is authorized based on the implemented rule.
    /// </summary>
    /// <param name="user">The context of the user including identification, roles, and groups.</param>
    /// <returns>True if the user satisfies the authorization rule; otherwise, false.</returns>
    public bool IsUserAuthorized(UserContext user) =>
        user.Groups.Intersect(groups).Any();
}