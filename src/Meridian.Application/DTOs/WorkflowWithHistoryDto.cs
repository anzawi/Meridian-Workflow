namespace Meridian.Application.DTOs;

using Core;
using Core.Interfaces;

public class WorkflowWithHistoryDto<TData> where TData: class, IWorkflowData
{
    public WorkflowRequestInstance<TData> Request { get; set; } = null!;
    public List<WorkflowTransition> History { get; set; } = [];
}