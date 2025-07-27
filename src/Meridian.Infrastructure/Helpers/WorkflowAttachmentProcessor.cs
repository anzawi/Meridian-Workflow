namespace Meridian.Infrastructure.Helpers;

using System.Reflection;
using Application.Interfaces;
using Core.Interfaces;
using Core.Models;
using Services;

/// <summary>
/// The WorkflowAttachmentProcessor class is a static utility for processing
/// attachments in workflow data types that implement the <see cref="IWorkflowData"/> interface.
/// It ensures that workflow attachments, whether single or in collections, are properly
/// processed before being handled by a workflow engine or repository.
/// </summary>
internal static class WorkflowAttachmentProcessor
{
    /// <summary>
    /// Processes the workflow attachments within the provided data object.
    /// Handles single and list-based attachments using the specified storage provider factory.
    /// </summary>
    /// <typeparam name="TData">The type of workflow data implementing <see cref="IWorkflowData"/>.</typeparam>
    /// <param name="data">
    /// The data object containing workflow attachments to be processed. Must not be null.
    /// </param>
    /// <param name="storageProvider">
    /// An instance of <see cref="IWorkflowFileStorageProviderFactory"/> used for handling attachment uploads.
    /// If the storage provider is an instance of <see cref="DisabledWorkflowFileStorageProviderFactory"/>, no processing occurs.
    /// </param>
    /// <returns>
    /// Returns the processed <typeparamref name="TData"/> object with attachment references updated where needed.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="data"/> parameter is null.</exception>
    internal static async Task<TData> ProcessAttachmentsAsync<TData>(
        TData data,
        IWorkflowFileStorageProviderFactory storageProvider)
        where TData : class, IWorkflowData
    {
        if (storageProvider is DisabledWorkflowFileStorageProviderFactory)
        {
            return data;
        }

        if (data == null)
            throw new ArgumentNullException(nameof(data));

        var properties = typeof(TData)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToList();

        var tasks = new List<Task>();

        foreach (var prop in properties)
        {
            var propType = prop.PropertyType;

            // Case 1: Single attachment
            if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(WorkflowAttachment<>))
            {
                var value = prop.GetValue(data);
                if (value is not IWorkflowAttachment { Reference: null } attachment)
                    continue;

                tasks.Add(Task.Run(async () =>
                {
                    attachment.Reference = null;
                    dynamic dynamicAttachment = attachment;
                    var referenceType = propType.GetGenericArguments()[0];
                    dynamic? uploaded = await storageProvider.UploadAsync(attachment, referenceType);
                    dynamicAttachment.Content = null;
                    dynamicAttachment.Reference = uploaded;
                }));
            }

            // Case 2: List of attachments
            else if (propType.IsGenericType &&
                     propType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var itemType = propType.GetGenericArguments()[0];
                if (itemType.IsGenericType &&
                    itemType.GetGenericTypeDefinition() == typeof(WorkflowAttachment<>))
                {
                    var list = prop.GetValue(data) as System.Collections.IEnumerable;
                    if (list == null) continue;

                    foreach (var item in list)
                    {
                        if (item is not IWorkflowAttachment { Reference: null } attachment)
                            continue;

                        tasks.Add(Task.Run(async () =>
                        {
                            attachment.Reference = null;
                            dynamic dynamicAttachment = attachment;
                            var referenceType = itemType.GetGenericArguments()[0];
                            dynamic? uploaded = await storageProvider.UploadAsync(attachment, referenceType);
                            dynamicAttachment.Content = null;
                            dynamicAttachment.Reference = uploaded;
                        }));
                    }
                }
            }
        }

        await Task.WhenAll(tasks);
        return data;
    }
}