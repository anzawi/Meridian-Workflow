namespace Meridian.Infrastructure.Extensions;

using Core;
using Core.Interfaces;
using Core.Interfaces.AuthBuilder;
using Services.AuthBuilder;

/// <summary>
/// Provides extension methods for assigning workflow actions to users, roles, or groups
/// based on custom assignment rules.
/// </summary>
public static class WorkflowActionAssignmentExtensions
{
    /// <summary>
    /// Configures and applies an assignment rule to the specified workflow action.
    /// </summary>
    /// <typeparam name="TData">The type of the workflow data associated with the action, which must implement the <see cref="IWorkflowData"/> interface.</typeparam>
    /// <param name="action">The workflow action to which the assignment rule will be applied.</param>
    /// <param name="builder">A delegate to define the assignment rule using an <see cref="IAssignmentRuleBuilder"/>.</param>
    /// <returns>The workflow action with the applied assignment rule.</returns>
    public static WorkflowAction<TData> AssignTo<TData>(
        this WorkflowAction<TData> action,
        Action<IAssignmentRuleBuilder> builder)
        where TData : class, IWorkflowData
    {
        var ruleBuilder = new AssignmentRuleBuilder();
        builder(ruleBuilder);
        action.AssignTo(ruleBuilder.Build());
        return action;
    }
}
