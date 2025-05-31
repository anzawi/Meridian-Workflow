namespace Meridian.Core.Enums;
/// <summary>
/// Specifies that the hook should be executed when a state is entered in the workflow state machine.
/// </summary>
/// <remarks>
/// This member is part of the <c>StateHookType</c> enumeration and is utilized to define hooks
/// that are triggered upon the entry of a state during workflow execution.
/// </remarks>
public enum StateHookType
{
    /// <summary>
    /// Represents a hook type that is triggered when a workflow state is entered.
    /// </summary>
    /// <remarks>
    /// This hook type is used to execute logic or operations during the transition into a specific state within a workflow.
    /// Common applications include initializing state-specific resources, logging state entry events, or evaluating preconditions required for the state.
    /// </remarks>
    OnStateEnter,

    /// <summary>
    /// Represents a hook type that is triggered when a workflow state is exited.
    /// </summary>
    /// <remarks>
    /// This hook type is used to execute logic or operations as a workflow transitions out of the associated state.
    /// Common applications include releasing resources, logging state exit events, or performing state-specific cleanup tasks.
    /// </remarks>
    OnStateExit,
}