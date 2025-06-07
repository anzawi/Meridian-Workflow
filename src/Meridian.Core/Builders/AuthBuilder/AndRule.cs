namespace Meridian.Core.Builders.AuthBuilder;

using Contexts;
using Core;
using Meridian.Core.Interfaces.AuthBuilder;

/// <summary>
/// Represents a composite authorization rule that evaluates multiple assignment rules
/// and authorizes a user only if all of the rules are satisfied.
/// </summary>
/// <remarks>
/// The <see cref="AndRule"/> combines multiple <see cref="IAssignmentRule"/> implementations
/// and evaluates them using a logical AND operation. If all provided rules return true
/// from their <see cref="IAssignmentRule.IsUserAuthorized(UserContext)"/> method for the given
/// user context, this rule will also return true.
/// </remarks>
/// <example>
/// This rule is typically used to combine multiple assignment rules into a single check
/// where all rules must be satisfied for a user to be authorized.
/// </example>
internal class AndRule(params IAssignmentRule[] rules) : IAssignmentRule
{
    /// <summary>
    /// Evaluates whether the specified user is authorized based on the set of assignment rules.
    /// </summary>
    /// <param name="user">The context of the user including identification, roles, and groups.</param>
    /// <returns>True if the user meets all the defined authorization rules; otherwise, false.</returns>
    public bool IsUserAuthorized(UserContext user) => rules.All(r => r.IsUserAuthorized(user));
}