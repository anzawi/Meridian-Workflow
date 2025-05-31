namespace Meridian.Core.Tests;

using Enums;
using Exceptions;
using Xunit;

public class WorkflowDefinitionTests
{
    private readonly WorkflowDefinition<MockWorkflowData> _workflow = new(WorkflowId);
    private const string WorkflowId = "test-workflow";

    [Fact]
    public void Constructor_SetsIdCorrectly()
    {
        Assert.Equal(WorkflowId, this._workflow.Id);
    }

    [Fact]
    public void Code_GeneratesPascalCaseFromId()
    {
        var workflow = new WorkflowDefinition<MockWorkflowData>("test-workflow-id");
        Assert.Equal("TestWorkflowId", workflow.Code);
    }

    [Fact]
    public void Validate_WithEmptyId_ThrowsWorkflowDefinitionException()
    {
        this._workflow.Id = string.Empty;

        var exception = Assert.Throws<WorkflowDefinitionException>(() => this._workflow.Validate());
        Assert.Contains("must have a non-empty ID", exception.Message);
    }

    [Fact]
    public void Validate_WithNoStates_ThrowsWorkflowDefinitionException()
    {
        var exception = Assert.Throws<WorkflowDefinitionException>(() => this._workflow.Validate());
        Assert.Contains("must define at least one state", exception.Message);
    }

    [Fact]
    public void Validate_WithNoCompletedState_ThrowsWorkflowDefinitionException()
    {
        this._workflow.State("Initial", state => { });

        var exception = Assert.Throws<WorkflowDefinitionException>(() => this._workflow.Validate());
        Assert.Contains("must have a completed state", exception.Message);
    }

    [Fact]
    public void Validate_WithDuplicateStateNames_ThrowsWorkflowStateException()
    {
        this._workflow.State("State1", state => state.IsCompleted());
        this._workflow.State("State1", state => { });

        var exception = Assert.Throws<WorkflowStateException>(() => this._workflow.Validate());
        Assert.Contains("Duplicate state name", exception.Message);
    }

    [Fact]
    public void State_FirstStateIsAutomaticallyMarkedAsStart()
    {
        this._workflow.State("Initial", state => { });

        Assert.Equal(StateType.Start, this._workflow.States[0].Type);
    }

    [Fact]
    public void ValidateActions_WithEmptyActionName_ThrowsWorkflowStateException()
    {
        this._workflow.State("Initial", state => { state.Action("", "Next"); }).State("Next", state => state.IsCompleted());

        var exception = Assert.Throws<WorkflowStateException>(() => this._workflow.Validate());
        Assert.Contains("Has an empty name Action", exception.Message);
    }

    [Fact]
    public void ValidateActions_WithDuplicateActionNames_ThrowsWorkflowStateException()
    {
        this._workflow.State("Initial", state =>
        {
            state.Action("Action1", "Next");
            state.Action("Action1", "Next");
        }).State("Next", state => state.IsCompleted());

        var exception = Assert.Throws<WorkflowStateException>(() => this._workflow.Validate());
        Assert.Contains("Has a duplicated action", exception.Message);
    }

    [Fact]
    public void ValidateActions_WithAutoActionWithoutCondition_ThrowsWorkflowActionException()
    {
        this._workflow.State("Initial", state => { state.Action("Action1", "Next", isAuto: true); })
            .State("Next", state => state.IsCompleted());

        var exception = Assert.Throws<WorkflowActionException>(() => this._workflow.Validate());
        Assert.Contains("Is Auto-action and must have a condition", exception.Message);
    }

    [Fact]
    public void ValidateActions_WithNonAutoActionWithCondition_ThrowsWorkflowActionException()
    {
        this._workflow.State("Initial", state => { state.Action("Action1", "Next", isAuto: false, condition: _ => true); })
            .State("Next", state => state.IsCompleted());

        var exception = Assert.Throws<WorkflowActionException>(() => this._workflow.Validate());
        Assert.Contains("Has a condition but is not marked as Auto-Action", exception.Message);
    }

    [Fact]
    public void ValidateNextStates_WithUndefinedNextState_ThrowsWorkflowActionException()
    {
        this._workflow.State("Initial", state => { state.Action("Action1", "Undefined"); })
            .State("Next", state => state.IsCompleted());

        var exception = Assert.Throws<WorkflowActionException>(() => this._workflow.Validate());
        Assert.Contains("References undefined next state", exception.Message);
    }

    [Fact]
    public void GetOnCreateHistory_WithDefaultValues_ReturnsExpectedTransition()
    {
        this._workflow.State("Initial", _ => { });

        var history = this._workflow.GetOnCreateHistory();

        Assert.Equal("Submitted", history.Action);
        Assert.Equal("Requester", history.PerformedBy);
        Assert.Equal("Initial", history.ToState);
    }

    [Fact]
    public void OverrideOnCreateHistory_SetsCustomHistory()
    {
        var customHistory = new WorkflowTransition
        {
            Action = "CustomAction",
            PerformedBy = "CustomUser",
            ToState = "CustomState"
        };

        this._workflow.OverrideOnCreateHistory(customHistory);
        var history = this._workflow.GetOnCreateHistory();

        Assert.Equal("CustomAction", history.Action);
        Assert.Equal("CustomUser", history.PerformedBy);
        Assert.Equal("CustomState", history.ToState);
    }
}