namespace Meridian.Core.Tests;

using Enums;

public class WorkflowHookDescriptorTests
{
    private readonly WorkflowHookDescriptor<MockWorkflowData> _descriptor;
    private readonly MockWorkflowHook _hook;

    public WorkflowHookDescriptorTests()
    {
        _hook = new MockWorkflowHook();
        _descriptor = new WorkflowHookDescriptor<MockWorkflowData>
        {
            Hook = _hook
        };
    }

    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        var descriptor = new WorkflowHookDescriptor<MockWorkflowData>();
        
        Assert.Multiple(
            () => Assert.Equal(HookExecutionMode.Sequential, descriptor.Mode),
            () => Assert.False(descriptor.ContinueOnFailure),
            () => Assert.True(descriptor.IsAsync),
            () => Assert.True(descriptor.LogExecutionHistory)
        );
    }

    [Theory]
    [InlineData(HookExecutionMode.Sequential)]
    [InlineData(HookExecutionMode.Parallel)]
    public void Mode_CanBeSet(HookExecutionMode mode)
    {
        // Act
        _descriptor.Mode = mode;

        // Assert
        Assert.Equal(mode, _descriptor.Mode);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ContinueOnFailure_CanBeSet(bool value)
    {
        // Act
        _descriptor.ContinueOnFailure = value;

        // Assert
        Assert.Equal(value, _descriptor.ContinueOnFailure);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsAsync_CanBeSet(bool value)
    {
        // Act
        _descriptor.IsAsync = value;

        // Assert
        Assert.Equal(value, _descriptor.IsAsync);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void LogExecutionHistory_CanBeSet(bool value)
    {
        // Act
        _descriptor.LogExecutionHistory = value;

        // Assert
        Assert.Equal(value, _descriptor.LogExecutionHistory);
    }

    [Fact]
    public void Hook_CanBeSet()
    {
        // Arrange
        var newHook = new MockWorkflowHook();

        // Act
        _descriptor.Hook = newHook;

        // Assert
        Assert.Same(newHook, _descriptor.Hook);
    }
}