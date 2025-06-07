namespace Meridian.Infrastructure.Services;

using Application.Interfaces;
using Core;
using Core.Models;

/// <summary>
/// Represents a factory for creating instances responsible for handling workflow file storage
/// when file storage operations are disabled. This implementation silently ignores
/// file upload operations and does not store any files.
/// </summary>
/// <remarks>
/// This factory can be used in scenarios where the file storage feature is intentionally disabled,
/// ensuring that any file upload operations are no-ops.
/// </remarks>
internal class DisabledWorkflowFileStorageProviderFactory : IWorkflowFileStorageProviderFactory
{
    /// <summary>
    /// Asynchronously handles the upload of a workflow attachment. This implementation
    /// ignores the input and returns null.
    /// </summary>
    /// <param name="file">The workflow attachment to be uploaded.</param>
    /// <param name="referenceType">The reference type associated with the attachment.</param>
    /// <returns>A task that represents the asynchronous operation. The task result will always be null.</returns>
    public Task<object?> UploadAsync(IWorkflowAttachment file, Type referenceType)
    {
        // Silently ignore and return null
        return Task.FromResult<object?>(null);
    }
}