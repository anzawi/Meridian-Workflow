namespace Meridian.Core.Builders.AuthBuilder;

using Meridian.Core.Interfaces.AuthBuilder;

/// <summary>
/// Provides a mechanism to build complex assignment rules used in the workflow action assignment process.
/// </summary>
internal class AssignmentRuleBuilder : IAssignmentRuleBuilder
{
    /// <summary>
    /// A collection of assignment rules used to define authorization logic.
    /// This list contains instances of rules that implement the <see cref="Meridian.Core.Interfaces.AuthBuilder.IAssignmentRule"/> interface.
    /// Rules are added through various methods in the AssignmentRuleBuilder class, such as Role, Group, User, Not, Or, and And.
    /// </summary>
    private readonly List<IAssignmentRule> _rules = [];

    /// <summary>
    /// Adds a role-based assignment rule to the builder using the specified roles.
    /// </summary>
    /// <param name="roles">An array of role names to be used in the assignment rule.</param>
    /// <returns>An instance of <see cref="IAssignmentRuleBuilder"/> to allow method chaining for further rule configuration.</returns>
    public IAssignmentRuleBuilder Role(params string[] roles)
    {
        this._rules.Add(new RoleRule(roles));
        return this;
    }

    /// <summary>
    /// Adds a group-based rule to the assignment rule builder. This allows restricting access to specific group memberships.
    /// </summary>
    /// <param name="groups">An array of group names to be included in the rule.</param>
    /// <returns>Returns the current instance of <see cref="IAssignmentRuleBuilder"/> for chaining additional rules.</returns>
    public IAssignmentRuleBuilder Group(params string[] groups)
    {
        this._rules.Add(new GroupRule(groups));
        return this;
    }

    /// <summary>
    /// Adds a user-based assignment rule to the builder, specifying the users
    /// that should be authorized under the rule.
    /// </summary>
    /// <param name="users">An array of user identifiers to include in the rule.</param>
    /// <returns>Returns the current instance of <see cref="IAssignmentRuleBuilder"/> for method chaining.</returns>
    public IAssignmentRuleBuilder User(params string[] users)
    {
        this._rules.Add(new UserRule(users));
        return this;
    }

    /// <summary>
    /// Adds a "Not" rule to the current assignment rule builder, negating the inner rules specified by the provided configuration.
    /// </summary>
    /// <param name="inner">A delegate that defines the inner rules to be negated. This delegate takes an instance of <see cref="IAssignmentRuleBuilder"/> to specify the inner rules.</param>
    /// <returns>An instance of <see cref="IAssignmentRuleBuilder"/> for chaining further configuration.</returns>
    public IAssignmentRuleBuilder Not(Action<IAssignmentRuleBuilder> inner)
    {
        var innerBuilder = new AssignmentRuleBuilder();
        inner(innerBuilder);
        this._rules.Add(new NotRule(innerBuilder.Build()));
        return this;
    }

    /// <summary>
    /// Combines multiple assignment rule blocks with an OR condition, where at least one of the provided blocks must satisfy the conditions.
    /// </summary>
    /// <param name="blocks">An array of delegate actions used to define individual assignment rule blocks within an OR condition.</param>
    /// <returns>An instance of <see cref="IAssignmentRuleBuilder"/> with the OR condition applied.</returns>
    public IAssignmentRuleBuilder Or(params Action<IAssignmentRuleBuilder>[] blocks)
    {
        var rules = blocks.Select(block =>
        {
            var builder = new AssignmentRuleBuilder();
            block(builder);
            return builder.Build();
        }).ToArray();

        this._rules.Add(new OrRule(rules));
        return this;
    }

    /// <summary>
    /// Combines multiple assignment rule blocks using a logical "AND" operation.
    /// All specified blocks must evaluate to true for the combined rule to be satisfied.
    /// </summary>
    /// <param name="blocks">An array of delegate actions used to define assignment rule blocks with an <see cref="IAssignmentRuleBuilder"/>.</param>
    /// <returns>An instance of <see cref="IAssignmentRuleBuilder"/> for further rule building.</returns>
    public IAssignmentRuleBuilder And(params Action<IAssignmentRuleBuilder>[] blocks)
    {
        var rules = blocks.Select(block =>
        {
            var builder = new AssignmentRuleBuilder();
            block(builder);
            return builder.Build();
        }).ToArray();

        this._rules.Add(new AndRule(rules));
        return this;
    }

    /// <summary>
    /// Builds and returns a combined assignment rule based on the previously defined rules.
    /// The resulting rule encapsulates the logic for determining if a user is authorized
    /// based on the rules configured in the builder.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="IAssignmentRule"/> which represents the combined
    /// authorization rule, using an "AND" combination if multiple rules were configured.
    /// </returns>
    public IAssignmentRule Build()
    {
        return this._rules.Count == 1 ? this._rules[0] : new AndRule(this._rules.ToArray());
    }
}