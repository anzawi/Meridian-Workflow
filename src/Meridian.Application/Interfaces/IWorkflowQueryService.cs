namespace Meridian.Application.Interfaces;

using Core.Entities;

/// <summary>
/// Provides methods to query workflow request instances within the system.
/// </summary>
public interface IWorkflowQueryService
{
    /// <summary>
    /// Retrieves a list of all workflow request instances asynchronously.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of
    /// <see cref="WorkflowRequestInstance"/> objects representing all workflow request instances.
    /// </returns>
    Task<List<WorkflowRequestInstance>> GetAllAsync();

    /// <summary>
    /// Retrieves a list of workflow request instances filtered by the specified workflow definition ID.
    /// </summary>
    /// <param name="definitionId">The ID of the workflow definition used to filter the request instances.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of
    /// <see cref="WorkflowRequestInstance"/> objects associated with the specified definition ID.
    /// </returns>
    Task<List<WorkflowRequestInstance>> GetRequestsByDefinitionAsync(string definitionId);

    /// <summary>
    /// Retrieves workflow request instances matching a specific definition ID and state.
    /// </summary>
    /// <param name="definitionId">The ID of the workflow definition to filter requests by.</param>
    /// <param name="state">The state to filter workflow requests by.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of
    /// <see cref="WorkflowRequestInstance"/> objects that match the provided definition ID and state.
    /// </returns>
    Task<List<WorkflowRequestInstance>> GetRequestsInStateAsync(string definitionId, string state);

    /// <summary>
    /// Retrieves the list of workflow request instances assigned to the specified user,
    /// based on their user ID, roles, and groups, and their associated available actions.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="userRoles">The list of roles assigned to the user.</param>
    /// <param name="userGroups">The list of groups to which the user belongs.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of
    /// <see cref="WorkflowRequestInstance"/> objects assigned to the user.</returns>
    Task<List<WorkflowRequestInstance>> GetUserTasksAsync(string userId, List<string> userRoles,
        List<string> userGroups);
}