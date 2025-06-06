namespace Meridian.Core.Interfaces.AuthBuilder;

using Meridian.Core;

/// <summary>
/// Represents the contract for an authorization rule that determines whether a user
/// is authorized based on specific conditions or criteria.
/// </summary>
/// <remarks>
/// Implementations of this interface define the logic required to evaluate user authorization
/// by using the provided user context. Rules can vary from simple checks, such as user roles or group membership,
/// to more complex composite rules combining multiple conditions.
/// </remarks>
public interface IAssignmentRule
{
    /// <summary>
    /// Determines whether a user is authorized based on the implemented rule logic.
    /// </summary>
    /// <param name="user">The user context containing information about the user to be evaluated.</param>
    /// <returns>True if the user is authorized according to the rule; otherwise, false.</returns>
    bool IsUserAuthorized(UserContext user);
}