namespace Meridian.Core.Builders.AuthBuilder;

using Contexts;
using Core;
using Meridian.Core.Interfaces.AuthBuilder;

/// <summary>
/// Represents an authorization rule based on a specific list of users.
/// </summary>
/// <remarks>
/// This class verifies whether a given user is authorized by checking if their
/// unique user identifier exists within a predefined list of users.
/// It implements the <see cref="IAssignmentRule"/> interface.
/// </remarks>
internal class UserRule(params string[] users) : IAssignmentRule
{
    /// <summary>
    /// Determines whether the specified user is authorized based on a predefined list of user IDs.
    /// </summary>
    /// <param name="user">
    /// The <see cref="UserContext"/> representing the user whose authorization is being evaluated.
    /// </param>
    /// <returns>
    /// <c>true</c> if the user's UserId matches any of the predefined user IDs for this rule; otherwise, <c>false</c>.
    /// </returns>
    public bool IsUserAuthorized(UserContext user) =>
        users.Contains(user.UserId);
}