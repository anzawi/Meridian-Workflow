namespace Meridian.Application.Extensions;

using System.Text.Json;
using Core;
using Core.Entities;
using Core.Interfaces;

/// <summary>
/// Provides extension methods for working with WorkflowRequestInstance objects, allowing conversions between
/// typed and untyped representations as well as handling collections of WorkflowRequestInstances.
/// </summary>
public static class WorkflowRequestExtensions
{
    /// <summary>
    /// Converts an untyped <see cref="WorkflowRequestInstance"/> into a typed <see cref="WorkflowRequestInstance{TData}"/>.
    /// </summary>
    /// <typeparam name="TData">
    /// The type of the workflow data implementing <see cref="IWorkflowData"/>.
    /// </typeparam>
    /// <param name="request">
    /// The untyped <see cref="WorkflowRequestInstance"/> to be converted. Can be null.
    /// </param>
    /// <returns>
    /// A typed <see cref="WorkflowRequestInstance{TData}"/> containing the deserialized data and other properties from the input object,
    /// or null if the input object is null.
    /// </returns>
    public static WorkflowRequestInstance<TData>? ToTyped<TData>(this WorkflowRequestInstance? request)
        where TData : class, IWorkflowData
    {
        if (request == null)
        {
            return null;
        }

        if (string.IsNullOrEmpty(request.DataJson))
        {
            request.DataJson = "{}";
        }

        var typedData = request.DataJson.Equals("{}") ? null : JsonSerializer.Deserialize<TData>(request.DataJson)
                        ?? throw new InvalidOperationException($"Cannot deserialize data to {typeof(TData).Name}");

        return new WorkflowRequestInstance<TData>
        {
            Id = request.Id,
            DefinitionId = request.DefinitionId,
            CurrentState = request.CurrentState,
            Data = typedData,
            Status = request.Status,
            Transitions = request.Transitions,
        };
    }

    /// <summary>
    /// Converts a typed <see cref="WorkflowRequestInstance{TData}"/> into an untyped <see cref="WorkflowRequestInstance"/>.
    /// </summary>
    /// <typeparam name="TData">
    /// The type of the workflow data implementing <see cref="IWorkflowData"/>.
    /// </typeparam>
    /// <param name="typedRequest">
    /// The typed <see cref="WorkflowRequestInstance{TData}"/> to be converted. Can be null.
    /// </param>
    /// <returns>
    /// An untyped <see cref="WorkflowRequestInstance"/> equivalent to the input object, or null if the input is null.
    /// </returns>
    public static WorkflowRequestInstance? ToUntyped<TData>(this WorkflowRequestInstance<TData>? typedRequest)
        where TData : class, IWorkflowData
    {
        if (typedRequest == null) return null;
        return new WorkflowRequestInstance
        {
            Id = typedRequest.Id,
            DefinitionId = typedRequest.DefinitionId,
            CurrentState = typedRequest.CurrentState,
            DataJson = typedRequest.Data is null ? "{}" :JsonSerializer.Serialize(typedRequest.Data),
            Status = typedRequest.Status,
            Transitions = typedRequest.Transitions,
        };
    }

    /// <summary>
    /// Converts an untyped <see cref="WorkflowRequestInstance"/> to a strongly-typed <see cref="WorkflowRequestInstance{TData}"/> object.
    /// </summary>
    /// <typeparam name="TData">The type of the workflow data that implements <see cref="IWorkflowData"/>.</typeparam>
    /// <param name="requests">The untyped workflow request to be converted.</param>
    /// <returns>A strongly-typed <see cref="WorkflowRequestInstance{TData}"/> object if the conversion is successful; otherwise, null.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the data cannot be deserialized to the specified type.</exception>
    public static List<WorkflowRequestInstance<TData>> ToTyped<TData>(this List<WorkflowRequestInstance> requests)
        where TData : class, IWorkflowData
    {
        return requests.Count == 0 ? [] : requests.Select(request => request.ToTyped<TData>()!).ToList();
    }
}