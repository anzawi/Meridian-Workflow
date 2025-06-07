namespace Meridian.Infrastructure.Services;

using System.ComponentModel.DataAnnotations;
using Application.Interfaces;
using Core;
using Core.Contexts;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Helpers;

/// <summary>
/// The WorkflowEngine class is responsible for managing and executing workflows based on a
/// predefined workflow definition. It provides methods for creating new workflow instances,
/// executing actions, and retrieving available actions and states.
/// </summary>
/// <typeparam name="TData">
/// The type of data that the workflow operates on. Must implement the IWorkflowData interface.
/// </typeparam>
public class WorkflowEngine<TData>(WorkflowDefinition<TData> definition) : IWorkflowEngine<TData>
    where TData : class, IWorkflowData
{
    /// <summary>
    /// Represents the workflow definition that outlines the structure, states, actions,
    /// and behaviors of a workflow. It serves as the blueprint for workflow execution
    /// within the <see cref="WorkflowEngine{TData}"/>.
    /// </summary>
    private readonly WorkflowDefinition<TData> _definition = definition;

    /// <inheritdoc />
    public string DefinitionId => this._definition.Name;

    /// <inheritdoc />
    public WorkflowDefinition<TData> GetDefinition() => this._definition;

    /// <inheritdoc />
    public async Task CreateAsync(WorkflowRequestInstance<TData> request)
    {
        request.CurrentState = this._definition.States.FirstOrDefault(state => state.Type is StateType.Start)?.Name 
                               ?? throw new WorkflowStateException(this._definition.Name, "Start", "No Start state defined, Use state.IsStarted()");


        var ctx = new WorkflowContext<TData> { Request = request, History = request.Transitions };

        // execute hooks for on create new request
        await HookExecutor.ExecuteAll(this._definition.OnCreateHooks, ctx);

        request.Transitions.Add(this._definition.GetOnCreateHistory());

        await this.AutoExecuteNextAsync(request, performedBy: "system", userRoles: [], userGroups: []);
    }

    /// <inheritdoc />
    public async Task<WorkflowRequestInstance<TData>> ExecuteActionAsync(WorkflowRequestInstance<TData> request,
        string actionName, string performedBy,
        TData? data, List<string> userRoles, List<string> userGroups)
    {
        var currentState = this._definition.States.FirstOrDefault(s => s.Name == request.CurrentState)
                           ?? throw new WorkflowStateException(this._definition.Name, request.CurrentState!, 
                               "Current state not found in workflow definition");

        var action = currentState.Actions.FirstOrDefault(a => a.Name == actionName)
                     ?? throw new WorkflowActionException(this._definition.Name, currentState.Name, actionName,
                         "Action not found in current state");
        
        data ??= request.Data ?? Activator.CreateInstance<TData>();
        var errors = ValidateUserInput(action, data);
        if (!string.IsNullOrEmpty(errors))
        {
            throw new ValidationException(errors);
        }
        var ctx = new WorkflowContext<TData>
        {
            Request = request,
            UserId = performedBy,
            InputData = data,
            UserRoles = userRoles,
            UserGroups = userGroups,
            History = request.Transitions
        };

        if (!action.IsAuthorized(performedBy, userRoles, userGroups))
            throw new WorkflowAuthorizationException(performedBy, action.AssignedRoles, action.AssignedGroups);

        // execute hooks for on-exit the current state
        await HookExecutor.ExecuteAll(currentState.OnExitHooks, ctx);

        // execute hooks on the taken action
        await HookExecutor.ExecuteAll(action.OnExecuteHooks, ctx);


        var nextState = this._definition.States.First(s => s.Name == action.NextState);
        // execute hooks for on-enter the next state
        await HookExecutor.ExecuteAll<TData>(nextState.OnEnterHooks, ctx);

        ctx.History.Add(new WorkflowTransition
        {
            FromState = request.CurrentState,
            ToState = action.NextState,
            Action = action.Name,
            PerformedBy = performedBy,
            Metadata = DataConverter.Serialize(data),
            Timestamp = DateTime.UtcNow
        });

        // execute global hooks when request move from state to other
        await HookExecutor.ExecuteAll(this._definition.OnTransitionHooks, ctx);

        request.CurrentState = action.ResolveNextState(data);
        request.Data = data;
        request.Transitions = ctx.History;

        await this.AutoExecuteNextAsync(request, performedBy, userRoles, userGroups);
        return request;
    }

    /// <inheritdoc />
    public List<WorkflowAction<TData>> GetAvailableActions(WorkflowRequestInstance<TData> request,
        string? userId = null,
        List<string>? userRoles = null, List<string>? userGroups = null)
    {
        var state = this._definition.States.First(s => s.Name == request.CurrentState);
        return state.Actions
            .Where(a => a.IsAuthorized(userId, userRoles, userGroups))
            .ToList();
    }

    /// <inheritdoc />
    public WorkflowState<TData> GetCurrentState(WorkflowRequestInstance<TData> request)
        => this._definition.States.FirstOrDefault(s => s.Name == request.CurrentState)
           ?? throw new KeyNotFoundException(nameof(request.CurrentState));

    /// <summary>
    /// Automatically executes the next action in the workflow if certain conditions are met.
    /// </summary>
    /// <param name="request">The workflow request instance containing the current state and data.</param>
    /// <param name="performedBy">The identifier of the performer executing the action, such as a system or user.</param>
    /// <param name="userRoles">The list of user roles associated with the performer, used for validating actions.</param>
    /// <param name="userGroups">The list of user groups associated with the performer, used for validating actions.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task AutoExecuteNextAsync(
        WorkflowRequestInstance<TData> request,
        string performedBy,
        List<string> userRoles,
        List<string> userGroups)
    {
        var state = this._definition.States.FirstOrDefault(s => s.Name == request.CurrentState);
        if (state == null) return;


        foreach (var action in state.Actions.Where(a => a.IsAuto))
        {
            var requestData = request.Data ?? Activator.CreateInstance<TData>();
            if (action.Condition?.Invoke(requestData) != true) continue;

            await this.ExecuteActionAsync(request, action.Name, performedBy, requestData, userRoles, userGroups);
            break; // stop after first valid auto-action
        }
    }

    /// <summary>
    /// Validates the user-provided input data against both standard data annotations
    /// and action-specific custom validation rules.
    /// </summary>
    /// <param name="action">The workflow action specifying the custom validation rules.</param>
    /// <param name="data">The user-provided input data to be validated.</param>
    /// <exception cref="ValidationException">
    /// Thrown when the input data fails standard validation or custom validation rules.
    /// </exception>
    private static string? ValidateUserInput(WorkflowAction<TData> action, TData data)
    {
        var context = new ValidationContext(data);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(data, context, results, validateAllProperties: true);

        if (!isValid)
            return string.Join("; ", results.Select(r => r.ErrorMessage));

        if (action.ValidateInput is not null)
        {
            var customErrors = action.ValidateInput(data);
            if (customErrors.Count > 0)
                return string.Join("; ", customErrors);
        }

        return null;
    }
}