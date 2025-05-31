namespace Meridian.Core.Interfaces;

/// <summary>
/// Defines a contract for a workflow hook that executes within a workflow system.
/// A workflow hook represents a piece of logic that is executed at specific stages
/// or transitions of a workflow, using the provided workflow context.
/// </summary>
/// <typeparam name="TData">
/// The type of data associated with the workflow. This type must implement <see cref="IWorkflowData"/>.
/// </typeparam>
public interface IWorkflowHook<TData> where TData : class, IWorkflowData
{
    /// <summary>
    /// Executes a workflow hook asynchronously based on the provided context.
    /// </summary>
    /// <param name="context">The workflow context containing data, request, user information, and transition history.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteAsync(WorkflowContext<TData> context);
}