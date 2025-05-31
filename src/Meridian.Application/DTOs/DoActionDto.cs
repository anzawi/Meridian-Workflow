namespace Meridian.Application.DTOs;

public class DoActionDto
{
    
    public string RequestId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public List<string> UserRoles { get; set; } = [];
    public List<string> UserGroups { get; set; } = [];
    public Dictionary<string, object> Data { get; set; } = new();
}