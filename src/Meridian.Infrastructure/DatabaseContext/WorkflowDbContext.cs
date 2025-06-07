namespace Meridian.Infrastructure.DatabaseContext;

using System.Text.Json;
using Application.Configuration;
using Core;
using Core.Entities;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Represents the database context for managing workflow-related entities in the Meridian infrastructure.
/// </summary>
/// <remarks>
/// This class provides access to the database tables associated with workflow management, including workflow
/// request instances, transitions, and tasks. It is responsible for applying entity configurations
/// during the model-building process.
/// </remarks>
/// <remarks>
/// The <see cref="WorkflowDbContext"/> supports customization of table prefixes and database schema using
/// the <see cref="MeridianWorkflowDbBuilder"/> passed during instantiation.
/// </remarks>
/// <example>
/// Intended for use as a dependency in services like repositories and workflow task processing.
/// </example>
public class WorkflowDbContext : DbContext
{
    /// <summary>
    /// The <c>_prefix</c> field is used to store a customizable table name prefix for database entities.
    /// This value is typically provided through the <c>MeridianWorkflowDbBuilder</c> and used in constructing
    /// table names during Entity Framework Core model configuration to maintain consistent naming conventions
    /// across the database schema.
    /// </summary>
    private readonly string? _prefix;

    /// <summary>
    /// Represents the database schema name used for defining table configurations in the current database context.
    /// </summary>
    /// <remarks>
    /// This is an optional property that allows specifying a database schema to group relevant database objects
    /// under a particular schema namespace, depending on the configuration provided by the <see cref="MeridianWorkflowDbBuilder"/>.
    /// </remarks>
    private readonly string? _schema;

    /// <summary>
    /// Represents the database context for the Meridian workflow module.
    /// </summary>
    /// <remarks>
    /// This context is responsible for managing access to the database entities related to workflows, including workflow requests, transitions, and tasks.
    /// It enables configuration for table prefixes and schema through the use of a <see cref="MeridianWorkflowDbBuilder"/>.
    /// </remarks>
    public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options, MeridianWorkflowDbBuilder dbBuilder) :
        base(options)
    {
        this._prefix = dbBuilder.TablesPrefix;
        this._schema = dbBuilder.Schema;
    }

    /// <summary>
    /// Gets or sets the database set representing a collection of
    /// <see cref="WorkflowRequestInstance"/> entities. This property is used to query
    /// and interact with workflow request instances stored in the database.
    /// </summary>
    public DbSet<WorkflowRequestInstance> Requests => this.Set<WorkflowRequestInstance>();

    /// <summary>
    /// Represents the transitions in a workflow process within the database context.
    /// </summary>
    /// <remarks>
    /// This property provides access to the set of <see cref="WorkflowTransition"/> entities
    /// that define state changes and actions performed within workflow instances. The transitions
    /// are stored in the table defined by <see cref="WorkflowTransition.TableName"/>.
    /// </remarks>
    public DbSet<WorkflowTransition> Transitions => this.Set<WorkflowTransition>();

    /// <summary>
    /// Represents the set of workflow tasks in the workflow database context.
    /// </summary>
    /// <remarks>
    /// This property is of type <c>DbSet&lt;WorkflowRequestTask&gt;</c> and provides access to the workflow tasks
    /// stored in the database. Workflow tasks represent individual units of work or actions within a
    /// workflow request, and may include task metadata such as assigned users, roles, and their statuses.
    /// </remarks>
    public DbSet<WorkflowRequestTask> WorkflowTasks => this.Set<WorkflowRequestTask>();

    /// <summary>
    /// Configures the schema needed for the context during model creation. Overrides the ModelCreating behavior
    /// to apply configurations for entities in the data context.
    /// </summary>
    /// <param name="modelBuilder">An instance of <see cref="ModelBuilder"/> used to configure entity properties and relationships.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkflowRequestInstance>(this.ConfigureWorkflowRequestInstance);
        modelBuilder.Entity<WorkflowTransition>(this.ConfigureWorkflowTransition);
        modelBuilder.Entity<WorkflowRequestTask>(this.ConfigureWorkflowTask);
    }

    /// <summary>
    /// Configures the entity of type <see cref="WorkflowRequestInstance"/> for the database using the specified <see cref="EntityTypeBuilder"/>.
    /// </summary>
    /// <param name="entity">
    /// The <see cref="EntityTypeBuilder{WorkflowRequestInstance}"/> used to configure the <see cref="WorkflowRequestInstance"/> entity.
    /// </param>
    private void ConfigureWorkflowRequestInstance(EntityTypeBuilder<WorkflowRequestInstance> entity)
    {
        var name = $"{this._prefix ?? string.Empty}{WorkflowRequestInstance.TableName}";
        entity.ToTable(name, this._schema);
        
        entity.HasKey(e => e.Id);
        entity.Property(e => e.DataJson)
            .HasColumnName("Data")
            .HasColumnType("nvarchar(max)");
    }

    /// <summary>
    /// Configures the entity mappings for the <see cref="WorkflowTransition"/> table.
    /// </summary>
    /// <param name="entity">
    /// The <see cref="EntityTypeBuilder{WorkflowTransition}"/> instance used to configure the entity details.
    /// </param>
    /// <remarks>
    /// This method sets the table name and schema using the provided prefix and schema values.
    /// It maps entity properties to database column types including converting the <see cref="WorkflowTransition.Metadata"/> property
    /// for persistence in the database.
    /// </remarks>
    private void ConfigureWorkflowTransition(EntityTypeBuilder<WorkflowTransition> entity)
    {
        var name = $"{this._prefix ?? string.Empty}{WorkflowTransition.TableName}";
        entity.ToTable(name, this._schema);
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Metadata)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object?>>(v, (JsonSerializerOptions?)null)!)
            .HasColumnType("nvarchar(max)");
    }

    /// <summary>
    /// Configures the entity settings for the <see cref="WorkflowRequestTask"/> in the database context.
    /// This method defines table structure, property configurations, and custom conversions
    /// for managing the persistence of workflow tasks.
    /// </summary>
    /// <param name="entity">The <see cref="EntityTypeBuilder{WorkflowRequestTask}"/> used to configure the task entity.</param>
    private void ConfigureWorkflowTask(EntityTypeBuilder<WorkflowRequestTask> entity)
    {
        var name = $"{this._prefix ?? string.Empty}{WorkflowRequestTask.TableName}";
        entity.ToTable(name, this._schema);
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Status).HasConversion<string>();
        entity.Property(e => e.RequestId).IsRequired();
        entity.Property(e => e.State).IsRequired();
        entity.Property(e => e.Action).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();

        entity.Property(e => e.AssignedToRoles)
            .HasConversion(
                v => string.Join(";", v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList());

        entity.Property(e => e.AssignedToGroups)
            .HasConversion(
                v => string.Join(";", v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList());
    }
}
