namespace Meridian.Core.Tests;

using Enums;
using Interfaces;

public class WorkflowStateTests
{
    private readonly WorkflowState<MockWorkflowData> _state;

    public WorkflowStateTests()
    {
        this._state = new WorkflowState<MockWorkflowData>("TestState");
    }

    [Fact]
    public void Code_GeneratesPascalCaseFromName()
    {
        var state = new WorkflowState<MockWorkflowData>("test-state");
        Assert.Equal("TestState", state.Code);
    }

    [Theory]
    [InlineData(nameof(WorkflowState<MockWorkflowData>.IsCompleted), StateType.Completed)]
    [InlineData(nameof(WorkflowState<MockWorkflowData>.IsStarted), StateType.Start)]
    [InlineData(nameof(WorkflowState<MockWorkflowData>.IsRejected), StateType.Rejected)]
    [InlineData(nameof(WorkflowState<MockWorkflowData>.IsCancelled), StateType.Cancelled)]
    public void StateTypeMethods_SetCorrectStateType(string methodName, StateType expectedType)
    {
        // Use reflection to call the method
        var method = typeof(WorkflowState<MockWorkflowData>).GetMethod(methodName);
        method?.Invoke(this._state, null);

        Assert.Equal(expectedType, this._state.Type);
    }

    [Fact]
    public void Action_WithDuplicateName_IgnoresSecondAction()
    {
        this._state.Action("TestAction", "NextState");
        this._state.Action("TestAction", "DifferentNextState");

        Assert.Single(this._state.Actions);
        Assert.Equal("NextState", this._state.Actions[0].NextState);
    }

    [Fact]
    public void Action_WithConfiguration_AppliesConfigurationCorrectly()
    {
        const string actionName = "TestAction";
        const string nextState = "NextState";
        const bool isAutomatic = true;

        this._state.Action(actionName, nextState, action =>
        {
            action.AssignToRoles("TestRole");
            action.IsAuto = isAutomatic;
            action.Condition = _ => true;
        });

        var action = Assert.Single(this._state.Actions);
        Assert.Multiple(
            () => Assert.Equal(actionName, action.Name),
            () => Assert.Equal(nextState, action.NextState),
            () => Assert.Equal(isAutomatic, action.IsAuto),
            () => Assert.Single(action.AssignedRoles, "TestRole"),
            () => Assert.NotNull(action.Condition)
        );
    }

    [Fact]
    public void Action_WithAutoAndCondition_SetsPropertiesCorrectly()
    {
        Func<MockWorkflowData, bool> condition = _ => true;
        this._state.Action("AutoAction", "NextState", action =>
        {
            action.IsAuto = true;
            action.Condition = condition;
        });

        var action = Assert.Single(this._state.Actions);
        Assert.Multiple(
            () => Assert.True(action.IsAuto),
            () => Assert.Same(condition, action.Condition)
        );
    }

    [Fact]
    public void Constructor_InitializesCollectionsEmpty()
    {
        var state = new WorkflowState<MockWorkflowData>("TestState");

        Assert.Multiple(
            () => Assert.Empty(state.Actions),
            () => Assert.Empty(state.OnEnterHooks),
            () => Assert.Empty(state.OnExitHooks)
        );
    }
}