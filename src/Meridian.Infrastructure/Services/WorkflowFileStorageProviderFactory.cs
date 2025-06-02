namespace Meridian.Infrastructure.Services;

using Application.Interfaces;
using Core;
using Core.Enums;
using Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a factory for creating and managing instances of workflow file storage providers.
/// This factory enables asynchronous uploading of workflow attachments associated with a
/// specific reference type by dynamically resolving the appropriate storage provider for the type.
/// </summary>
public class WorkflowFileStorageProviderFactory(IServiceScopeFactory serviceProviderFactory)
    : IWorkflowFileStorageProviderFactory
{
    /// <inheritdoc />
    public async Task<object?> UploadAsync(IWorkflowAttachment file, Type referenceType)
    {
        try
        {
            using var scope = serviceProviderFactory.CreateScope();
            var providerType = typeof(IWorkflowFileStorageProvider<>).MakeGenericType(referenceType);
            var provider = scope.ServiceProvider.GetService(providerType)
                           ?? throw new WorkflowFileException(
                               file.FileName,
                               WorkflowFileOperation.Upload,
                               $"No storage provider found for type {referenceType.Name}");

            var method = providerType.GetMethod(nameof(IWorkflowFileStorageProvider<object>.UploadAsync));
            var task = (Task)method!.Invoke(provider, [file])!;
            await task.ConfigureAwait(false);

            var resultProperty = task.GetType().GetProperty("Result")!;
            return resultProperty.GetValue(task)!;
        }
        catch (Exception ex) when (ex is not WorkflowException)
        {
            throw new WorkflowFileException(
                file.FileName,
                WorkflowFileOperation.Upload,
                "File upload failed",
                ex);
        }
    }
}