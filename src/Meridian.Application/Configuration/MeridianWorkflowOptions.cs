namespace Meridian.Application.Configuration;

using Interfaces;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Represents configurable options for the Mini Workflow system.
/// This class allows customization of database configuration, file storage provider, and workflow registration.
/// </summary>
public class MeridianWorkflowOptions
{
    /// <summary>
    /// Provides configuration options for building and customizing the MiniWorkflow database setup.
    /// </summary>
    /// <remarks>
    /// This property offers access to <c>MiniWorkflowDbBuilder</c>, enabling customization of
    /// database options such as table prefix, schema, and additional configurations.
    /// It is primarily used internally by the MiniWorkflow system and can be configured by
    /// invoking the <c>ConfigureDb(Action&lt;MiniWorkflowDbBuilder&gt;)</c> method.
    /// </remarks>
    internal MiniWorkflowDbBuilder DbBuilder { get; } = new();

    /// <summary>
    /// Represents the type of file storage provider to be used in the workflow system.
    /// This property stores the <see cref="Type" /> of file storage provider implementation
    /// that must adhere to the generic <see cref="IWorkflowFileStorageProvider{TAttachmentReference}" /> interface.
    /// </summary>
    /// <remarks>
    /// The property is configured via the <c>SetFileStorageProvider</c> method, which validates
    /// that the specified type implements the required interface. It is primarily utilized
    /// for dependency injection and configuring the file storage provider for attachment processing.
    /// </remarks>
    internal Type? FileStorageProviderType { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the attachment processor is enabled.
    /// When set to <c>true</c>, a file storage provider must be configured using
    /// <see cref="SetFileStorageProvider(System.Type)"/>. This allows the workflow system
    /// to manage and process file attachments. If disabled, a default disabled provider
    /// will be used.
    /// </summary>
    public bool EnableAttachmentProcessor { get; set; } = true;

    /// <summary>
    /// Gets or sets the collection of workflow bootstrappers used to define and initialize workflows during application startup.
    /// </summary>
    /// <remarks>
    /// Each element in the collection implements the <see cref="IWorkflowBootstrapper"/> interface,
    /// allowing for the registration of custom workflows. Users must populate this property with the necessary
    /// workflow instances to enable workflow functionality in the application.
    /// </remarks>
    public List<IWorkflowBootstrapper> Workflows { get; set; } = [];

    /// <summary>
    /// Configures the database settings for the Mini Workflow system.
    /// </summary>
    /// <param name="configure">An action that allows customization of the <see cref="MiniWorkflowDbBuilder"/> to specify database configurations.</param>
    public void ConfigureDb(Action<MiniWorkflowDbBuilder> configure)
    {
        configure(this.DbBuilder);
    }

    /// <summary>
    /// Sets the file storage provider type that implements the IWorkflowFileStorageProvider interface.
    /// </summary>
    /// <param name="providerType">The type of the file storage provider to be set. This must implement IWorkflowFileStorageProvider with a specific generic type.</param>
    /// <exception cref="ArgumentNullException">Thrown when the providerType is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the providerType does not implement the generic IWorkflowFileStorageProvider interface.</exception>
    public void SetFileStorageProvider(Type providerType)
    {
        if (providerType == null)
            throw new ArgumentNullException(nameof(providerType));

        var match = providerType
            .GetInterfaces()
            .FirstOrDefault(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IWorkflowFileStorageProvider<>));

        if (match == null)
            throw new ArgumentException("Provider type must implement IWorkflowFileStorageProvider<>");

        this.FileStorageProviderType = providerType;
    }
}

/// <summary>
/// Represents a builder for configuring the database context of the MiniWorkflow module.
/// </summary>
/// <remarks>
/// This class is used to configure database-specific options, such as schema and table prefixing,
/// and allows customization of the <see cref="DbContextOptionsBuilder"/> for the WorkflowDbContext.
/// </remarks>
public class MiniWorkflowDbBuilder
{
    /// <summary>
    /// Gets or sets a delegate to configure options for the database context.
    /// This property holds an action that allows customization of the
    /// <see cref="DbContextOptionsBuilder"/> for setting up the database context
    /// used by the MiniWorkflow system.
    /// </summary>
    internal Action<DbContextOptionsBuilder> ConfigureOptions { get; private set; } = _ => { };

    /// <summary>
    /// Gets or sets the prefix used for the database table names in the workflow context.
    /// This property determines the naming convention for tables created within the database,
    /// providing a way to namespace or distinguish tables associated with the workflow from others.
    /// </summary>
    public string TablesPrefix { get; set; } = "MiniFlow_";

    /// <summary>
    /// Gets or sets the database schema that will be used for the workflow-related tables.
    /// If not specified, the default schema for the database will be used.
    /// </summary>
    public string? Schema { get; set; }

    /// <summary>
    /// Configures the database options for the MiniWorkflow.
    /// </summary>
    /// <param name="configure">
    /// A delegate that receives an instance of <see cref="DbContextOptionsBuilder"/> for applying custom database configuration options for MiniWorkflow.
    /// </param>
    public void Use(Action<DbContextOptionsBuilder> configure)
    {
        this.ConfigureOptions = configure;
    }
}