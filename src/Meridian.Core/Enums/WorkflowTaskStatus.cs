namespace Meridian.Core.Enums;

/// <summary>
/// Represents a workflow task status indicating that the task is currently in progress.
/// </summary>
/// <remarks>
/// The <c>InProgress</c> status signifies that a task has been started but is not yet complete.
/// This status is typically used to track tasks under active execution or handling.
/// </remarks>
public enum WorkflowTaskStatus
{
    /// <summary>
    /// Indicates that the workflow task is awaiting action or processing.
    /// </summary>
    /// <remarks>
    /// This status signifies that the task has been created but no action has been taken yet.
    /// It generally represents the initial state of a task before it is started.
    /// </remarks>
    Pending,

    /// <summary>
    /// Indicates that the workflow task is currently in progress.
    /// </summary>
    /// <remarks>
    /// This status represents that the task has been started and is undergoing execution but has not yet been completed.
    /// It is used to track tasks actively being worked on.
    /// </remarks>
    InProgress,

    /// <summary>
    /// Represents a completed workflow task.
    /// </summary>
    /// <remarks>
    /// This status indicates that the task has been successfully finished
    /// and requires no further action. It typically signifies the final
    /// state of a task in the workflow process.
    /// </remarks>
    Completed,

    /// <summary>
    /// Indicates that the workflow task has been intentionally skipped.
    /// </summary>
    /// <remarks>
    /// This status signifies that the task was bypassed and not executed as part of the workflow process.
    /// Tasks with this status are excluded from standard completion or processing requirements.
    /// </remarks>
    Skipped
}