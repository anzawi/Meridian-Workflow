namespace Meridian.Core.Extensions;

using Dtos;
using Interfaces;
using Models;

internal static class WorkflowActionExtensions
{
    internal static WorkflowActionDto ToActionDto<TData>(this WorkflowAction<TData> action)
        where TData : class, IWorkflowData
    {
        return new WorkflowActionDto
        {
            Name = action.Name,
            Code = action.Code,
            NextState = action.NextState,
            IsAuto = action.IsAuto,
            AssignedUsers = action.AssignedUsers,
            AssignedRoles = action.AssignedRoles,
            AssignedGroups = action.AssignedGroups,
            HasCondition = action.Condition != null,
        };
    }

    internal static List<WorkflowActionDto> ToActionDto<TData>(this List<WorkflowAction<TData>> actions)
        where TData : class, IWorkflowData
    {
        return actions.Select(ToActionDto).ToList();
    }
}