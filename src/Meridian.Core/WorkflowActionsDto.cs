namespace Meridian.Core;

public class WorkflowActionDto
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string NextState { get; set; } = null!;
    public bool IsAuto { get; set; }
    public List<string> AssignedUsers { get; set; } = null!;
    public List<string> AssignedRoles { get; set; } = null!;
    public List<string> AssignedGroups { get; set; } = null!;
    public bool HasCondition { get; set; }
}