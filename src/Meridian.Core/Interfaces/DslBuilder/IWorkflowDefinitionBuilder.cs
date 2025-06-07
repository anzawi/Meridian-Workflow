namespace Meridian.Core.Interfaces.DslBuilder;

using Enums;
using Hooks;
using Models;

/// <summary>
/// Provides an interface to build and configure workflow definitions for a specific data type.
/// </summary>
/// <typeparam name="TData">The type of the workflow data that implements <see cref="IWorkflowData"/>.</typeparam>
public interface
    IWorkflowDefinitionBuilder<TData> : IHookBuilder<IWorkflowDefinitionBuilder<TData>, TData, WorkflowHookType>
    where TData : class, IWorkflowData
{
    /// <summary>
    /// Overrides the default behavior for creating the initial workflow history with a specified transition.
    /// </summary>
    /// <param name="initialHistory">The initial transition to be used for creating the workflow history.</param>
    /// <returns>
    /// An instance of <see cref="IWorkflowDefinitionBuilder{TData}"/> to allow for method chaining.
    /// </returns>
    IWorkflowDefinitionBuilder<TData> OverrideOnCreateHistory(WorkflowTransition initialHistory);

    /// <summary>
    /// Defines a state in the workflow with the specified name and configuration.
    /// </summary>
    /// <param name="name">The unique name of the state being defined.</param>
    /// <param name="stateBuilder">An action to configure the state's behavior and transitions.</param>
    /// <returns>An instance of <see cref="IWorkflowDefinitionBuilder{TData}"/> to allow for chaining additional configurations.</returns>
    IWorkflowDefinitionBuilder<TData> State(string name, Action<IStateBuilder<TData>> stateBuilder);

    /// <summary>
    /// Outputs relevant workflow information to the console.
    /// </summary>
    /// <remarks>
    /// This method can be used for debugging purposes or to display
    /// workflow details during its definition or execution process.
    /// </remarks>
    void PrintToConsole();

    /// <summary>
    /// Constructs and returns a finalized workflow definition instance.
    /// </summary>
    /// <returns>
    /// A <see cref="WorkflowDefinition{TData}"/> object representing the configured workflow definition.
    /// </returns>
    WorkflowDefinition<TData> Build();
}