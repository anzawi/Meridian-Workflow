namespace Meridian.Infrastructure.Services;

using Application.Interfaces;
using Application.Interfaces.Repositories;
using Core.Entities;

/// <inheritdoc />
public class WorkflowQueryService : IWorkflowQueryService
{
    /// <summary>
    /// Represents the repository instance responsible for managing workflow request data and
    /// handling database operations for the WorkflowRequestInstance entities.
    /// </summary>
    private readonly IWorkflowRepository _repository;

    /// <summary>
    /// Represents a private field that holds an instance of <see cref="IWorkflowEngineRegistry"/>.
    /// This field is used to interact with a registry of workflow engines within the context of
    /// workflow operations. The registry allows resolving engines for specific workflow definitions
    /// and provides methods to manage engine registration and retrieval.
    /// </summary>
    private readonly IWorkflowEngineRegistry _registry;

    /// <summary>
    /// Provides query methods for retrieving workflow request instances and related data from the repository.
    /// Implements the <see cref="IWorkflowQueryService"/> interface.
    /// </summary>
    public WorkflowQueryService(IWorkflowRepository repository, IWorkflowEngineRegistry registry)
    {
        this._repository = repository;
        this._registry = registry;
    }

    /// <inheritdoc />
    public async Task<List<WorkflowRequestInstance>> GetAllAsync()
    {
        return await this._repository.GetAllAsync();
    }

    /// <inheritdoc />
    public async Task<List<WorkflowRequestInstance>> GetRequestsByDefinitionAsync(string definitionId)
    {
        var all = await this._repository.GetAllAsync();
        return all.Where(r => r.DefinitionId == definitionId).ToList();
    }

    /// <inheritdoc />
    public async Task<List<WorkflowRequestInstance>> GetRequestsInStateAsync(string definitionId, string state)
    {
        var all = await this._repository.GetAllAsync();
        return all.Where(r => r.DefinitionId == definitionId && r.CurrentState == state).ToList();
    }

    /// <inheritdoc />
    public async Task<List<WorkflowRequestInstance>> GetUserTasksAsync(string userId, List<string> userRoles,
        List<string> userGroups)
    {
        var all = await this._repository.GetAllAsync();

        return all.Where(r =>
        {
            if (!this._registry.Contains(r.DefinitionId))
                return false;

            var engineObj = this._registry.ResolveUntyped(r.DefinitionId);

            var method = engineObj.GetType().GetMethod("GetAvailableActions");
            if (method == null) return false;

            var actions = method.Invoke(engineObj, [r, userId, userRoles, userGroups]) as IEnumerable<object>;
            return actions?.Any() ?? false;
        }).ToList();
    }
}