namespace Meridian.Application.Interfaces;

using Core;

/// <summary>
/// Defines a contract for file storage providers used in workflow attachment processing.
/// </summary>
/// <typeparam name="TAttachmentReference">The type of the reference returned after a file upload.</typeparam>
public interface IWorkflowFileStorageProvider<TAttachmentReference>
{
    /// <summary>
    /// Asynchronously uploads a workflow attachment to a storage provider
    /// and returns a reference to the uploaded attachment.
    /// </summary>
    /// <param name="attachmentFile">
    /// The workflow attachment to be uploaded, containing file details
    /// such as name, content type, and content.
    /// </param>
    /// <returns>
    /// A reference to the uploaded attachment indicating details about the
    /// stored file.
    /// </returns>
    Task<TAttachmentReference> UploadAsync(IWorkflowAttachment attachmentFile);
}