namespace Meridian.Infrastructure.Services;

using Application.DTOs;
using Application.Extensions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Core;
using Core.Dtos;
using Core.Extensions;
using Core.Interfaces;
using Core.Models;
using Helpers;

/// <summary>
/// The WorkflowService class provides functionality to manage and execute
/// workflow processes for a given data type. It serves as an implementation
/// of the IWorkflowService interface, integrating with various components
/// like workflow engines, repositories, file storage providers, and task services.
/// </summary>
/// <typeparam name="TData">
/// Represents the type of data being processed in the workflow.
/// The type must implement the IWorkflowData interface.
/// </typeparam>
public class WorkflowService<TData>(
    IWorkflowEngine<TData> engine,
    IWorkflowRepository repository,
    IWorkflowFileStorageProviderFactory storageProvider,
    IWorkflowTaskService<TData> taskService)
    : IWorkflowService<TData>
    where TData : class, IWorkflowData
{
    /// <inheritdoc />
    public async Task<WorkflowRequestInstance<TData>> CreateRequestAsync(TData inputData,
        string createdBy)
    {
        var data = await WorkflowAttachmentProcessor.ProcessAttachmentsAsync(inputData, storageProvider);

        var request = new WorkflowRequestInstance<TData>
        {
            DefinitionId = engine.DefinitionId,
            Data = data,
            CurrentState = string.Empty
        };
        await engine.CreateAsync(request);
        await repository.SaveAsync(request.ToUntyped()!);

        await taskService.CreateTasksAsync(request);

        return request;
    }

    /// <inheritdoc />
    public async Task<WorkflowRequestInstance<TData>> DoActionAsync(Guid requestId, string action, string performedBy,
        List<string> userRoles, List<string> userGroups, TData? data = null)
    {
        var request = (await repository.LoadAsync(requestId)).ToTyped<TData>()
                      ?? throw new InvalidOperationException("Request not found");

        var updatedRequest = await engine.ExecuteActionAsync(request, action, performedBy, data, userRoles, userGroups);
        await repository.SaveAsync(updatedRequest.ToUntyped()!);

        await taskService.CompleteTaskAsync(request.Id, action, performedBy);
        await taskService.CreateTasksAsync(request);
        return request;
    }

    /// <inheritdoc />
    public async Task<WorkflowRequestInstance<TData>?> GetRequestAsync(Guid requestId)
    {
        var request = await repository.LoadAsync(requestId);
        return request.ToTyped<TData>();
    }

    /// <inheritdoc />
    public async Task<List<WorkflowRequestInstance<TData>>> GetUserTasksAsync(string userId, List<string> userRoles,
        List<string> userGroups)
    {
        var all = await repository.GetAllAsync();
        var tasks = all.Where(r => engine.GetAvailableActions(r.ToTyped<TData>()!, userId, userRoles, userGroups).Any())
            .ToList();
        return tasks.ToTyped<TData>();
    }

    /// <inheritdoc />
    public List<WorkflowActionDto> GetAvailableActions(WorkflowRequestInstance<TData> request,
        string? userId = null, List<string>? userRoles = null, List<string>? userGroups = null)
        => engine.GetAvailableActions(request, userId, userRoles, userGroups).ToActionDto();

    /// <inheritdoc />
    public async Task<List<WorkflowActionDto>> GetAvailableActions(Guid requestId,
        string? userId = null, List<string>? userRoles = null, List<string>? userGroups = null)
    {
        var request = await this.GetRequestAsync(requestId);
        if (request is null) return [];
        return engine.GetAvailableActions(request, userId, userRoles, userGroups).ToActionDto();;
    }

    /// <inheritdoc />
    public WorkflowState<TData> GetCurrentState(WorkflowRequestInstance<TData> request) =>
        engine.GetCurrentState(request);

    /// <inheritdoc />
    public Task<List<WorkflowTransition>> GetRequestHistoryAsync(Guid requestId)
        => repository.GetHistoryAsync(requestId);

    /// <inheritdoc />
    public async Task<WorkflowWithHistoryDto<TData>> GetRequestWithHistoryAsync(Guid requestId)
    {
        var request = await repository.LoadAsync(requestId) ?? throw new InvalidOperationException("Not found");
        var history = await repository.GetHistoryAsync(requestId);

        return new WorkflowWithHistoryDto<TData>
        {
            Request = request.ToTyped<TData>()!,
            History = history
        };
    }
}