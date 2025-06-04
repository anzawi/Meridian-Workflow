namespace Meridian.Application.Interfaces;

using Core;
using Core.Interfaces;
using Meridian.Application.DTOs;

/// <summary>
/// Provides workflow management and execution functionalities for workflows that utilize a specific
/// type of workflow data. This interface serves as a contract for handling workflow-related operations
/// such as creating workflow requests, performing workflow actions, retrieving workflow instances,
/// and obtaining workflow state or history.
/// </summary>
/// <typeparam name="TData">
/// The type of data associated with a workflow. Must implement the <see cref="IWorkflowData"/> interface.
/// </typeparam>
public interface IWorkflowService<TData> where TData : class, IWorkflowData
{
    /// <summary>
    /// Creates a new workflow request instance with the specified definition and input data.
    /// </summary>
    /// <param name="inputData">The input data associated with the workflow request.</param>
    /// <param name="createdBy">The identifier of the user initiating the request.</param>
    /// <returns>A task representing the asynchronous operation, containing the created workflow request instance.</returns>
    Task<WorkflowRequestInstance<TData>> CreateRequestAsync(TData inputData, string createdBy);

    /// <summary>
    /// Executes a specified action on a workflow request instance.
    /// </summary>
    /// <param name="requestId">The unique identifier of the workflow request.</param>
    /// <param name="action">The action to be performed on the workflow request.</param>
    /// <param name="performedBy">The identifier of the user performing the action.</param>
    /// <param name="data">The data associated with the workflow request to update its state.</param>
    /// <param name="userRoles">A list of roles assigned to the user performing the action.</param>
    /// <param name="userGroups">A list of groups to which the user performing the action belongs.</param>
    /// <returns>The updated workflow request instance after performing the action.</returns>
    Task<WorkflowRequestInstance<TData>> DoActionAsync(Guid requestId, string action, string performedBy,
        List<string> userRoles, List<string> userGroups, TData? data = null);

    /// <summary>
    /// Retrieves a workflow request instance based on the provided request ID.
    /// </summary>
    /// <param name="requestId">The unique identifier of the workflow request to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// the <see cref="WorkflowRequestInstance{TData}"/> if found, or null if no matching request exists.
    /// </returns>
    Task<WorkflowRequestInstance<TData>?> GetRequestAsync(Guid requestId);

    /// <summary>
    /// Retrieves a list of workflow request instances assigned to the specified user.
    /// The tasks are determined based on the user's roles and groups, as well as available actions.
    /// </summary>
    /// <param name="userId">The ID of the user whose tasks need to be retrieved.</param>
    /// <param name="userRoles">A list of roles associated with the user for filtering tasks.</param>
    /// <param name="userGroups">A list of groups associated with the user for filtering tasks.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of workflow request instances assigned to the user.</returns>
    Task<List<WorkflowRequestInstance<TData>>> GetUserTasksAsync(string userId, List<string> userRoles,
        List<string> userGroups);

    /// <summary>
    /// Retrieves the list of available actions for a specific workflow request instance,
    /// optionally filtered by the user ID, their roles, or groups.
    /// </summary>
    /// <param name="request">The workflow request instance for which available actions are being retrieved.</param>
    /// <param name="userId">The optional ID of the user requesting the actions.</param>
    /// <param name="userRoles">The optional list of role identifiers for the user.</param>
    /// <param name="userGroups">The optional list of group identifiers for the user.</param>
    /// <returns>A list of <see cref="WorkflowAction{TData}"/> objects representing the available actions for the given workflow request.</returns>
    List<WorkflowActionDto> GetAvailableActions(WorkflowRequestInstance<TData> request, string? userId = null,
        List<string>? userRoles = null, List<string>? userGroups = null);

    /// <summary>
    /// Retrieves the list of available actions for a specific request by ID,
    /// optionally filtered by the user ID, their roles, or groups.
    /// </summary>
    /// <param name="requestId">The workflow request Id.</param>
    /// <param name="userId">The optional ID of the user requesting the actions.</param>
    /// <param name="userRoles">The optional list of role identifiers for the user.</param>
    /// <param name="userGroups">The optional list of group identifiers for the user.</param>
    /// <returns>A list of <see cref="WorkflowAction{TData}"/> objects representing the available actions for the given workflow request.</returns>
    Task<List<WorkflowActionDto>> GetAvailableActions(Guid requestId,
        string? userId = null, List<string>? userRoles = null, List<string>? userGroups = null);

    /// <summary>
    /// Retrieves the current state of the specified workflow request instance.
    /// </summary>
    /// <param name="request">The workflow request instance for which the current state is to be determined.</param>
    /// <returns>The current state of the specified workflow request instance as a <see cref="WorkflowState{TData}"/> object.</returns>
    WorkflowState<TData> GetCurrentState(WorkflowRequestInstance<TData> request);

    /// <summary>
    /// Retrieves the history of transitions for a specific workflow request identified by its request ID.
    /// </summary>
    /// <param name="requestId">The unique identifier of the workflow request for which the history is to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="WorkflowTransition"/> objects representing the transition history of the workflow request.</returns>
    Task<List<WorkflowTransition>> GetRequestHistoryAsync(Guid requestId);

    /// <summary>
    /// Retrieves a workflow request along with its history using the specified request ID.
    /// </summary>
    /// <param name="requestId">The unique identifier of the workflow request.</param>
    /// <returns>A <see cref="WorkflowWithHistoryDto{TData}"/> instance containing the workflow request and its history.</returns>
    Task<WorkflowWithHistoryDto<TData>> GetRequestWithHistoryAsync(Guid requestId);
}