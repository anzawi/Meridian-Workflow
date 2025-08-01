using Meridian.Core.Interfaces.DslBuilder.Hooks;

namespace Meridian.Infrastructure.WorkflowDI;

using Application.Configuration;
using Application.Interfaces;
using DatabaseContext;
using Helpers;
using Microsoft.Extensions.DependencyInjection;
using Services;

/// <summary>
/// Provides extension methods for setting up and configuring Meridian workflow services
/// within the dependency injection container.
/// </summary>
public static class WorkflowDiExtensions
{
    /// <summary>
    /// Adds the Meridian Workflow infrastructure and services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to which the Meridian Workflow services will be added.</param>
    /// <param name="configure">
    /// A delegate to configure the <see cref="MeridianWorkflowOptions"/> instance, allowing customization of MeridianWorkflow behavior.
    /// </param>
    /// <returns>
    /// The IServiceCollection with the Meridian Workflow services registered.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the Meridian Workflow options have not been configured properly:
    /// - If the database configuration is not specified through <see cref="MeridianWorkflowDbBuilder.ConfigureOptions"/>.
    /// - If the attachment processor is enabled but no file storage provider is set.
    /// </exception>
    public static IServiceCollection AddMeridianWorkflow(
        this IServiceCollection services,
        Action<MeridianWorkflowOptions> configure)
    {
        var options = new MeridianWorkflowOptions();
        configure(options);

        if (options.DbBuilder.ConfigureOptions == null)
            throw new InvalidOperationException("You must call db.Use(...) inside ConfigureDb(...)");

        // Register DB Context
        services.AddDbContext<WorkflowDbContext>(options.DbBuilder.ConfigureOptions);

        // Optional: Use TablesPrefix and Schema during model building
        services.Configure<MeridianWorkflowDbBuilder>(builder =>
        {
            builder.TablesPrefix = options.DbBuilder.TablesPrefix;
            builder.Schema = options.DbBuilder.Schema;
        });

        services.AddHostedService<WorkflowMigrationService>();

        // Attachment
        if (options.EnableAttachmentProcessor)
        {
            if (options.FileStorageProviderType == null)
                throw new InvalidOperationException(
                    "Attachment processor is enabled, but no file storage provider was set. Call SetFileStorageProvider(...)");

            var providerType = options.FileStorageProviderType;
            if (providerType.IsGenericTypeDefinition)
            {
                // Register open generic type
                services.AddScoped(typeof(IWorkflowFileStorageProvider<>), providerType);
            }
            else
            {
                // Register closed type with its specific interface
                var implementedInterface = providerType
                    .GetInterfaces()
                    .First(i => i.IsGenericType &&
                                i.GetGenericTypeDefinition() == typeof(IWorkflowFileStorageProvider<>));
                services.AddScoped(implementedInterface, providerType);
            }

            services.AddSingleton<IWorkflowFileStorageProviderFactory, WorkflowFileStorageProviderFactory>();
        }
        else
        {
            services.AddSingleton<IWorkflowFileStorageProviderFactory, DisabledWorkflowFileStorageProviderFactory>();
        }

        // Workflows
        WorkflowEngineRegistrar.RegisterAll(services, options);
        services.AddSingleton(options.DbBuilder);
        services.AddScoped(typeof(IWorkflowTaskService<>), typeof(WorkflowTaskService<>));

        return services;
    }
}