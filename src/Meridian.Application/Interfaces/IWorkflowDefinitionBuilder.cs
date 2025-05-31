namespace Meridian.Application.Interfaces;

using Core;
using Core.Interfaces;

/// <summary>
/// Provides the ability to define and configure workflow definitions with a custom data type.
/// </summary>
public interface IWorkflowDefinitionBuilder
{
    /// <summary>
    /// Defines a workflow with a unique identifier and a configuration action.
    /// </summary>
    /// <typeparam name="TData">The type of the data used in the workflow, which must implement <see cref="IWorkflowData"/>.</typeparam>
    /// <param name="definitionId">The unique identifier for the workflow definition.</param>
    /// <param name="configure">An action that specifies the configuration for the workflow definition.</param>
    void Define<TData>(string definitionId, Action<WorkflowDefinition<TData>> configure)
        where TData : class, IWorkflowData;
}