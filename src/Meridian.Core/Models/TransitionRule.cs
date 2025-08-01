namespace Meridian.Core.Models;

/// <summary>
/// Represents a rule for transitioning between states in a workflow based on a condition.
/// This record encapsulates the logic and metadata required to determine the next state
/// when certain criteria are met during the execution of a workflow.
/// </summary>
/// <typeparam name="TData">
/// The type of the data object associated with the workflow.
/// This type is used to evaluate the condition and must provide the necessary structure
/// for determining the validity of the transition.
/// </typeparam>
internal sealed record TransitionRule<TData>(
    Func<TData, bool> Condition,
    string TargetState,
    // For debugging purposes
    string Label);
