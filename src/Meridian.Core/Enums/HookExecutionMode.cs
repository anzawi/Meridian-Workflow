namespace Meridian.Core.Enums;

/// <summary>
/// Represents a mode in which workflow hooks are executed in parallel.
/// </summary>
/// <remarks>
/// When a hook is set to execute in the <c>Parallel</c> mode, multiple hooks
/// are executed concurrently, allowing for faster processing when tasks
/// are independent and can be parallelized. This is typically useful for
/// non-blocking and independent operations. Use caution as this approach
/// may introduce race conditions if hooks share mutable resources.
/// </remarks>
public enum HookExecutionMode
{
    /// <summary>
    /// Specifies that hooks are executed sequentially in the order they are defined.
    /// </summary>
    /// <remarks>
    /// When the <c>Sequential</c> execution mode is used, hooks are run one after
    /// another in a synchronous manner. This ensures operations are completed
    /// in their defined sequence, which is beneficial when hooks are interdependent
    /// or when maintaining a specific order is critical. However, this mode might
    /// introduce longer execution times compared to parallel execution.
    /// </remarks>
    Sequential,

    /// <summary>
    /// Represents a mode where hooks in a workflow are executed concurrently.
    /// </summary>
    /// <remarks>
    /// The <c>Parallel</c> execution mode is utilized when independent hooks can execute without sequential dependency.
    /// This mode is ideal for improving performance and throughput for tasks that are non-blocking and do not share mutable state.
    /// Care should be taken to avoid race conditions or side effects when using this mode.
    /// </remarks>
    Parallel
}