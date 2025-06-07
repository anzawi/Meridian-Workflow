namespace Meridian.Core.Delegates;

using Contexts;
using Interfaces;

internal class DelegateWorkflowHook<TData>(Func<WorkflowContext<TData>, Task> hookFunc) : IWorkflowHook<TData>
    where TData : class, IWorkflowData
{
    private readonly Func<WorkflowContext<TData>, Task> _hookFunc = hookFunc ?? throw new ArgumentNullException(nameof(hookFunc));

    public Task ExecuteAsync(WorkflowContext<TData> context)
    {
        return this._hookFunc(context);
    }
    
    public static implicit operator DelegateWorkflowHook<TData>(Func<WorkflowContext<TData>, Task> handler)
        => new(handler);
}