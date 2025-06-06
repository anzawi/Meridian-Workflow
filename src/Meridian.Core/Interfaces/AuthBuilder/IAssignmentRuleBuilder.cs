namespace Meridian.Core.Interfaces.AuthBuilder;

/// <summary>
/// Interface for building assignment rules for workflow action assignments.
/// Provides methods to define rules based on roles, groups, users, and logical conditions.
/// </summary>
public interface IAssignmentRuleBuilder
{
    /// <summary>
    /// Adds a role-based assignment rule to the builder using the specified roles.
    /// </summary>
    /// <param name="roles">An array of role names to be used in the assignment rule.</param>
    /// <returns>An instance of <see cref="IAssignmentRuleBuilder"/> to allow method chaining for further rule configuration.</returns>
    IAssignmentRuleBuilder Role(params string[] roles);

    /// <summary>
    /// Adds a rule to restrict access based on group memberships.
    /// </summary>
    /// <param name="groups">An array of group names that will be used to define the rule.</param>
    /// <returns>Returns the current instance of <see cref="IAssignmentRuleBuilder"/> to allow method chaining.</returns>
    IAssignmentRuleBuilder Group(params string[] groups);

    /// <summary>
    /// Adds a user-based assignment rule to include specific users in the authorization process.
    /// </summary>
    /// <param name="users">An array of user identifiers to be included in the rule.</param>
    /// <returns>Returns the current instance of <see cref="IAssignmentRuleBuilder"/> to allow method chaining.</returns>
    IAssignmentRuleBuilder User(params string[] users);

    /// <summary>
    /// Adds a negation ("Not") rule to the assignment rule builder, negating the inner rules provided.
    /// </summary>
    /// <param name="inner">A delegate that configures the inner assignment rules to be negated.</param>
    /// <returns>An instance of <see cref="IAssignmentRuleBuilder"/> to allow further rule chaining.</returns>
    IAssignmentRuleBuilder Not(Action<IAssignmentRuleBuilder> inner);

    /// <summary>
    /// Combines multiple assignment rule blocks with an OR condition, ensuring that at least one of the provided blocks satisfies its conditions.
    /// </summary>
    /// <param name="blocks">An array of delegate actions that define individual assignment rule blocks. Each action applies specific rules using an <see cref="IAssignmentRuleBuilder"/> instance.</param>
    /// <returns>An instance of <see cref="IAssignmentRuleBuilder"/> with the OR condition integrated into the rule set.</returns>
    IAssignmentRuleBuilder Or(params Action<IAssignmentRuleBuilder>[] blocks);

    /// <summary>
    /// Combines multiple assignment rule blocks using a logical "AND" operation.
    /// All specified blocks must evaluate to true for the combined rule to be satisfied.
    /// </summary>
    /// <param name="blocks">An array of delegate actions used to define assignment rule blocks with an <see cref="IAssignmentRuleBuilder"/>.</param>
    /// <returns>An instance of <see cref="IAssignmentRuleBuilder"/> for further rule building.</returns>
    IAssignmentRuleBuilder And(params Action<IAssignmentRuleBuilder>[] blocks);

    /// <summary>
    /// Builds and returns a combined assignment rule based on the configured rules.
    /// The resulting rule encapsulates the authorization logic specified during the builder's configuration process.
    /// </summary>
    /// <returns>
    /// An instance of IAssignmentRule representing the combined authorization rule.
    /// </returns>
    IAssignmentRule Build();
}