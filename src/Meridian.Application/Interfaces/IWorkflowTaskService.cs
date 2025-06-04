namespace Meridian.Application.Interfaces;

using Core;
using Core.Entities;
using Core.Interfaces;

/// <summary>
/// Provides operations for managing workflow tasks within a workflow system.
/// </summary>
/// <typeparam name="TData">The type of workflow data associated with each task, constrained to implement <see cref="IWorkflowData"/>.</typeparam>
public interface IWorkflowTaskService<TData> where TData : class, IWorkflowData
{
    /// <summary>
    /// Creates tasks associated with a workflow request based on its current state
    /// and transitions defined in the workflow engine.
    /// </summary>
    /// <param name="request">
    /// An instance of <see cref="WorkflowRequestInstance{TData}"/> representing the workflow request
    /// for which tasks should be created. It contains information like the workflow definition,
    /// current state, and available actions.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation of creating and saving workflow tasks
    /// for the specified request.
    /// </returns>
    Task CreateTasksAsync(WorkflowRequestInstance<TData> request);

    /// <summary>
    /// Marks a workflow task as complete for the specified workflow request, action, and user.
    /// </summary>
    /// <param name="requestId">The unique identifier of the workflow request containing the task.</param>
    /// <param name="actionName">The name of the action associated with the task to complete.</param>
    /// <param name="userId">The identifier of the user completing the task.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CompleteTaskAsync(Guid requestId, string actionName, string userId);

    /// <summary>
    /// Retrieves a list of workflow tasks assigned to the specified user, roles, or groups
    /// that are not yet completed.
    /// </summary>
    /// <param name="userId">The ID of the user to fetch tasks for.</param>
    /// <param name="roles">A list of roles associated with the user.</param>
    /// <param name="groups">A list of groups that the user belongs to.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of workflow request tasks
    /// assigned to the user, their roles, or their groups.</returns>
    Task<List<WorkflowRequestTask>> GetUserTasksAsync(string userId, List<string> roles, List<string> groups);
}