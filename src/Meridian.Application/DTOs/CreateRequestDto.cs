namespace Meridian.Application.DTOs;

public class CreateRequestDto
{
    public string DefinitionId { get; set; } = string.Empty;
    public Dictionary<string, object> InputData { get; set; } = new();
    public string UserId { get; set; } = string.Empty;
}