namespace Meridian.Core.Models;

/// <summary>
/// Represents a workflow attachment, typically used in handling files related
/// to workflows. The attachment includes details about the file such as its name,
/// content type, and content. It also optionally contains a reference to a system-specific
/// representation of the file.
/// </summary>
public interface IWorkflowAttachment
{
    /// <summary>
    /// Gets or sets the name of the file associated with the workflow attachment.
    /// </summary>
    string FileName { get; set; }

    /// <summary>
    /// Gets or sets the MIME type of the content associated with the workflow attachment.
    /// </summary>
    /// <remarks>
    /// The ContentType property specifies the media type of the content, such as "application/pdf",
    /// "image/jpeg", or "text/plain". This property is used to indicate the nature and format of the file.
    /// </remarks>
    string ContentType { get; set; }

    /// <summary>
    /// Gets or sets the content of the attachment as a byte array.
    /// Represents the binary data of the file stored in the attachment.
    /// </summary>
    byte[]? Content { get; set; }

    /// <summary>
    /// Represents a reference to an associated object or data for the workflow attachment.
    /// The type of the reference is determined by the generic type parameter
    /// in the specific implementation of <see cref="WorkflowAttachment{TAttachmentReference}"/>.
    /// This property is commonly used to associate additional metadata or context,
    /// such as an identifier or object representation, with the attachment.
    /// </summary>
    object? Reference { get; set; }
}

/// <summary>
/// Represents an attachment associated with a workflow, containing metadata, content, and a reference object.
/// </summary>
/// <typeparam name="TAttachmentReference">
/// The type of the reference object associated with the attachment. This type must be a reference type.
/// </typeparam>
public sealed class WorkflowAttachment<TAttachmentReference> : IWorkflowAttachment
    where TAttachmentReference : class
{
    /// <summary>
    /// Gets or sets the name of the file associated with the workflow attachment.
    /// </summary>
    /// <remarks>
    /// This property represents the filename of the associated attachment
    /// and is used to identify the file, typically including extensions.
    /// </remarks>
    public string FileName { get; set; } = default!;

    /// <summary>
    /// Gets or sets the MIME type of the content associated with the workflow attachment.
    /// This property indicates the type of file, such as "application/pdf", "image/png", or "text/plain".
    /// </summary>
    public string ContentType { get; set; } = default!;

    /// <summary>
    /// Gets or sets the binary content of the workflow attachment.
    /// </summary>
    /// <remarks>
    /// This property represents the main data of the file, typically as a byte array.
    /// It may be null if the attachment does not contain any content.
    /// </remarks>
    public byte[]? Content { get; set; }

    /// <summary>
    /// Gets or sets a reference to the associated attachment.
    /// This property allows a flexible type of reference that can correspond
    /// to an external resource, database entity, or any other contextual information
    /// related to the workflow attachment. The type of the reference should be
    /// compatible with the generic type parameter used in the implementation of the class.
    /// </summary>
    object? IWorkflowAttachment.Reference
    {
        get => this.Reference;
        set => this.Reference = value as TAttachmentReference;
    }

    /// <summary>
    /// Represents a reference to an attachment of a specified type in the workflow.
    /// This property is used to associate an attachment with an external resource or an identifier,
    /// enabling linking or further processing.
    /// The type of the reference is determined by the generic parameter
    /// associated with the <see cref="WorkflowAttachment{TAttachmentReference}"/>.
    /// </summary>
    public TAttachmentReference? Reference { get; set; }
}