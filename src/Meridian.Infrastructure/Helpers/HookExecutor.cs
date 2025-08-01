using Meridian.Core.Extensions;
using Meridian.Core.Interfaces.DslBuilder.Hooks;

namespace Meridian.Infrastructure.Helpers;

using Core.Contexts;
using Core.Dtos;
using Core.Enums;
using Core.Interfaces;
using Core.Models;

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
    /// <param name="logger">The logger that implemented by consumer <see cref="IHookExecutionLogger"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task ExecuteAll<TData>(
        List<WorkflowHookDescriptor<TData>> hooks,
        WorkflowContext<TData> context,
        IHookExecutionLogger? logger = null,
        CancellationToken cancellationToken = default)
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
                    await SafeExecute(h, context);
                }
            }
            else
            {
                var tasks = group.Select(h =>
                    SafeExecute(h, context));
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
                            await SafeExecute(h, context);
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
                    SafeExecute(h, context));
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
    /// <returns>A task that represents the asynchronous operation of executing the workflow hook.</returns>
    private static async Task SafeExecute<TData>(
        WorkflowHookDescriptor<TData> hook,
        WorkflowContext<TData> context
       ) where TData : class, IWorkflowData
    {
        var startedAt = DateTime.UtcNow;
        var entry = new WorkflowTransition
        {
            Type = "Event",
            Action = hook.Name ?? hook.GetType().Name,
            Timestamp = startedAt,
            Metadata = [],
        };

        try
        {
            await hook.Hook.ExecuteAsync(context);
            entry.Metadata.Add("Status", "Success");
            entry.Metadata.Add("startedAt", startedAt);
            entry.Metadata.Add("DurationMs", (DateTime.UtcNow - startedAt).TotalMilliseconds);
            entry.Metadata.Add("Status", "Success");

            entry.Metadata.MergeMetadata(hook.Metadata); // Add hook metadata to entry metadata ( if any)
        }
        catch (Exception ex)
        {
            entry.Metadata.Add("Status", "Failed");
            entry.Metadata.Add("Error", ex.Message);
            entry.Metadata.Add("startedAt", startedAt);
            entry.Metadata.Add("DurationMs", (DateTime.UtcNow - startedAt).TotalMilliseconds);
            entry.Metadata.MergeMetadata(hook.Metadata);
            context.History.Add(entry); // Always log before failing
            if (!hook.ContinueOnFailure)
                throw;
            return;
        }

        if (hook.LogExecutionHistory)
        {
            context.History.Add(entry);
        }
    }
}