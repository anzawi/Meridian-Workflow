using Meridian.Core.Interfaces;

namespace Meridian.Core.Validation.Internal;

internal sealed class NamedValidator<TData>(string name, Func<TData, ValidationResult> validator)
    where TData : class, IWorkflowData
{
    public string Name { get; } = name;

    public ValidationResult Validate(TData data) => validator(data);
}