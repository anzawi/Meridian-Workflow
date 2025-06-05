namespace LeaveRequestSample.Models;

public class AttachmentReference
{
    public Guid Id { get; set; }
    public string Path { get; set; } = string.Empty;
    public string? Source { get; set; }
}
