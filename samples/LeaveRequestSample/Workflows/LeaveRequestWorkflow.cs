namespace LeaveRequestSample.Workflows;

using Constants;
using Hooks.NormalHooks;
using Hooks.ResusableHooks;
using Meridian.Application.Extensions;
using Meridian.Application.Interfaces;
using Meridian.Core;
using Meridian.Core.Dtos;
using Meridian.Core.Enums;
using Models;
using Models.Enums;

// In this class you will see the workflow definition twice!
// First one (Commented out) using the TemplateExtensions feature -clearly its simplify the workflow definition and make it more clearer.
// Second one (Uncommented) using the simple way of fluent API - this is the way to define the workflow.

// You can comment one and uncomment the other -- There are no difference between them -- both will work as same.
// Here we just show you examples of both ways, you can use the one you like the most.

// And you can see in both, there are examples to add hooks in direct way or using reusable hooks templates (find them in the Hooks folder).

/// <summary>
/// Represents the workflow for handling leave requests within the system.
/// </summary>
/// <remarks>
/// This class defines the steps and structure of the leave request workflow.
/// It implements the <see cref="IWorkflowBootstrapper"/> interface, which enables
/// the registration of workflow definition within the system.
/// </remarks>
public sealed class LeaveRequestWorkflow : IWorkflowBootstrapper
{
    /*public void Register(IWorkflowDefinitionBuilder builder)
    {
        builder.Define<LeaveRequestData>("LeaveRequestWorkflow", workflow =>
        {
            workflow.WithLeaveRequestHooks();
            workflow
                .WithDirectManagerReview()
                .WithSectionHeadReview()
                .WithHrReview()
                .WithFinalStates()
                .PrintToConsole();
        });
    }*/

    /// <summary>
    /// Registers a workflow definition with the specified workflow builder.
    /// </summary>
    /// <param name="builder">
    /// The workflow definition builder used to define and configure the workflow.
    /// </param>
    public void Register(IWorkflowDefinitionBuilder builder)
    {
        builder.Define<LeaveRequestData>("LeaveRequestWorkflow", workflow =>
        {
            // NOTE: we can define HOOKs in multiple ways please read here:
            // https://github.com/anzawi/Meridian-Workflow/tree/main?tab=readme-ov-file#-hooks-event-handlers.

            // Add a hook to the workflow in general (type after request created)
            // this example show you the direct way to add hooks.
            // the second parameter is optional, you can use any other type. its WorkflowHookType.OnRequestCreated by default
            workflow.AddHook(new WorkflowHookDescriptor<LeaveRequestData>
            {
                LogExecutionHistory = false, // Dont log this execution in the request history -- true by default
                ContinueOnFailure = false, // Continue the workflow if the hook failed -- false by default
                IsAsync = false, // Is the hook async -- false by default
                Mode = HookExecutionMode.Sequential, // Hook Mode Sequential or parallel -- Sequential by default
                // here the functionality
                Hook = new NewLeaveRequestCreated(),
            }, WorkflowHookType.OnRequestCreated);
            
            // The same hook abovecan be added like this also:
            /*
             workflow.AddHook(new NewLeaveRequestCreated(),
               cfg =>
               {
                   cfg.LogExecutionHistory = false; // Dont log this execution in the request history -- true by default
                   cfg.ContinueOnFailure = false; // Continue the workflow if the hook failed -- false by default
                   cfg.IsAsync = false; // Is the hook async -- false by default
                   cfg.Mode = HookExecutionMode
                       .Sequential; // Hook Mode Sequential or parallel -- Sequential by default
               }, WorkflowHookType.OnRequestCreated);
             */


            // Define the start state, the request after submit will be in Under direct manager review state
            workflow.State(LeaveRequestStates.DirectManagerReview, state =>
            {
                // the direct manager can approve the request
                state.Action(LeaveRequestActions.Approve, LeaveRequestStates.HrReview, action =>
                {
                    action.AssignToRoles("supervisor");
                    // You can assign to users and groups in the same time.
                    // action.AssignToRoles("supervisor", "other role");
                    // action.AssignToUsers("supervisor", "otherUser");
                    //action.AssignedGroups("group1", "group2");;
                    // We don't need to add other conditions here, because if the condition is not met, the request will moved to the next state "LeaveRequestStates.HrReview"
                    action.When(request => request.Days > 15, LeaveRequestStates.SectionHeadReview);

                    // This validation will be executed while the action execution,
                    // validate the id the leave reason is not empty
                    // validate if the leave is sick leave then the medical report attachment must be provided
                    action.WithValidation(data =>
                    {
                        var errors = new List<string>();

                        if (string.IsNullOrWhiteSpace(data.Reason))
                            errors.Add("Reason is required.");

                        if (data is { LeaveType: LeaveTypes.Sick, MedicalReport: null })
                            errors.Add("Medical Report is required for sick leave.");

                        return errors;
                    });
                });

                // the direct manager can reject the request
                state.Action(LeaveRequestActions.Reject, LeaveRequestStates.Rejected,
                    action => { action.AssignToRoles("supervisor"); });
            });

            // The request after direct manager review will be in Section head review state if the leave more than 15 days.
            workflow.State(LeaveRequestStates.SectionHeadReview, state =>
            {
                // Here we add hook to send notification in direct way.
                state.AddHook(new SendNotification(LeaveRequestStates.SectionHeadReview),
                    cfg => { cfg.Mode = HookExecutionMode.Parallel; }, StateHookType.OnStateEnter);
                // the section head can approve the request
                state.Action(LeaveRequestActions.Approve, LeaveRequestStates.HrReview, action =>
                {
                    action.AssignToRoles("sectionHead");
                    // Here we adding hooks using the template.
                    action.NotifyEmployee(LeaveRequestActions.Approve, false);
                });

                // the section head can reject the request
                state.Action(LeaveRequestActions.Reject, LeaveRequestStates.Rejected,
                    action => { action.AssignToRoles("sectionHead"); });
            });

            // The request after direct manager approval and the days less than or equal to 15 days will be in HR review state.
            // The request section head review will be in HR review state.
            workflow.State(LeaveRequestStates.HrReview, state =>
            {
                // the HR can approve the request
                state.Action(LeaveRequestActions.Approve, LeaveRequestStates.Approved, action =>
                {
                    action.AssignToRoles("hr");
                    action.NotifyEmployee(LeaveRequestActions.Approve, false);
                });

                // the HR can reject the request
                state.Action(LeaveRequestActions.Reject, LeaveRequestStates.Rejected, action =>
                {
                    action.AssignToRoles("hr");
                    action.NotifyEmployee(LeaveRequestActions.Reject, false);
                });
            });

            // The request after any reject action will be in Rejected state and marked as rejected.
            workflow.State(LeaveRequestStates.Rejected, state =>
            {
                state.AddHook(new SendNotification(LeaveRequestStates.Rejected),
                    cfg => { cfg.Mode = HookExecutionMode.Parallel; }, StateHookType.OnStateEnter);
                state.IsRejected();
            });
            // The request after HR approval will be in Approved state if the request is approved and marked as completed.
            workflow.State(LeaveRequestStates.Approved, state =>
            {
                state.AddHook(new SendNotification(LeaveRequestStates.Rejected),
                    cfg => { cfg.Mode = HookExecutionMode.Parallel; }, StateHookType.OnStateEnter);
                state.IsCompleted();
            });

            // Print the workflow to console
            workflow.PrintToConsole();
        });
    }
}
