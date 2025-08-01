using Meridian.Core.Contexts;
using Meridian.Core.Models;

namespace Meridian.Core.Interfaces.DslBuilder.Hooks;

public interface IHookExecutionLogger
{
    ValueTask LogAsync<TData>(WorkflowTransition entry, WorkflowContext<TData> context, CancellationToken cancellationToken)
        where TData : class, IWorkflowData;
}