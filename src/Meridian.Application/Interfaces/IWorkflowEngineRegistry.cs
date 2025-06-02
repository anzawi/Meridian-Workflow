namespace Meridian.Application.Interfaces;

using Core.Interfaces;

/// <summary>
/// Represents a registry for workflow engines that allows registering and retrieving
/// engines based on workflow definition identifiers. This interface provides methods
/// to register workflow engines with specific data types and resolve them for use in
/// various workflow operations.
/// </summary>
public interface IWorkflowEngineRegistry
{
    /// <summary>
    /// Registers a workflow engine for a specified workflow definition ID.
    /// </summary>
    /// <typeparam name="TData">
    /// The type of the workflow data, which must implement the <see cref="IWorkflowData"/> interface.
    /// </typeparam>
    /// <param name="definitionId">
    /// The unique identifier for the workflow definition. This identifier is used to resolve the corresponding workflow engine.
    /// </param>
    /// <param name="engine">
    /// The workflow engine instance to be associated with the specified definition ID.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when the provided <paramref name="definitionId"/> is null, empty, or consists solely of whitespace.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the provided <paramref name="engine"/> is null.
    /// </exception>
    void Register<TData>(string definitionId, IWorkflowEngine<TData> engine)
        where TData : class, IWorkflowData;

    /// Resolves and retrieves a strongly-typed workflow engine for the given definition identifier.
    /// <param name="definitionId">The unique identifier of the workflow definition.</param>
    /// <typeparam name="TData">The type of the workflow data associated with the workflow engine, which must implement IWorkflowData.</typeparam>
    /// <returns>The strongly-typed workflow engine that corresponds to the specified definition identifier.</returns>
    IWorkflowEngine<TData> ResolveTyped<TData>(string definitionId)
        where TData : class, IWorkflowData;

    /// <summary>
    /// Resolves a workflow engine and its associated data type for the specified workflow definition ID.
    /// </summary>
    /// <param name="definitionId">The unique identifier for the workflow definition.</param>
    /// <returns>A tuple containing the workflow engine as an object and the associated data type.</returns>
    /// <exception cref="ArgumentException">Thrown if the provided <paramref name="definitionId"/> is null, empty, or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown if no workflow engine is registered for the specified <paramref name="definitionId"/>.</exception>
    public (object Engine, Type DataType) ResolveWithType(string definitionId);

    /// <summary>
    /// Resolves a workflow engine instance in an untyped manner for the specified definition ID.
    /// </summary>
    /// <param name="definitionId">The unique identifier of the workflow definition.</param>
    /// <returns>An untyped workflow engine associated with the given definition ID.</returns>
    public object ResolveUntyped(string definitionId);

    /// <summary>
    /// Checks if a workflow engine with the given definition ID exists in the registry.
    /// </summary>
    /// <param name="definitionId">The definition ID of the workflow engine to check.</param>
    /// <returns>Returns true if a workflow engine with the specified definition ID exists in the registry; otherwise, false.</returns>
    bool Contains(string definitionId);
}