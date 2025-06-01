# Meridian Workflow

A lightweight, developer-first workflow engine built for .NET 8+. Define workflows using fluent DSL, handle state transitions, and manage tasks without external dependencies.

## 📚 Table of Contents


- [Why Should I Use Meridian?](#-why-should-i-use-meridian)
- [Core Features](#core-features)
- [Project Structure](#project-structure)
- [Installation](#installation)
- [Get Started in 5 Steps](#get-started-in-5-steps)
- [Feature Deep Dive](#-feature-deep-dive)
- [Hooks (Event Handlers)](#-hooks-event-handlers)
- [Action Authorization](#-action-authorization)
- [Auto Actions](#auto-actions)
- [File Attachments](#-file-attachments)
- [Tasks Per Action](#tasks-per-action)
- [Visual Debugging (Console Flowchart)](#-visual-debugging-console-flowchart)
- [Architecture](#-architecture)
- [Extending Meridian Workflow](#-extending-meridian-workflow)
- [Sample Projects](#-sample-projects-soon)
- [Use Cases](#-use-cases)
- [Roadmap](#-roadmap)
- [Contributing](#-contributing)
- [Status / Limitations](#-status--limitations)
- [License](#-license)


## ❓ Why Should I Use Meridian?

Meridian is designed with developers in mind. It offers a clean, type-safe, and highly extensible workflow engine you can embed directly into your .NET 8+ applications without complex configuration or external dependencies.

- ✅ Fully type-safe fluent DSL
- 🔁 Powerful state transition management
- 🧠 Hook system for business logic
- 🔐 Fine-grained role/user-based action authorization
- 📁 Built-in file handling and task generation
- ☁️ Minimal dependencies and cloud-ready
- 🧩 Clean architecture with plug-and-play extensions
- ⚡ Suitable for microservices or monoliths


## Core Features

### Workflow Definition & Execution
- 🎯 **Type-Safe Workflow DSL**
  - Fluent API for intuitive workflow definitions
  - Compile-time type checking
  - Built-in validation
- 🔄 **State Management**
  - Multi-step transitions
  - State entry/exit hooks
  - Auto-actions support
  - State-specific validation rules

### Advanced Hook System
- ⚡ **Flexible Execution Models**
  - Parallel hook execution for independent operations
  - Sequential execution for dependent operations
  - Critical and non-critical hook handling
- 🔌 **Hook Types**
  - Workflow-level hooks
  - State-specific hooks (OnEnter/OnExit)
  - Custom hook implementation support

### Security & Authorization
- 🔐 **Fine-grained Access Control**
  - Role-based authorization
  - Group-based permissions
  - Action-level security
  - User context awareness

### File Management
- 📁 **Built-in File Handling**
  - Pluggable storage providers
  - File upload/download operations
  - Attachment metadata management
  - Support for multiple storage backends
- 🔧 **Storage Configuration**
  - Disabled storage option for non-file workflows
  - Custom provider implementation support

### Task Management
- 📋 **Task Tracking**
  - Automatic task generation
  - Status tracking
  - Assignment to users/roles/groups
  - Task lifecycle management

### Data Handling & Persistence
- 💾 **Flexible Storage**
  - Multiple database support
  - Schema customization
  - Table prefix configuration
- 🔄 **Data Processing**
  - Automatic validation
  - Change tracking
  - JSON-based serialization
  - Data comparison utilities

### Monitoring & History
- 📊 **Comprehensive Tracking**
  - Detailed transition history
  - State change logging
  - User action tracking
  - Timestamp-based auditing

### Error Handling
- ⚠️ **Specialized Exception Handling**
  - Workflow-specific exceptions
  - Detailed error contexts
  - Operation-specific error types
  - Clear error messages

### Architecture & Integration
- 🏗️ **Clean Architecture**
  - Dependency injection ready
  - Interface-based design
  - Extensible components
- 🔌 **Easy Integration**
  - ASP.NET Core support
  - Minimal dependencies
  - Cloud-ready design

  
## Project Structure

The Meridian Workflow project follows a clean architecture pattern with the following structure:

- `src/`
  - `Meridian.Core/`: Core domain logic and entities
  - `Meridian.AspNetCore/`: ASP.NET Core integration
  - `Meridian.Application/`: Application layer (use cases)
  - `Meridian.Infrastructure/`: Infrastructure implementations
- `tests/`: Test projects
- `.git/`: Git repository
- `README.md`: Project documentation
- `LICENSE.txt`: License information
- `Meridian.sln`: Solution file



### Project Organization

- **src/** - Contains all source code organized in different projects:
    - **Meridian.Core**: Contains the core domain logic, entities, and business rules
    - **Meridian.AspNetCore**: Provides integration with ASP.NET Core
    - **Meridian.Application**: Houses application-specific logic and use cases
    - **Meridian.Infrastructure**: Implements infrastructure concerns (persistence, external services)

- **tests/** - Contains all test projects
- **README.md** - Main documentation file
- **LICENSE.txt** - Project license information
- **Meridian.sln** - Visual Studio solution file


## 📦 Installation
> 🔧 NuGet package coming soon...

## 🚀 Get Started in 5 Steps

#### 1. Define Workflow Data Model
```csharp
public class LeaveRequestData : IWorkflowData
{
    public string Reason { get; set; } = string.Empty;
    public int Days { get; set; }
}
```

#### 2. Define the Workflow
```csharp
public class LeaveRequestWorkflow : IWorkflowBootstrapper
{
    public void Register(IWorkflowDefinitionBuilder builder)
    {
        builder.Define<LeaveRequestData>("LeaveRequest", definition =>
        {
            definition.State("Pending", state =>
            {
                state.Action("Approve", "Approved");
                state.Action("Reject", "Rejected");
            });

            definition.State("Approved", state => state.IsCompleted());
            definition.State("Rejected", state => state.IsRejected());
        });
    }
  }
```
#### 3. Register Meridian Workflow Engine
```csharp
builder.Services.AddMeridianWorkflow(options =>
{
    options.Workflows =
    [
        new LeaveRequestWorkflow(),
    ];
});
```

#### 4. Use the Engine

```csharp
public class MyClass
{
    public MyClass(IWorkflowService<LeaveRequestData> leaveRequestWorkflow) 
    {
        // use the leaveRequestWorkflow to create, execute action, get history, get request, get logged-in user tasks, ...etc
    }
}
```
#### 5. Visualize the Workflow (Optional)
```csharp
workflowDefinition.PrintToConsole();
```

## 🛠️ Feature Deep Dive

### ✅ Fluent DSL Definition
* Why? Enables clean, reusable workflow definitions.
* Examples:
```csharp
builder.Define<LeaveRequestData>("LeaveRequest", def =>
{
    def.State("Pending", state =>
    {
        // State configuration
        state.Action("Approve", "Approved", action => 
        {
            // Action configuration
        });
        state.Action("Reject", "Rejected");
    });
});
```

#### Use Definition Templates:

Definition templates help you create reusable workflow patterns and keep your workflow definitions DRY (Don't Repeat Yourself). They are particularly useful when you have common states, actions, or behaviors that appear in multiple workflows.

**🔑 Key Benefits**
- ♻️ Reusable workflow patterns
- 🎯 Consistent behavior across workflows
- 📝 Reduced code duplication
- 🛠️ Easy maintenance
- 🧩 Modular workflow design

**📚 Common Use Cases**

1. **Common States**: Reuse standard states like Approved, Rejected, or UnderReview
2. **Standard Actions**: Apply consistent actions like approve/reject patterns
3. **Security Templates**: Reuse role and permission configurations
4. **Hook Templates**: Apply common hooks across workflows

```csharp
public static class GeneralWorkflowTemplates
{
    public static WorkflowDefinition<InitialApprovalData> WithCommonStates(
        this WorkflowDefinition<InitialApprovalData> workflowDefinition)
    {
        workflowDefinition
            .State(GeneralWorkflowStates.Rejected, state =>
            {
                state.SendToSmartServicesHook();
            })
            .State(GeneralWorkflowStates.Updating, state =>
            {
                state.SendToSmartServicesHook();
            })
            .State(GeneralWorkflowStates.Approved, state =>
            {
                state.SendToSmartServicesHook();
                state.IsCompleted();
            });

        return workflowDefinition;
    }
    
     public static WorkflowState<InitialApprovalData> WithStandardRejectionActions(
        this WorkflowState<InitialApprovalData> state)
    {
        state.Action(GeneralWorkflowActions.Reject, GeneralWorkflowStates.Rejected);
        state.Action(GeneralWorkflowActions.Incomplete, GeneralWorkflowStates.Updating);
        return state;
    }
}

// Usage
public class InitialApprovalWorkflow : IWorkflowBootstrapper
{
    public void Register(IWorkflowDefinitionBuilder builder)
    {
        builder.Define<InitialApprovalData>("InitialApproval", definition =>
        {
            definition
                .WithCommonStates();
                .State(GeneralWorkflowStates.UnderReview, state =>
                {
                    state.WithStandardRejectionActions();
                })
        });
    }
}

// Hooks template
public static class CommonHooks
{
    public static WorkflowState<InitialApprovalData> SendToSmartServicesHook(
        this WorkflowState<InitialApprovalData> state)
    {
        state.AddHook(new WorkflowHookDescriptor<InitialApprovalData>
        {
            Hook = new SendRequestToSmartServices(),
            IsAsync = true,
        }, StateHookType.OnStateEnter);

        return state;
    }
}
```

### 🧠 Hooks (Event Handlers)
* **Purpose:** Execute logic during request lifecycle (create, transition, entry/exit).
* **Types:**
  - Workflow Definition
    - OnCreateHooks (When a new request is created)
    - OnTransitionHooks (When request transitions)
  - State
    - OnEnterHooks (when request enters the state)
    - OnExitHooks (when request exits the state)
  - Action
    - OnExecuteHooks (when user takes an action)

In simple and easy way you can use `AddHook` extension to register hooks, or you can register it using the props.

- Using `AddHook`:
```csharp
// Add Hook to workflow on new request was created
workflowDefinition.AddHook(new WorkflowHookDescriptor<LeaveRequestData>
        {
            Hook = new NewRequestWasCreated(),
            IsAsync = false,
        }
    WorkflowHookType.OnRequestCreated);

// Add Hook to state (on enter)
workflowDefinition
    .State("StateName", state => 
    {
        state.AddHook(new WorkflowHookDescriptor<LeaveRequestData>
        {
            Hook = new SendRequestToSmartServices(),
            IsAsync = true,
        }, StateHookType.OnStateEnter);
    });

// Add Hook to action execution
workflowDefinition
    .State("StateName", state => 
    {
        state.Action("actionName", "targetState", action => 
        {
            action.AddHook(new WorkflowHookDescriptor<LeaveRequestData>
          {
              Hook = new DoSomething(),
              IsAsync = true,
            });
        });
    });
```

#### Define Hooks
Consider rephrasing as “To use a hook like `NewRequestWasCreated`
```csharp
public class NewRequestWasCreated: IWorkflowHook<LeaveRequestData>
{
    public Task ExecuteAsync(WorkflowContext<LeaveRequestData> context)
    {
        return Task.CompletedTask;
    }
}
```

## 🔐 Action Authorization
Assign allowed users, roles, or groups for each action:
```csharp
workflowDefinition
    .State("StateName", state => 
    {
        state.Action("actionName", "targetState", action => 
        {
            action.AssignToGroups("group1", "group2");
            action.AssignToUsers("user1", "user2");
            action.AssignToRoles("role1", "role2", "role3");
        });
    });
```

### Auto Actions
Mark an action to be taken by condition
```csharp
workflowDefinition
    .State("StateName", state => 
    {
        state.Action("actionName", "targetState", action => 
        {
            action.IsAuto = true;
            action.Condition = data => data.Department == "IT" && data.Priority > 10;
        });
    });
```
## 📎 File Attachments
No extra effort needed from developers. Just use:
```csharp
public class LeaveRequestData : IWorkflowData
{
    public WorkflowFile<WorkflowFileAttachment> MedicalReport { get; set; } = new();
}
```
**The engine:**
* Detects attachments
* Uploads via IWorkflowFileStorageProvider
* Replaces them with references
```csharp
public interface IWorkflowFileStorageProvider<TReference>
{
    Task<TReference> UploadAsync(WorkflowFileAttachment attachment);
}
```
You implement this to store on disk, S3, or DB.

### Tasks Per Action
Automatically creates a WorkflowRequestTask per action in a state.
```csharp
public class WorkflowRequestTask
{
    public string RequestId { get; set; }
    public string Action { get; set; }
    public string State { get; set; }
    public List<string> AssignedToRoles { get; set; }
    public WorkflowTaskStatus Status { get; set; }
}
```
**On each transition:**
* ✅ Old tasks marked as completed
* ✅ New tasks created for next state's actions

## 🧪 Visual Debugging (Console Flowchart)
```csharp
workflowDefinition
    .State("StateName", state => 
    {
    })
    // Use this extension
    .PrintToConsole();
```
#### Output
```
══════════════════════════════════════════
Workflow: LeaveRequest
══════════════════════════════════════════

State: Pending
 ├─ Approve → Approved
 └─ Reject  → Rejected

State: Approved
State: Rejected

```

## 📊 Architecture
* Clean separation of Domain / Application / Infrastructure
* Plug-and-play registry-based engine resolver
* Reflection-based workflow registry
* Supports future runtime definitions (JSON)

## 🔌 Extending Meridian Workflow
| Feature                 | You Can Plug In...                              |
|-------------------------|--------------------------------------------------|
| File Upload             | `IWorkflowFileStorageProvider<TReference>`      |
| Hook Execution          | Implement `IWorkflowHook<TData>`                |
| Custom Transition Logic | Add logic in hook or conditions                 |

## 📁 Sample Projects (Soon)
### ✅ Leave Request
* Standard approval, HR hooks
* PDF file attachments

## 🧰 Use Cases

Meridian Workflow can handle a wide variety of business scenarios:

- 📅 **Leave Request Approval**
  - Simple multi-step approval with hooks and auto actions

- 📄 **Document Review Workflow**
  - Handle file uploads, rejections, and resubmissions

- 👥 **HR Onboarding**
  - Automate onboarding steps with role-based task assignments

- 🛠️ **Support Ticket Lifecycle**
  - Escalation, auto-routing, SLA tracking

- 🧾 **Procurement Approvals**
  - Include price validation, PDF verification, and user-specific approvals

- 🔄 **Multi-Level Reviews**
  - Nested approval chains with sub-workflows and task delegation
- MORE AND MORE!!

## 📍 Roadmap

The following features are planned or under consideration for future releases:

- [ ] Timeout & Escalation Support
- [ ] Delegation & Reassignment
- [ ] Multi-request Relationships
- [ ] Sub-Workflows & Nested Workflows
- [ ] JSON-based Workflow Definitions
> 🚧 **More coming soon...**

## 👏 Contributing
Want to help improve Meridian Workflow?
* Report issues
* Submit new DSL extensions
* Create advanced real-world samples
* Help to add Timeout / Escalation
* Help to add Delegation / Reassign
* Help to add Multi-Request Relationship
* Help to add Sub-Workflows
* Help to convert Fluent-DSL to JSON-DSL

## ⚠️ Status / Limitations
> Unit tests are not fully completed.  
Contributions to improve coverage and test edge cases are welcome.

## 📄 License
Apache License 2.0. Free for use in open-source and commercial applications. Includes conditions for redistribution and attribution.
