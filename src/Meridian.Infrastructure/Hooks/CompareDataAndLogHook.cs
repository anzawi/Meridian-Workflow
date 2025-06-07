namespace Meridian.Infrastructure.Hooks;

using System.Reflection;
using Core;
using Core.Contexts;
using Core.Interfaces;
using Core.Models;

/// <summary>
/// A hook that compares the current and previous workflow data, and logs any detected changes.
/// </summary>
/// <typeparam name="TData">
/// The type of the workflow data object being compared, which must implement the <see cref="IWorkflowData"/> interface.
/// </typeparam>
/// <remarks>
/// When executed, this hook inspects all public instance properties of the workflow data type to determine if any
/// properties have changed. For each changed property, a transition entry is added to the workflow context's history
/// with details about the field, its previous value, and its new value.
/// </remarks>
/// <example>
/// This class can be registered to a workflow to monitor and log changes in data during a workflow transition.
/// </example>
public class CompareDataAndLogHook<TData> : IWorkflowHook<TData>
    where TData : class, IWorkflowData
{
    /// Executes a workflow hook operation to compare data and log any changes in the workflow context.
    /// <param name="context">
    /// The workflow context that contains the input data, request data, and workflow history.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    public Task ExecuteAsync(WorkflowContext<TData> context)
    {
        var newData = context.InputData;
        var oldData = context.Request.Data;

        if (newData == null || oldData == null)
            return Task.CompletedTask;

        var properties = typeof(TData).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            var oldValue = prop.GetValue(oldData);
            var newValue = prop.GetValue(newData);

            if (!Equals(oldValue, newValue))
            {
                context.History.Add(new WorkflowTransition
                {
                    Timestamp = DateTime.UtcNow,
                    Metadata = new Dictionary<string, object?>
                    {
                        { "field", prop.Name },
                        { "old", oldValue?.ToString() ?? "None" },
                        { "new", newValue?.ToString() ?? "None" },
                    },
                    Type = "data_changed",
                });
            }
        }


        return Task.CompletedTask;
    }
}