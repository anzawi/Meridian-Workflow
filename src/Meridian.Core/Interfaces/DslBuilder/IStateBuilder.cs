namespace Meridian.Core.Interfaces.DslBuilder;

using Enums;
using Hooks;
using Models;

/// <summary>
/// Defines a builder interface for configuring states within a workflow and managing their associated actions, status, and hooks.
/// </summary>
/// <typeparam name="TData">
/// The type of the workflow data associated with the state being configured. Must implement <see cref="IWorkflowData"/>.
/// </typeparam>
public interface IStateBuilder<TData>: IHookBuilder<IStateBuilder<TData>, TData, StateHookType>
    where TData : class, IWorkflowData
{
    /// <summary>
    /// Marks the current workflow state as the starting state of the workflow.
    /// </summary>
    /// <returns>
    /// The updated workflow state builder instance with the state configured as the starting point.
    /// </returns>
    IStateBuilder<TData> IsStart();

    /// <summary>
    /// Marks the current state as a completed state in the workflow.
    /// </summary>
    /// <returns>
    /// The updated state builder instance with the completion status applied to the state.
    /// </returns>
    IStateBuilder<TData> IsCompleted();

    /// <summary>
    /// Marks the current workflow state as a "Rejected" state.
    /// </summary>
    /// <returns>
    /// The updated workflow state instance, marked as rejected.
    /// </returns>
    /// <remarks>
    /// This method is used to explicitly designate a state where the workflow transitions to when an operation or action results in rejection.
    /// Hooks, such as notifications or validations, can be configured on this state for further processing.
    /// </remarks>
    IStateBuilder<TData> IsRejected();

    /// <summary>
    /// Marks the current workflow state as canceled, denoting that the state is part of a cancellation flow or outcome.
    /// </summary>
    /// <returns>
    /// The updated workflow state builder instance with the cancellation type applied.
    /// </returns>
    IStateBuilder<TData> IsCanceled();
    
      /// <summary>
    /// Adds a configurable action to the current workflow state with conditional transitions support.
    /// </summary>
    /// <param name="name">
    /// The name of the action to be added.
    /// </param>
    /// <param name="nextState">
    /// The default state to transition to after the action is executed. This state will be used when:
    /// <list type="bullet">
    /// <item><description>No conditions are defined using the <see cref="WorkflowAction{TData}.When"/> method</description></item>
    /// <item><description>None of the conditions defined using <see cref="WorkflowAction{TData}.When"/> evaluate to true</description></item>
    /// </list>
    /// </param>
    /// <param name="config">
    /// The configuration callback to apply additional settings to the action. Within this callback, you can:
    /// <list type="bullet">
    /// <item><description>Add conditional transitions using the <see cref="WorkflowAction{TData}.When"/> method</description></item>
    /// <item><description>Assign users, roles, or groups</description></item>
    /// <item><description>Configure validation and hooks</description></item>
    /// </list>
    /// </param>
    /// <returns>
    /// The updated workflow state instance with the new action added to it.
    /// </returns>
    /// <remarks>
    /// When using conditional transitions with the <see cref="WorkflowAction{TData}.When"/> method in the config action,
    /// the <paramref name="nextState"/> parameter serves as the default (fallback) state. For example:
    /// <code>
    /// state.Action("Approve", "PendingManagerApproval",
    ///     action => action
    ///         .AssignToRoles("Manager")
    ///         .When(data => data.Amount > 10000, "PendingDirectorApproval")    // Evaluated first
    ///         .When(data => data.Amount > 5000, "PendingSupervisorApproval")   // Evaluated second
    ///         // If none of the above conditions are true, transitions to "PendingManagerApproval"
    /// );
    /// </code>
    /// </remarks>
    IStateBuilder<TData> Action(string name, string nextState, Action<IActionBuilder<TData>>? config = null);
}