namespace Meridian.Application.Interfaces;

using Core;
using Core.Interfaces;
using Core.Models;

/// <summary>
/// Represents the core engine for processing workflows of a specific type of data.
/// This interface defines methods for managing workflow definitions, creating workflow instances,
/// executing workflow actions, retrieving current states, and determining allowed actions based on specified criteria.
/// </summary>
/// <typeparam name="TData">The type of the workflow data, which must implement the <see cref="IWorkflowData"/> interface.</typeparam>
public interface IWorkflowEngine<TData> where TData : class, IWorkflowData
{
    /// <summary>
    /// Gets the identifier of the workflow definition associated with the workflow engine or request instance.
    /// </summary>
    /// <remarks>
    /// This property represents the unique identifier of the workflow definition
    /// that governs the structure, states, and transitions of the workflow.
    /// It is typically used to link workflow requests with their corresponding definitions.
    /// </remarks>
    string DefinitionId { get; }

    /// <summary>
    /// Retrieves the workflow definition associated with the engine.
    /// </summary>
    /// <returns>
    /// A <see cref="WorkflowDefinition{TData}"/> representing the definition of the workflow for the specified data type.
    /// </returns>
    WorkflowDefinition<TData> GetDefinition();

    /// <summary>
    /// Creates a new workflow request and initializes it with the appropriate state, transitions,
    /// and executes any defined hooks during the creation process.
    /// </summary>
    /// <param name="request">The workflow request instance containing the data and metadata for the workflow initialization.</param>
    /// <returns>A task representing the asynchronous operation of initializing the workflow request.</returns>
    Task CreateAsync(WorkflowRequestInstance<TData> request);

    /// <summary>
    /// Executes a specific action within the workflow for the given request instance.
    /// The action is performed by a specified user with associated roles and groups.
    /// </summary>
    /// <param name="request">The workflow request instance on which the action is to be performed.</param>
    /// <param name="actionName">The name of the action to be executed.</param>
    /// <param name="performedBy">The identifier of the user performing the action.</param>
    /// <param name="data">The details of the workflow data to be associated with the request.</param>
    /// <param name="userRoles">The list of roles associated with the user performing the action.</param>
    /// <param name="userGroups">The list of groups associated with the user performing the action.</param>
    /// <returns>A task that represents the operation, returning the updated workflow request instance upon completion.</returns>
    Task<WorkflowRequestInstance<TData>> ExecuteActionAsync(WorkflowRequestInstance<TData> request,
        string actionName,
        string performedBy,
        TData? data,
        List<string> userRoles,
        List<string> userGroups);

    /// <summary>
    /// Retrieves the list of available workflow actions for the specified request instance,
    /// filtered by user permissions (userId, roles, and groups).
    /// </summary>
    /// <param name="request">The workflow request instance for which actions are being retrieved.</param>
    /// <param name="userId">The optional user identifier to filter actions by authorization.</param>
    /// <param name="userRoles">A list of optional roles associated with the user to filter actions by authorization.</param>
    /// <param name="userGroups">A list of optional groups associated with the user to filter actions by authorization.</param>
    /// <returns>
    /// A list of available workflow actions that the specified user (or user roles/groups) is authorized to perform.
    /// </returns>
    List<WorkflowAction<TData>> GetAvailableActions(
        WorkflowRequestInstance<TData> request,
        string? userId = null,
        List<string>? userRoles = null,
        List<string>? userGroups = null
    );

    /// <summary>
    /// Retrieves the current state of the specified workflow request instance.
    /// </summary>
    /// <param name="request">The workflow request instance whose current state is to be determined. It contains information about the workflow instance, including its current state.</param>
    /// <returns>The <see cref="WorkflowState{TData}"/> representing the current state of the workflow request instance.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the current state of the request cannot be found in the workflow definition.</exception>
    WorkflowState<TData> GetCurrentState(WorkflowRequestInstance<TData> request);
}