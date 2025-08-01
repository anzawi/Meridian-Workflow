namespace Meridian.Application.Interfaces;

using Core.Models;

/// <summary>
/// Represents a factory interface for handling workflow file storage providers.
/// It defines the contract for uploading workflow attachments and associating them
/// with a specific reference type.
/// </summary>
public interface IWorkflowFileStorageProviderFactory
{
    /// <summary>
    /// Uploads the given workflow attachment to a file storage provider.
    /// </summary>
    /// <param name="file">The workflow attachment to be uploaded, containing file metadata and content.</param>
    /// <param name="referenceType">The reference type associated with the file, determining how it will be processed by the provider.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains an object representing the reference to the uploaded file, or null if the operation is ignored or unsuccessful.</returns>
    Task<object?> UploadAsync(IWorkflowAttachment file, Type referenceType);

}