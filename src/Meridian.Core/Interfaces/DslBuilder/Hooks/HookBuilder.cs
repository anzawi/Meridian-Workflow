using Meridian.Core.Dtos;

namespace Meridian.Core.Interfaces.DslBuilder.Hooks;

public readonly struct HookBuilder<TBuilder, TData>(TBuilder builder, WorkflowHookDescriptor<TData> descriptor)
    where TData : class, IWorkflowData
{
    public HookBuilder<TBuilder, TData> WithName(string name)
    {
        descriptor.Name = name;
        return this;
    }

    public HookBuilder<TBuilder, TData> WithMetadata(string key, object? value)
    {
        descriptor.AddMetadata(key, value);
        return this;
    }

    public TBuilder EndHook() => builder;

    private TBuilder Builder => builder;

    public static implicit operator TBuilder(HookBuilder<TBuilder, TData> hookBuilder)
        => hookBuilder.Builder;
}