namespace Meridian.Core.Dtos;

using Meridian.Core.Enums;
using Meridian.Core.Interfaces;

/// <summary>
/// Represents a descriptor for a workflow hook, providing configuration options
/// such as execution mode, error handling, and asynchronous behavior.
/// </summary>
/// <typeparam name="TData">
/// The type of the workflow data associated with the hook, constrained to implement <see cref="IWorkflowData"/>.
/// </typeparam>
/// <remarks>
/// This class is used to define the behavior of a workflow hook, including its mode of execution
/// (e.g., sequential or parallel), whether execution should continue on failure, whether the hook runs asynchronously,
/// and if execution history should be logged. It serves as a foundational component in workflow configuration.
/// </remarks>
public class WorkflowHookDescriptor<TData> where TData : class, IWorkflowData
{
    private readonly Dictionary<string, object?> _metadata = [];
    public string? Name { get; internal set; }
    
    /// <summary>
    /// Represents an implementation of a hook that conforms to the <see cref="IWorkflowHook{TData}"/> interface.
    /// A hook is a functional component within a workflow that executes a specific task or set of operations
    /// during the lifecycle of a workflow.
    /// </summary>
    /// <typeparam name="TData">
    /// The type of data associated with the workflow. This type must implement the <see cref="IWorkflowData"/> interface.
    /// </typeparam>
    /// <remarks>
    /// A <see cref="Hook"/> can manipulate the workflow data, log execution details, and perform
    /// custom logic at defined points, such as during state transitions or request creation.
    /// It is a fundamental unit of extensibility and customization within a workflow framework.
    /// </remarks>
    public IWorkflowHook<TData> Hook { get; set; } = null!;

    /// <summary>
    /// Gets or sets the execution mode of the workflow hook.
    /// </summary>
    /// <remarks>
    /// The <see cref="Mode"/> property determines whether the hook execution is run sequentially
    /// or in parallel. The possible values are defined in the <see cref="HookExecutionMode"/> enum.
    /// </remarks>
    public HookExecutionMode Mode { get; set; } = HookExecutionMode.Sequential;

    /// <summary>
    /// Determines whether the workflow hook execution should continue if an exception occurs.
    /// </summary>
    /// <remarks>
    /// When set to true, the execution of subsequent hooks will proceed even if the current hook fails.
    /// If set to false, the execution will stop upon encountering an exception in the hook.
    /// This property provides flexibility in handling workflows where partial failure may be acceptable.
    /// </remarks>
    public bool ContinueOnFailure { get; set; } = false;

    /// <summary>
    /// Indicates whether the workflow hook is executed asynchronously.
    /// </summary>
    /// <remarks>
    /// When set to true, the hook executes in an asynchronous manner, allowing non-blocking operations.
    /// If set to false, the hook executes synchronously, ensuring sequential processing within the workflow execution.
    /// </remarks>
    public bool IsAsync { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the execution history of the workflow hook should be logged.
    /// If enabled, details of the execution will be tracked for auditing, debugging, or monitoring purposes.
    /// </summary>
    public bool LogExecutionHistory { get; set; } = true;
    public IReadOnlyDictionary<string, object?> Metadata => _metadata.AsReadOnly();

    internal void AddMetadata(string key, object? value)
    {
        this._metadata[key] = value;
    }
}