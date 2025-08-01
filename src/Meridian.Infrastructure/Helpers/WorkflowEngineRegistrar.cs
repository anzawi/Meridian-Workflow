using Meridian.Application.Configuration;

namespace Meridian.Infrastructure.Helpers;

using Application.Interfaces;
using Application.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Repositories;
using Services;

/// <summary>
/// Provides methods to register and initialize workflow engines and related services into the specified
/// dependency injection service collection. This class is designed to map workflows defined by
/// implementations of <see cref="IWorkflowBootstrapper"/> to their corresponding concrete engines and services.
/// <example>
/// <code>
///var bootstrappers = new IWorkflowBootstrapper[]
///{
///    new LeaveApprovalWorkflow(),
///    new GenericWorkflowBootstrapper()
///};
///WorkflowEngineRegistrar.RegisterAll(services, bootstrappers);
/// </code>
/// </example>
/// </summary>
internal static class WorkflowEngineRegistrar
{
    /// <summary>
    /// Registers all workflow engines, services, and the workflow engine registry into the provided service collection.
    /// </summary>
    /// <param name="services">The service collection to register the workflow components into.</param>
    /// <param name="options">Workflow Configuration options.</param>
    public static void RegisterAll(IServiceCollection services, MeridianWorkflowOptions options)
    {
        var bootstrappers = options.Workflows;
        var builder = new WorkflowDefinitionBuilder(options.HookExecutionLogger);

        foreach (var bootstrapper in bootstrappers)
            bootstrapper.Register(builder);

        var engines = builder.BuildEngines();

        // Register all engines and services
        foreach (var (definitionId, engine, dataType) in engines)
        {
            var engineType = typeof(IWorkflowEngine<>).MakeGenericType(dataType);
           
            services.AddSingleton(engineType, engine);
        }
        services.AddScoped(typeof(IWorkflowService<>), typeof(WorkflowService<>));
        // Register engine registry
        services.AddSingleton<IWorkflowEngineRegistry>(provider =>
        {
            var registry = new InMemoryWorkflowEngineRegistry();
            foreach (var (definitionId, engine, dataType) in engines)
            {
                var method = typeof(IWorkflowEngineRegistry)
                    .GetMethod("Register")!
                    .MakeGenericMethod(dataType);
                method.Invoke(registry, [definitionId, engine]);
            }
            return registry;
        });
        
        services.AddScoped(typeof(IWorkflowService<>), typeof(WorkflowService<>));
        services.AddScoped<IWorkflowQueryService, WorkflowQueryService>();
        services.AddScoped<IWorkflowRepository, WorkflowRepository>();
    }
}