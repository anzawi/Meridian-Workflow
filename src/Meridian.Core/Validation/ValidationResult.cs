namespace Meridian.Core.Validation;

public sealed class ValidationResult
{
    public static readonly ValidationResult Ok = new(true, null, null, null, null);

    public bool IsValid { get; }
    public string? Message { get; }
    public string? Code { get; }
    public Severity? Severity { get; }
    public IReadOnlyDictionary<string, object>? Metadata { get; }

    private ValidationResult(
        bool isValid,
        string? message,
        string? code,
        Severity? severity,
        Dictionary<string, object>? metadata)
    {
        IsValid = isValid;
        Message = message;
        Code = code;
        Severity = severity;
        Metadata = metadata;
    }

    public static ValidationResult Fail(string message) => new(false, message, null, null, null);

    public ValidationResult WithCode(string code) =>
        new(IsValid, Message, code, Severity, Metadata?.ToDictionary(kv => kv.Key, kv => kv.Value));

    public ValidationResult WithSeverity(Severity severity) =>
        new(IsValid, Message, Code, severity, Metadata?.ToDictionary(kv => kv.Key, kv => kv.Value));

    public ValidationResult WithMetadata(string key, object value)
    {
        var newMetadata = Metadata?.ToDictionary(kv => kv.Key, kv => kv.Value) ?? new Dictionary<string, object>();
        newMetadata[key] = value;
        return new ValidationResult(IsValid, Message, Code, Severity, newMetadata);
    }
}
