namespace Meridian.Core.Enums;

/// <summary>
/// Represents a workflow hook type that is triggered when a request is created.
/// </summary>
/// <remarks>
/// This hook type is intended for operations or logic that should be executed immediately
/// after the creation of a request within the workflow system.
/// </remarks>
public enum WorkflowHookType
{
    /// <summary>
    /// Defines the workflow hook type that is executed immediately after a request is created in the workflow system.
    /// </summary>
    /// <remarks>
    /// Designed for implementing logic or operations triggered upon the creation of a workflow request.
    /// Ideal for tasks such as initializing properties, generating logs, or sending notifications
    /// when a request is established in the system.
    /// </remarks>
    OnRequestCreated,

    /// <summary>
    /// Represents the workflow hook type that is executed during a transition between states in a workflow.
    /// </summary>
    /// <remarks>
    /// Designed for implementing logic or operations that are triggered when a workflow request undergoes a state transition.
    /// Common usages include validating transition conditions, updating state-specific data, and logging state changes.
    /// </remarks>
    OnRequestTransition
}