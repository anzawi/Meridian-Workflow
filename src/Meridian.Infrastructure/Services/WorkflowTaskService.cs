namespace Meridian.Infrastructure.Services;

using Application.Interfaces;
using Core;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using DatabaseContext;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Provides services for managing workflow tasks in a workflow system. This class interacts with
/// the database context and the workflow engine registry to handle task creation, retrieval, and completion.
/// </summary>
/// <typeparam name="TData">
/// The type of the workflow data associated with tasks. This type must implement the <see cref="IWorkflowData"/> interface.
/// </typeparam>
public class WorkflowTaskService<TData>(WorkflowDbContext db, IWorkflowEngineRegistry registry)
    : IWorkflowTaskService<TData>
    where TData : class, IWorkflowData
{
    /// <inheritdoc />
    public async Task CreateTasksAsync(WorkflowRequestInstance<TData> request)
    {
        var engine = registry.ResolveTyped<TData>(request.DefinitionId);

        var state = engine.GetCurrentState(request)!;

        if (state.Type is StateType.Completed or StateType.Cancelled or StateType.Rejected)
        {
            return;
        }

        var tasks = state.Actions.Select(action => new WorkflowRequestTask
        {
            RequestId = request.Id,
            State = state.Name,
            Action = action.Name,
            AssignedToRoles = action.AssignedRoles,
            AssignedToGroups = action.AssignedGroups,
            AssignedToUsers = action.AssignedUsers,
            CreatedAt = DateTime.UtcNow,
        });

        await db.WorkflowTasks.AddRangeAsync(tasks);
        await db.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task CompleteTaskAsync(string requestId, string actionName, string userId)
    {
        var task = await db.WorkflowTasks.FirstOrDefaultAsync(t =>
            t.RequestId == requestId &&
            t.Action == actionName &&
            t.Status != WorkflowTaskStatus.Completed);

        if (task is not null)
        {
            task.Status = WorkflowTaskStatus.Completed;
            task.CompletedAt = DateTime.UtcNow;
            task.TakenByUserId = userId;
            await db.SaveChangesAsync();
        }
    }

    /// <inheritdoc />
    public async Task<List<WorkflowRequestTask>> GetUserTasksAsync(string userId, List<string> roles,
        List<string> groups)
    {
        var query = db.WorkflowTasks.AsQueryable()
            .Where(t => t.Status != WorkflowTaskStatus.Completed &&
                        (t.AssignedToUsers.Contains(userId) ||
                         roles.Any(r => t.AssignedToRoles.Contains(r)) ||
                         groups.Any(g => t.AssignedToGroups.Contains(g))));

        return await query.ToListAsync();
    }
}