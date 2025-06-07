namespace Meridian.Infrastructure.Repositories;

using Application.Interfaces.Repositories;
using Core;
using Core.Entities;
using Core.Exceptions;
using Core.Models;
using DatabaseContext;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Represents the repository responsible for managing workflow request instances and their transitions
/// within the workflow system.
/// </summary>
public class WorkflowRepository(WorkflowDbContext db) : IWorkflowRepository
{
    /// <summary>
    /// Saves the specified workflow request instance to the database asynchronously.
    /// If a request with the same identifier already exists, it updates the existing instance.
    /// </summary>
    /// <param name="request">The workflow request instance to be saved or updated.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SaveAsync(WorkflowRequestInstance request)
    {
        try
        {
            var existing = await db.Requests.Include(r => r.Transitions).FirstOrDefaultAsync(x => x.Id == request.Id);
            if (existing is null)
                db.Requests.Add(request);
            else
                db.Entry(existing).CurrentValues.SetValues(request);

            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new WorkflowPersistenceException(
                request.Id.ToString(),
                "Database update failed",
                ex);
        }
        catch (Exception ex)
        {
            throw new WorkflowPersistenceException(
                request.Id.ToString(),
                "Unexpected error during save operation",
                ex);
        }
    }

    /// <summary>
    /// Asynchronously loads a WorkflowRequestInstance by the specified request ID.
    /// The method includes related transitions as part of the returned instance.
    /// </summary>
    /// <param name="requestId">The unique identifier of the workflow request to load.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// WorkflowRequestInstance if found; otherwise, null.
    /// </returns>
    public async Task<WorkflowRequestInstance?> LoadAsync(Guid requestId)
    {
        return await db.Requests.Include(r => r.Transitions).FirstOrDefaultAsync(x => x.Id == requestId);
    }

    /// <summary>
    /// Asynchronously retrieves all workflow request instances along with their associated transitions.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation, containing a list of <see cref="WorkflowRequestInstance"/>.
    /// </returns>
    public async Task<List<WorkflowRequestInstance>> GetAllAsync()
    {
        return await db.Requests.Include(r => r.Transitions).ToListAsync();
    }

    /// <summary>
    /// Retrieves the history of workflow transitions for a given request.
    /// </summary>
    /// <param name="requestId">The unique identifier of the request whose transitions are to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="WorkflowTransition"/> objects, representing the history of transitions, ordered by timestamp in descending order. If no transitions exist, an empty list is returned.</returns>
    public async Task<List<WorkflowTransition>> GetHistoryAsync(Guid requestId)
    {
        var request = await db.Requests.Include(r => r.Transitions).FirstOrDefaultAsync(r => r.Id == requestId);
        return request?.Transitions.OrderByDescending(t => t.Timestamp).ToList() ?? [];
    }
}