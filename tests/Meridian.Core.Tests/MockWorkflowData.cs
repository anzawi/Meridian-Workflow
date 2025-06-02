namespace Meridian.Core.Tests;

using Interfaces;

public abstract class MockWorkflowData : IWorkflowData
{
    public string Id { get; set; } = string.Empty;
}
