namespace Meridian.Core.Tests;

using Enums;
using Interfaces;

public class WorkflowStateTests
{
    private readonly WorkflowState<MockWorkflowData> _state;

    public WorkflowStateTests()
    {
        _state = new WorkflowState<MockWorkflowData> { Name = "TestState" };
    }

    [Fact]
    public void Code_GeneratesPascalCaseFromName()
    {
        var state = new WorkflowState<MockWorkflowData> { Name = "test-state" };
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
        method?.Invoke(_state, null);
        
        Assert.Equal(expectedType, _state.Type);
    }

    [Fact]
    public void Action_WithDuplicateName_IgnoresSecondAction()
    {
        _state.Action("TestAction", "NextState");
        _state.Action("TestAction", "DifferentNextState");

        Assert.Single(_state.Actions);
        Assert.Equal("NextState", _state.Actions[0].NextState);
    }

    [Fact]
    public void Action_WithConfiguration_AppliesConfigurationCorrectly()
    {
        const string actionName = "TestAction";
        const string nextState = "NextState";
        const bool isAutomatic = true;
        
        _state.Action(actionName, nextState, 
            config: action => { action.AssignToRoles("TestRole"); },
            isAuto: isAutomatic,
            condition: _ => true);

        var action = Assert.Single(_state.Actions);
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
        _state.Action("AutoAction", "NextState", isAuto: true, condition: condition);

        var action = Assert.Single(_state.Actions);
        Assert.Multiple(
            () => Assert.True(action.IsAuto),
            () => Assert.Same(condition, action.Condition)
        );
    }

    [Fact]
    public void Constructor_InitializesCollectionsEmpty()
    {
        var state = new WorkflowState<MockWorkflowData>();
        
        Assert.Multiple(
            () => Assert.Empty(state.Actions),
            () => Assert.Empty(state.OnEnterHooks),
            () => Assert.Empty(state.OnExitHooks)
        );
    }
}