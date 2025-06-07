namespace Meridian.Application.Interfaces.Repositories;

using Core;
using Core.Entities;
using Core.Models;

/// <summary>
/// Represents a repository interface for managing workflow request instances and transitions.
/// Provides methods for saving, loading, and querying workflow-related data.
/// </summary>
public interface IWorkflowRepository
{
    /// <summary>
    /// Persists the specified workflow request instance to the storage. If the request instance
    /// already exists, it updates the existing entry; otherwise, it creates a new one.
    /// </summary>
    /// <param name="request">
    /// The <see cref="WorkflowRequestInstance"/> object representing the workflow request to save or update.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous operation of saving the workflow request instance.
    /// </returns>
    Task SaveAsync(WorkflowRequestInstance request);

    /// <summary>
    /// Asynchronously loads a workflow request instance by its unique identifier.
    /// </summary>
    /// <param name="requestId">The unique identifier of the workflow request instance to load.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the workflow request instance if found; otherwise, null.</returns>
    Task<WorkflowRequestInstance?> LoadAsync(Guid requestId);

    /// <summary>
    /// Retrieves all workflow request instances including their associated transitions.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of
    /// <see cref="WorkflowRequestInstance"/> objects representing all existing workflow request instances.
    /// </returns>
    Task<List<WorkflowRequestInstance>> GetAllAsync();

    /// <summary>
    /// Retrieves the history of workflow transitions associated with a specific request ID.
    /// </summary>
    /// <param name="requestId">The unique identifier of the workflow request.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="WorkflowTransition"/> objects representing the history of transitions.</returns>
    Task<List<WorkflowTransition>> GetHistoryAsync(Guid requestId);
}