namespace Meridian.Core.Builders.AuthBuilder;

using Contexts;
using Meridian.Core.Interfaces.AuthBuilder;

/// <summary>
/// Represents a composite authorization rule that evaluates multiple rules and authorizes a user
/// if at least one of the specified rules is satisfied.
/// </summary>
/// <remarks>
/// The <c>OrRule</c> class implements the <see cref="IAssignmentRule"/> interface, enabling the
/// composition of multiple authorization rules using a logical OR condition. This rule returns
/// true if any of the provided rules evaluate to true, granting authorization to the user.
/// </remarks>
internal class OrRule(params IAssignmentRule[] rules) : IAssignmentRule
{
    /// <summary>
    /// Determines whether a user is authorized based on the specified rule or combination of rules.
    /// </summary>
    /// <param name="user">The user context containing user-specific information such as user ID, roles, and groups.</param>
    /// <returns>True if at least one of the rules authorizes the user; otherwise, false.</returns>
    public bool IsUserAuthorized(UserContext user) => rules.Any(r => r.IsUserAuthorized(user));
}