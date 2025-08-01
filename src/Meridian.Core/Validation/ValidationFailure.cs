namespace Meridian.Core.Validation;

public sealed class WorkflowValidationFailureException(string state, string action, List<ValidationFailure> failures)
    : Exception($"Validation failed for action '{action}' in state '{state}': {failures.Count} issue(s).")
{
    public string State { get; } = state;
    public string Action { get; } = action;
    public IReadOnlyList<ValidationFailure> Failures { get; } = failures;
}

public sealed class ValidationFailure
{
    public string ValidationName { get; init; } = default!;
    public string Message { get; init; } = default!;
    public string? Code { get; init; }
    public Severity? Severity { get; init; }
    public IReadOnlyDictionary<string, object>? Metadata { get; init; }
}