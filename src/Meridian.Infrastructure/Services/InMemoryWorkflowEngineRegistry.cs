namespace Meridian.Infrastructure.Services;

using Application.Interfaces;
using Core.Exceptions;
using Core.Interfaces;

/// <summary>
/// Provides an in-memory implementation of the <see cref="IWorkflowEngineRegistry"/> interface for managing workflow engines.
/// </summary>
/// <remarks>
/// This class allows for the registration and retrieval of workflow engines based on their associated workflow definition IDs.
/// The registry stores both the workflow engine and its associated data type, enabling type-safe resolution of engines.
/// </remarks>
/// <threadsafety>
/// This implementation is not thread-safe. If used concurrently, synchronization mechanisms should be implemented externally.
/// </threadsafety>
public class InMemoryWorkflowEngineRegistry : IWorkflowEngineRegistry
{
    /// <summary>
    /// A dictionary that acts as an in-memory storage for workflow engines and their associated data types.
    /// The key is a string representing the definition ID of the workflow engine,
    /// and the value is a tuple containing the engine instance and the associated data type.
    /// </summary>
    private readonly Dictionary<string, (object Engine, Type DataType)> _engines = new();
    
    /// <inheritdoc />
    public void Register<TData>(string definitionId, IWorkflowEngine<TData> engine)
        where TData : class, IWorkflowData
    {
        if (string.IsNullOrWhiteSpace(definitionId))
            throw new ArgumentException("Definition ID cannot be null or empty.", nameof(definitionId));

        ArgumentNullException.ThrowIfNull(engine);
        
        if (this.Contains(definitionId))
        {
            throw new WorkflowRegistryException(
                definitionId,
                typeof(TData),
                "A workflow definition with this ID is already registered");
        }


        this._engines[definitionId] = (engine, typeof(TData));
    }

    /// <inheritdoc />
    public IWorkflowEngine<TData> ResolveTyped<TData>(string definitionId)
        where TData : class, IWorkflowData
    {
        var (engine, type) = this.GetEngineEntry(definitionId);

        if (engine is IWorkflowEngine<TData> typed)
            return typed;

        throw new InvalidOperationException(
            $"Engine registered for '{definitionId}' is for type '{type.Name}', not '{typeof(TData).Name}'.");
    }

    /// <inheritdoc />
    public (object Engine, Type DataType) ResolveWithType(string definitionId)
    {
        return this.GetEngineEntry(definitionId);
    }

    /// <inheritdoc />
    public object ResolveUntyped(string definitionId)
    {
        return this.GetEngineEntry(definitionId).Engine;
    }

    /// <inheritdoc />
    public bool Contains(string definitionId)
    {
        return this._engines.ContainsKey(definitionId);
    }

    /// <summary>
    /// Retrieves the engine entry associated with the given definition ID.
    /// </summary>
    /// <param name="definitionId">
    /// The unique identifier used to associate the engine and its corresponding data type.
    /// </param>
    /// <returns>
    /// A tuple containing the associated engine object and its corresponding data type.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the provided definition ID is null, empty, or consists only of whitespace.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when there is no engine registered for the provided definition ID.
    /// </exception>
    private (object Engine, Type DataType) GetEngineEntry(string definitionId)
    {
        if (string.IsNullOrWhiteSpace(definitionId))
            throw new ArgumentException("Definition ID cannot be null or empty.", nameof(definitionId));

        if (!this._engines.TryGetValue(definitionId, out var entry))
            throw new WorkflowRegistryException(
                definitionId,
                null,
                "No engine registered for definition ID'.");

        return entry;
    }
}