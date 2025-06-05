namespace LeaveRequestSample.Templates;

using Constants;
using Hooks.NormalHooks;
using Hooks.ResusableHooks;
using Meridian.Application.Extensions;
using Meridian.Core;
using Meridian.Core.Enums;
using Models;

/// <summary>
/// Provides workflow templates for defining and managing leave request workflows.
/// This class contains a set of reusable methods to define workflow states,
/// actions, and hooks for leave request processes.
/// </summary>
public static class LeaveRequestWorkflowTemplates
{
    // Workflow-wide hooks (e.g., on request created)
    /// <summary>
    /// Defines workflow-wide hooks for leave request workflows, such as actions to be executed
    /// when a new leave request is created.
    /// </summary>
    /// <param name="definition">
    /// The workflow definition to which the hooks should be attached. This defines the structure
    /// and behavior of the leave request workflow.
    /// </param>
    /// <returns>
    /// The modified workflow definition with the leave request hooks added.
    /// </returns>
    public static WorkflowDefinition<LeaveRequestData> WithLeaveRequestHooks(
        this WorkflowDefinition<LeaveRequestData> definition)
    {
        // NOTE: we can define HOOKs in multiple ways please read here:
        // https://github.com/anzawi/Meridian-Workflow/tree/main?tab=readme-ov-file#-hooks-event-handlers.
        return definition.AddHook(new NewLeaveRequestCreated(),
            cfg =>
            {
                cfg.LogExecutionHistory = false;
                cfg.ContinueOnFailure = false;
                cfg.IsAsync = false;
                cfg.Mode = HookExecutionMode.Sequential;
            }
        );
    }

    // Approve action template
    /// <summary>
    /// Adds an approve action to the specified workflow state for leave request processing.
    /// </summary>
    /// <param name="state">The workflow state to which the approve action will be added.</param>
    /// <param name="role">The role responsible for performing the approve action.</param>
    /// <param name="to">The next state to transition to upon successful approval.</param>
    /// <param name="notify">Specifies whether to notify the employee upon approval. Defaults to false.</param>
    /// <param name="condition">
    /// An optional function that specifies a condition under which the approve action is executed.
    /// The condition is evaluated based on the leave request data.
    /// </param>
    /// <param name="validator">
    /// An optional function that performs additional validation for the approve action.
    /// This function returns a list of validation errors, if any.
    /// </param>
    public static void AddApproveAction(
        this WorkflowState<LeaveRequestData> state,
        string role,
        string to,
        bool notify = false,
        Func<LeaveRequestData, bool>? condition = null,
        Func<LeaveRequestData, List<string>>? validator = null)
    {
        state.Action(LeaveRequestActions.Approve, to, action =>
        {
            action.AssignToRoles(role);

            if (notify)
                action.NotifyEmployee(LeaveRequestActions.Approve, false);

            if (condition != null)
                action.When(condition, to);

            if (validator != null)
                action.WithValidation(validator);
        });
    }

    // Reject action template
    /// <summary>
    /// Adds a reject action to a workflow state. This action transitions the workflow to a "Rejected" state
    /// and assigns the responsibility of rejection to the specified role.
    /// Optionally, it can send a notification to the employee upon rejection.
    /// </summary>
    /// <param name="state">The workflow state to which the reject action is added.</param>
    /// <param name="role">The role responsible for executing the reject action.</param>
    /// <param name="notify">
    /// A boolean value indicating whether to notify the employee about the rejection.
    /// If set to true, a notification will be sent upon rejection.
    /// </param>
    public static void AddRejectAction(
        this WorkflowState<LeaveRequestData> state,
        string role,
        bool notify = false)
    {
        state.Action(LeaveRequestActions.Reject, LeaveRequestStates.Rejected, action =>
        {
            action.AssignToRoles(role);

            if (notify)
                action.NotifyEmployee(LeaveRequestActions.Reject, false);
        });
    }

