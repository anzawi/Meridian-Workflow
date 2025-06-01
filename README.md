# Meridian Workflow

A lightweight, developer-first workflow engine built for .NET 8+. Define workflows using fluent DSL, handle state transitions, and manage tasks without external dependencies.

## Core Features

### Engine & Architecture
- 🚀 Generic Workflow Engine (`WorkflowEngine<TData>`)
- 💉 Dependency Injection Ready
- 🔌 Pluggable Storage (In-Memory, SQLite, SQL Server)
- 🎯 Strongly-Typed Workflow DSL
- 🔧 Extensible Design with Clean Architecture

### Workflow Definition & Execution
- ⚡ Fluent DSL for workflow definitions
- 🔄 Multi-step transitions
- 🎯 State & Action Management
- 🤖 Auto-Actions support
- 📋 Task Generation for action-based workflows

### Development & Debugging
- 🔍 Simulatable & Debuggable workflows
- 📊 Console-based workflow visualization
- 🐛 Built-in debugging support
- 🧪 In-Memory Testing Support

### Security & Authorization
- 👥 Role-based Authorization
- 👤 User-level Permissions
- 🔑 Group-based Access Control

### Extensions & Plugins
- 🪝 Lifecycle Hooks (On Request Create, On Request Transition, On Action Execution, On State Enter/Exit)
- 📎 File Attachments System (Builtin Attachment Handling)
- ⚙️ Custom Storage Providers

### Integration
- 📦 Entity Framework Core Integration
- 🔗 Clean Service Architecture

## Getting Started

Check our [GitHub repository](https://github.com/meridianww/meridian-workflows) for installation instructions and examples.


# Project Structure

The Meridian Workflow project follows a clean architecture pattern with the following structure:

Meridian/
├── src/                          # Source code
│   ├── Meridian.Core/           # Core domain logic and entities
│   ├── Meridian.AspNetCore/     # ASP.NET Core integration
│   ├── Meridian.Application/    # Application layer (use cases)
│   └── Meridian.Infrastructure/ # Infrastructure implementations
├── tests/                       # Test projects
├── .git/                        # Git repository
├── .idea/                       # IDE settings
├── README.md                    # Project documentation
├── LICENSE.txt                  # License information
└── Meridian.sln                # Solution file

## Project Organization

- **src/** - Contains all source code organized in different projects:
    - **Meridian.Core**: Contains the core domain logic, entities, and business rules
    - **Meridian.AspNetCore**: Provides integration with ASP.NET Core
    - **Meridian.Application**: Houses application-specific logic and use cases
    - **Meridian.Infrastructure**: Implements infrastructure concerns (persistence, external services)

- **tests/** - Contains all test projects
- **README.md** - Main documentation file
- **LICENSE.txt** - Project license information
- **Meridian.sln** - Visual Studio solution file

## 🚀 Get Started

#### 1. Define Workflow Data
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

### 🧠 Hooks (Event Handlers)
* **Purpose:** Execute logic during request lifecycle (create, transition, entry/exit).
* **Types:**
  - Workflow Definition
    - OnCreateHooks (When a new request is created)
    - OnTransitionHooks (when request transition)
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
              Hook = new DoSomthing(),
              IsAsync = true,
            });
        });
    });
```

#### Define Hooks
To define a hook to be used with hooks such as `Hook = new NewRequestWasCreated()`
Just create a new class and implement `IWorkflowHook<TData>`
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
            action.AssignToGroups("group1", "groupe2");
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
* ✅ Old task marked Completed
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

## 👏 Contributing
Want to help improve MiniFlow?
* Report issues
* Submit new DSL extensions
* Create advanced real-world samples
* Help to convert Fluent-DSL to JSON-DSL

## 📄 License
Apache License 2.0. Free for use in open-source and commercial applications. Includes conditions for redistribution and attribution.
