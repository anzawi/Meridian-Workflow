namespace Meridian.Infrastructure.Helpers;

using Core;
using Core.Enums;
using Core.Interfaces;

/// <summary>
/// Provides methods for executing workflow hooks in a structured manner.
/// This class is used to invoke hooks that are associated with a workflow,
/// ensuring proper execution order and handling both synchronous and asynchronous hooks.
/// </summary>
internal static class HookExecutor
{
    /// <summary>
    /// Executes a collection of workflow hooks in both synchronous and asynchronous manners based on their configured execution mode.
    /// Hooks can be executed sequentially or in parallel depending on the specified execution mode.
    /// </summary>
    /// <typeparam name="TData">The type of the workflow data, which must implement <see cref="IWorkflowData"/>.</typeparam>
    /// <param name="hooks">A collection of <see cref="WorkflowHookDescriptor{TData}"/> representing the hooks to be executed.</param>
    /// <param name="context">The <see cref="WorkflowContext{TData}"/> providing context for the hooks being executed.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task ExecuteAll<TData>(
        List<WorkflowHookDescriptor<TData>> hooks,
        WorkflowContext<TData> context)
        where TData : class, IWorkflowData
    {
        var syncHooks = hooks.Where(h => !h.IsAsync).ToList();
        var asyncHooks = hooks.Where(h => h.IsAsync).ToList();

        foreach (var group in syncHooks.GroupBy(h => h.Mode))
        {
            if (group.Key == HookExecutionMode.Sequential)
            {
                foreach (var h in group)
                {
                    await SafeExecute(h.Hook, context, h.ContinueOnFailure, h.Hook.GetType().Name, h.LogExecutionHistory);
                }
            }
            else
            {
                var tasks = group.Select(h =>
                    SafeExecute(h.Hook, context, h.ContinueOnFailure, h.Hook.GetType().Name, h.LogExecutionHistory));
                await Task.WhenAll(tasks);
            }
        }

        // Fire-and-forget async hooks
        foreach (var group in asyncHooks.GroupBy(h => h.Mode))
        {
            if (group.Key == HookExecutionMode.Sequential)
            {
                _ = Task.Run(async () =>
                {
                    foreach (var h in group)
                    {
                        try
                        {
                            await SafeExecute(h.Hook, context, h.ContinueOnFailure, h.Hook.GetType().Name, h.LogExecutionHistory);
                        }
                        catch
                        {
                            /* optionally log */
                        }
                    }
                });
            }
            else
            {
                var tasks = group.Select(h =>
                    SafeExecute(h.Hook, context, h.ContinueOnFailure, h.Hook.GetType().Name, h.LogExecutionHistory));
                _ = Task.WhenAll(tasks);
            }
        }
    }

    /// <summary>
    /// Executes a given workflow hook safely, ensuring that any exceptions are managed appropriately.
    /// The method logs execution metadata, handles exceptions based on the `continueOnFailure` flag, and optionally adds an entry to the workflow execution history.
    /// </summary>
    /// <typeparam name="TData">The type of data used in the workflow context, which must implement <see cref="IWorkflowData"/>.</typeparam>
    /// <param name="hook">The workflow hook to be executed.</param>
    /// <param name="context">The workflow context associated with the hook execution.</param>
    /// <param name="continueOnFailure">Indicates whether the execution should continue if the hook throws an exception. If false, exceptions will be rethrown.</param>
    /// <param name="hookName">The name of the hook, primarily used for logging purposes.</param>
    /// <param name="logHistory">Indicates whether an execution entry should be logged in the workflow history.</param>
    /// <returns>A task that represents the asynchronous operation of executing the workflow hook.</returns>
    private static async Task SafeExecute<TData>(
        IWorkflowHook<TData> hook,
        WorkflowContext<TData> context,
        bool continueOnFailure,
        string hookName,
        bool logHistory) where TData : class, IWorkflowData
    {
        var entry = new WorkflowTransition
        {
           Type = "Event",
           Action = hookName,
           Timestamp = DateTime.UtcNow,
           Metadata = new Dictionary<string, object?>()
        };

        try
        {
            await hook.ExecuteAsync(context);
            entry.Metadata.Add("Statue", "Success");
        }
        catch (Exception ex)
        {
            entry.Metadata.Add("Statue", "Failed");
            entry.Metadata.Add("Error", ex.Message);

            context.History.Add(entry); // Always log before failing
            if (!continueOnFailure)
                throw;
            return;
        }

        if (logHistory)
        {
            context.History.Add(entry);
        }
    }
}