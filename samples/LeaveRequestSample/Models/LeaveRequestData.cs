namespace LeaveRequestSample.Models;

using Enums;
using Meridian.Core;
using Meridian.Core.Interfaces;

public class LeaveRequestData : IWorkflowData
{
    public string EmployeeName { get; set; } = string.Empty;
    public LeaveTypes LeaveType { get; set; }
    public int Days { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public string Reason { get; set; } = string.Empty;

    // Attachment: Required if LeaveType == "Sick"
    public WorkflowAttachment<AttachmentReference>? MedicalReport { get; set; }
}
