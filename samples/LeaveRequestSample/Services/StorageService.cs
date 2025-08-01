namespace LeaveRequestSample.Services;

using Meridian.Application.Interfaces;
using Meridian.Core.Models;
using Models;

/// <inheritdoc />
public class StorageService : IWorkflowFileStorageProvider<AttachmentReference>
{
    public async Task<AttachmentReference> UploadAsync(IWorkflowAttachment attachmentFile)
    {
        return await Task.FromResult(new AttachmentReference
        {
            Id = Guid.NewGuid(),
            Path = $"path/{Guid.NewGuid()}",
            Source = "source",
        });
    }
}