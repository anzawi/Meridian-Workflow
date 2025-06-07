namespace Meridian.Core.Tests;

using Dtos;
using Enums;

public class WorkflowHookDescriptorTests
{
    private readonly WorkflowHookDescriptor<MockWorkflowData> _descriptor;

    public WorkflowHookDescriptorTests()
    {
        var hook = new MockWorkflowHook();
        this._descriptor = new WorkflowHookDescriptor<MockWorkflowData>
        {
            Hook = hook,
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
        this._descriptor.Mode = mode;

        // Assert
        Assert.Equal(mode, this._descriptor.Mode);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ContinueOnFailure_CanBeSet(bool value)
    {
        // Act
        this._descriptor.ContinueOnFailure = value;

        // Assert
        Assert.Equal(value, this._descriptor.ContinueOnFailure);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsAsync_CanBeSet(bool value)
    {
        // Act
        this._descriptor.IsAsync = value;

        // Assert
        Assert.Equal(value, this._descriptor.IsAsync);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void LogExecutionHistory_CanBeSet(bool value)
    {
        // Act
        this._descriptor.LogExecutionHistory = value;

        // Assert
        Assert.Equal(value, this._descriptor.LogExecutionHistory);
    }

    [Fact]
    public void Hook_CanBeSet()
    {
        // Arrange
        var newHook = new MockWorkflowHook();

        // Act
        this._descriptor.Hook = newHook;

        // Assert
        Assert.Same(newHook, this._descriptor.Hook);
    }
}