    // Direct manager state
    /// <summary>
    /// Configures the workflow definition to add the "Direct Manager Review" state.
    /// In this state, the workflow allows a supervisor to either approve or reject a leave request,
    /// with optional conditions and validations for approval.
    /// </summary>
    /// <param name="workflow">
    /// The workflow definition to which the "Direct Manager Review" state will be added.
    /// </param>
    /// <returns>
    /// The modified workflow definition with the added "Direct Manager Review" state.
    /// </returns>
    public static WorkflowDefinition<LeaveRequestData> WithDirectManagerReview(
        this WorkflowDefinition<LeaveRequestData> workflow)
    {
        return workflow.State(LeaveRequestStates.DirectManagerReview, state =>
        {
            state.AddApproveAction(
                role: "supervisor",
                to: LeaveRequestStates.HrReview,
                notify: false,
                condition: request => request.Days > 15,
                validator: data =>
                {
                    var errors = new List<string>();
                    if (string.IsNullOrWhiteSpace(data.Reason))
                        errors.Add("Reason is required.");
                    return errors;
                });

            state.AddRejectAction("supervisor");
        });
    }

    // Section head state
    /// <summary>
    /// Adds the section head review state to the workflow, including hooks and actions for approval or rejection.
    /// This state represents the process where the section head reviews the leave request.
    /// </summary>
    /// <param name="workflow">The workflow definition to which the section head review state will be added.</param>
    /// <returns>The workflow definition including the section head review state.</returns>
    public static WorkflowDefinition<LeaveRequestData> WithSectionHeadReview(
        this WorkflowDefinition<LeaveRequestData> workflow)
    {
        return workflow.State(LeaveRequestStates.SectionHeadReview, state =>
        {
            state.AddHook(new WorkflowHookDescriptor<LeaveRequestData>
            {
                Hook = new SendNotification(LeaveRequestStates.SectionHeadReview),
                Mode = HookExecutionMode.Parallel,
            }, StateHookType.OnStateEnter);

            state.AddApproveAction("sectionHead", LeaveRequestStates.HrReview, notify: true);
            state.AddRejectAction("sectionHead", notify: true);
        });
    }

    // HR state
    /// <summary>
    /// Configures the workflow to include an HR review state where actions can be performed by HR.
    /// </summary>
    /// <param name="workflow">
    /// The workflow definition to be configured. This object represents the current workflow.
    /// </param>
    /// <returns>
    /// The updated workflow definition with the HR review state added.
    /// </returns>
    public static WorkflowDefinition<LeaveRequestData> WithHrReview(this WorkflowDefinition<LeaveRequestData> workflow)
    {
        return workflow.State(LeaveRequestStates.HrReview, state =>
        {
            state.AddApproveAction("hr", LeaveRequestStates.Approved, notify: true);
            state.AddRejectAction("hr", notify: true);
        });
    }

    // Final states: Approved & Rejected
    /// <summary>
    /// Configures the workflow to include the final states for leave requests: Approved and Rejected.
    /// Adds necessary hooks for these states and marks their respective completion/rejection logic.
    /// </summary>
    /// <param name="workflow">
    /// The workflow definition that is to be configured with the final states.
    /// </param>
    /// <returns>
    /// The workflow definition with the configured final states.
    /// </returns>
    public static WorkflowDefinition<LeaveRequestData> WithFinalStates(
        this WorkflowDefinition<LeaveRequestData> workflow)
    {
        workflow.State(LeaveRequestStates.Rejected, state =>
        {
            state.AddHook(new WorkflowHookDescriptor<LeaveRequestData>
            {
                Hook = new SendNotification(LeaveRequestStates.Rejected),
                Mode = HookExecutionMode.Parallel,
            }, StateHookType.OnStateEnter);

            state.IsRejected();
        });

        workflow.State(LeaveRequestStates.Approved, state =>
        {
            state.AddHook(new WorkflowHookDescriptor<LeaveRequestData>
            {
                Hook = new SendNotification(LeaveRequestStates.Rejected),
                Mode = HookExecutionMode.Parallel,
            }, StateHookType.OnStateEnter);
            state.IsCompleted();
        });

        return workflow;
    }
}