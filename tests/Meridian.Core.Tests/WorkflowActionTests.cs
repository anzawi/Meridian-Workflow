namespace Meridian.Core.Tests;

using Interfaces;

public class WorkflowActionTests
{
    private readonly WorkflowAction<MockWorkflowData> _action;

    public WorkflowActionTests()
    {
        this._action = new WorkflowAction<MockWorkflowData>
        {
            Name = "TestAction",
            NextState = "NextState"
        };
    }

    [Fact]
    public void Constructor_InitializesCollectionsEmpty()
    {
        var action = new WorkflowAction<MockWorkflowData>();
        
        Assert.Multiple(
            () => Assert.Empty(action.AssignedUsers),
            () => Assert.Empty(action.AssignedRoles),
            () => Assert.Empty(action.AssignedGroups),
            () => Assert.Empty(action.OnExecuteHooks)
        );
    }

    [Fact]
    public void AssignToUser_AddsUserToAssignedUsers()
    {
        // Act
        var result = this._action.AssignToUsers("user1");

        // Assert
        Assert.Multiple(
            () => Assert.Single(this._action.AssignedUsers, "user1"),
            () => Assert.Same(this._action, result)
        );
    }

    [Fact]
    public void AssignToRole_AddsRoleToAssignedRoles()
    {
        // Act
        var result = this._action.AssignToRoles("role1");

        // Assert
        Assert.Multiple(
            () => Assert.Single(this._action.AssignedRoles, "role1"),
            () => Assert.Same(this._action, result)
        );
    }

    [Fact]
    public void AssignToGroup_AddsGroupToAssignedGroups()
    {
        // Act
        var result = this._action.AssignToGroups("group1");

        // Assert
        Assert.Multiple(
            () => Assert.Single(this._action.AssignedGroups, "group1"),
            () => Assert.Same(this._action, result)
        );
    }
    

    [Fact]
    public void IsAssignedTo_WithMatchingUser_ReturnsTrue()
    {
        // Arrange
        this._action.AssignToUsers("user1");

        // Act & Assert
        Assert.Contains("user1", this._action.AssignedUsers);
    }

    [Fact]
    public void IsAssignedTo_WithMatchingRole_ReturnsTrue()
    {
        // Arrange
        this._action.AssignToRoles("role1");

        // Act & Assert
        Assert.Contains("role1", this._action.AssignedRoles);
    }

    [Fact]
    public void IsAssignedTo_WithMatchingGroup_ReturnsTrue()
    {
        // Arrange
        this._action.AssignToGroups("group1");

        // Act & Assert
        Assert.Contains("role1", this._action.AssignedGroups);
    }
}
