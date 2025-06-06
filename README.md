# Meridian Workflow

A lightweight, developer-first workflow engine built for .NET 8+. Define workflows using fluent DSL, handle state transitions, and manage tasks without external dependencies.

## 📚 Table of Contents


- [Why Should I Use Meridian?](#-why-should-i-use-meridian)
- [Core Features](#core-features)
- [Project Structure](#project-structure)
- [Installation](#-installation)
- [Get Started in 5 Steps](#-get-started-in-5-steps)
- [Feature Deep Dive](#%EF%B8%8F-feature-deep-dive)
- [Hooks (Event Handlers)](#-hooks-event-handlers)
- [Action Authorization](#-action-authorization)
- [Auto Actions](#auto-actions)
- [File Attachments](#-file-attachments)
- [Tasks Per Action](#tasks-per-action)
- [Visual Debugging (Console Flowchart)](#-visual-debugging-console-flowchart)
- [Available builtin Services / IWorkflowService&lt;TData&gt;](#-iworkflowservice)
- [Architecture](#-architecture)
- [Extending Meridian Workflow](#-extending-meridian-workflow)
- [Sample Projects](#-sample-projects)
- [Use Cases](#-use-cases)
- [Meridian vs Elsa vs Workflow Core](#-how-is-meridian-different)
- [Roadmap](#-roadmap)
- [Contributing](#-contributing)
- [Status / Limitations](#%EF%B8%8F-status--limitations)
- [License](#-license)
- [IWorkflowService&lt;TData&gt;](#-iworkflowservicetdata)

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

> 💡 **Note:** Not all workflow engines are the same.  
> **Meridian** is focused on **state-based, human-driven workflows** (e.g., approvals, reviews),  
> unlike general-purpose engines such as **Workflow Core** or **Elsa**.  
> 👉 See [How is Meridian Different?](#-how-is-meridian-different) for a detailed comparison.

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
    
    // other options...
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
    public static WorkflowDefinition<LeaveRequestData> WithCommonStates(
        this WorkflowDefinition<LeaveRequestData> workflowDefinition)
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
    
     public static WorkflowState<LeaveRequestData> WithStandardRejectionActions(
        this WorkflowState<LeaveRequestData> state)
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
        builder.Define<LeaveRequestData>("InitialApproval", definition =>
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
    public static WorkflowState<LeaveRequestData> SendToSmartServicesHook(
        this WorkflowState<LeaveRequestData> state)
    {
        state.AddHook(new WorkflowHookDescriptor<LeaveRequestData>
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
  - **Workflow Definition**
    - `OnCreateHooks` (when a new request is created)
    - `OnTransitionHooks` (when request transitions)
  - **State**
    - `OnEnterHooks` (when request enters the state)
    - `OnExitHooks` (when request exits the state)
  - **Action**
    - `OnExecuteHooks` (when a user takes an action)

You can use the `AddHook` extension method in three ways:

1. Pass a `WorkflowHookDescriptor<TData>` (full control)
2. Pass a class implementing `IWorkflowHook<TData>`
3. Pass a lambda delegate `Func<WorkflowContext<TData>, Task>`

---

#### ✅ Add Hook to Workflow (using WorkflowHookDescriptor)
```
workflowDefinition.AddHook(new WorkflowHookDescriptor<LeaveRequestData>
{
    Hook = new NewRequestWasCreated(),
    IsAsync = false,
    LogExecutionHistory = true
}, WorkflowHookType.OnRequestCreated);
```

#### ✅ Add Hook to Workflow (using class)
```
workflowDefinition.AddHook(
    new NewRequestWasCreated(),
    cfg => {
        cfg.IsAsync = false;
    },
    WorkflowHookType.OnRequestCreated);
```

#### ✅ Add Hook to Workflow (using lambda)
```
workflowDefinition.AddHook(
    async ctx =>
    {
        Console.WriteLine($"New request for {ctx.InputData?.EmployeeName}");
        await Task.CompletedTask;
    },
    cfg => {
        cfg.IsAsync = true;
        cfg.LogExecutionHistory = false;
    },
    WorkflowHookType.OnRequestCreated);
```

#### ✅ Add Hook to State (using WorkflowHookDescriptor)
```
workflowDefinition.State("StateName", state =>
{
    state.AddHook(new WorkflowHookDescriptor<LeaveRequestData>
    {
        Hook = new SendRequestToSmartServices(),
        IsAsync = true,
    }, StateHookType.OnStateEnter);
});
```

#### ✅ Add Hook to State (using class)
```
workflowDefinition.State("StateName", state =>
{
    state.AddHook(
        new SendRequestToSmartServices(),
        cfg => {
            cfg.IsAsync = true;
        },
        StateHookType.OnStateEnter);
});
```

#### ✅ Add Hook to State (using lambda)
```
workflowDefinition.State("StateName", state =>
{
    state.AddHook(
        async ctx =>
        {
            Console.WriteLine("Entered state");
            await Task.CompletedTask;
        },
        cfg => {
            cfg.IsAsync = true;
        },
        StateHookType.OnStateEnter);
});
```

#### ✅ Add Hook to Action (using WorkflowHookDescriptor)
```
workflowDefinition.State("StateName", state =>
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

#### ✅ Add Hook to Action (using class)
```
workflowDefinition.State("StateName", state =>
{
    state.Action("actionName", "targetState", action =>
    {
        action.AddHook(
            new DoSomething(),
            cfg => {
                cfg.IsAsync = true;
            });
    });
});
```

#### ✅ Add Hook to Action (using lambda)
```
workflowDefinition.State("StateName", state =>
{
    state.Action("actionName", "targetState", action =>
    {
        action.AddHook(
            async ctx =>
            {
                Console.WriteLine("Action executed");
                await Task.CompletedTask;
            },
            cfg => {
                cfg.IsAsync = true;
            });
    });
});
```

#### 🧩 Define a Hook Class
```
public class NewRequestWasCreated : IWorkflowHook<LeaveRequestData>
{
    public Task ExecuteAsync(WorkflowContext<LeaveRequestData> context)
    {
        Console.WriteLine("Hook class executed");
        return Task.CompletedTask;
    }
}
```


#### Use Builtin Hooks (reusable hooks)
Meridian Workflow provides built-in reusable hooks that simplify common workflow behaviors. One such hook is:

##### 🧩 `CompareDataAndLogHook<TData>`

This hook compares the existing request data with the new input data during a transition and logs all field-level changes to the request history.
It's useful for audit trails and understanding how data evolved over time.

##### 🔧 Usage

Add the hook to a request transition:
```csharp
definition.AddCompareDataAndLogHistory();
```

Or attach it manually to any hook-supported point (e.g., action execution):
```csharp
action.AddHook(new WorkflowHookDescriptor<LeaveRequestData> 
{
    Hook = new CompareDataAndLogHook<LeaveRequestData>(),
    Mode = HookExecutionMode.Parallel,
    IsAsync = false,
    LogExecutionHistory = false,
});

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

### Conditional Actions

Meridian Workflow supports **conditional transitions** that dynamically determine the next state based on the data being processed.
This enables flexible workflows that adapt to runtime conditions.

## Basic Usage

```csharp
state.Action("Approve", "PendingManagerApproval", action => action
    .AssignToRoles("Manager")
    .When(data => data.Amount > 10000, "PendingDirectorApproval")
    .When(data => data.Amount > 5000, "PendingSupervisorApproval")
);
```
## What This Does

- If `Amount > 10000`, transitions to `"PendingDirectorApproval"`
- If `Amount > 5000` and `<= 10000`, transitions to `"PendingSupervisorApproval"`
- Otherwise, transitions to the default state `"PendingManagerApproval"`

---

## How It Works

- Conditions are evaluated in the order they are defined.
- The first condition that returns `true` determines the transition.
- If no condition matches, the default transition is used.

---

## Best Practices

### 1. Order Matters

Place more specific conditions before more general ones:

```csharp
.When(data => data.Amount > 10000, "HighValueApproval") // Specific
.When(data => data.Amount > 1000, "StandardApproval")   // General
```
### 2. Provide a Clear Default

Always specify a meaningful default state:

```csharp
state.Action("Review", "StandardReview", action => action
    .When(data => data.IsUrgent, "ExpeditedReview")
    // Falls back to "StandardReview" if not urgent
);
```
### 3. Avoid Overlapping Conditions

Ensure that conditions are mutually exclusive:

```csharp
// Good - Conditions don't overlap
.When(data => data.Days > 30, "ExtendedLeave")
.When(data => data.Days > 15 && data.Days <= 30, "StandardLeave")

// Bad - Overlapping conditions
.When(data => data.Days > 15, "StandardLeave")
.When(data => data.Days > 30, "ExtendedLeave") // Never reached!
```
### Transition Tables (Multi-Condition Routing)

For complex conditional transitions, Meridian supports a more expressive syntax using a **transition table**. This allows you to define multiple branching paths in a single statement.

#### Example

```csharp
state.Action("Approve", action => action.TransitionTo(
    (data => data.Amount > 10000, "PendingDirectorApproval", "Amount > 10000"),
    (data => data.Amount > 5000, "PendingSupervisorApproval", "Amount > 5000"),
    (data => true, "PendingManagerApproval", "Default")
));
```

#### What This Does

- Transitions to `"PendingDirectorApproval"` if `Amount > 10000`
- Otherwise, transitions to `"PendingSupervisorApproval"` if `Amount > 5000`
- Otherwise, transitions to `"PendingManagerApproval"` (default case)

#### Benefits

- Centralizes all conditional transitions in a single block
- Improves readability and maintainability
- Enables optional labeling for documentation and debugging

#### Optional Overload

You can omit labels if not needed:

```csharp
state.Action("Approve", action => action.TransitionTo(
    (data => data.Amount > 10000, "PendingDirectorApproval"),
    (data => data.Amount > 5000, "PendingSupervisorApproval"),
    (data => true, "PendingManagerApproval")
));
```

---

### How Transition Tables Work

- All conditions are evaluated **in order**.
- The **first match** determines the next state.
- Labels (optional) are used for visual tools and logs.
- Transition tables override `.When(...)` if both are defined.

---

### Best Practices for Transition Tables

- Keep the fallback case `data => true` as the **last rule**.
- Use labels to describe business rules for better debugging and documentation.
- Do not mix `.When(...)` and `.TransitionTo(...)` unless intentional — only one will be used.

```csharp
// Good
.TransitionTo(
    (d => d.Type == "Annual", "AnnualReview", "Annual leave"),
    (d => d.Type == "Sick", "MedicalReview", "Sick leave"),
    (d => true, "DefaultReview", "Fallback")
)
```

```csharp
// Avoid this unless you know what you're doing
.When(d => d.Type == "Urgent", "Expedited")
.TransitionTo((d => true, "Standard"))
```



### Validate Model in Action
Meridian Workflow performs **automatic model validation** before executing an action to ensure data integrity.

#### 🔧 Disabling Auto-Validation

To disable automatic validation on a specific action:

```csharp
workflowDefinition
    .State("StateName", state => 
    {
        state.Action("actionName", "targetState", action => 
        {
            action.DisableAutoValidation();
        });
    });
```

#### ✨ Defining Custom Validation Logic
You can also define custom validation rules using the WithValidation method. This allows fine-grained, context-specific validation before the action executes.

```csharp
action.WithValidation(data =>
{
    var errors = new List<string>();
    if (string.IsNullOrEmpty(data.Department))
    {
        errors.Add("Department cannot be empty");
    }
    
    return errors;
});
```

## 📎 File Attachments
No extra effort is needed from developers. Just use:
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

**Example**
```csharp
public class AttachmentReference
{
    public Guid AttachmentId { get; set; }
    public string Path { get; set; } = string.Empty;
    public string? Source { get; set; }
}

public class WorkflowFileStorageProvider : IWorkflowFileStorageProvider<AttachmentReference>
{
    public async Task<AttachmentReference> UploadAsync(IWorkflowAttachment attachmentFile)
    {
        return await Task.FromResult(new AttachmentReference
        {
            Path = "File path",
            Source = "File source",
            AttachmentId = Guid.NewGuid(),
        });
    }
}
```
Register the File Storage Provider
```csharp
builder.Services.AddMeridianWorkflow(options =>
{
    options.EnableAttachmentProcessor = true; // Optional, true by default
    options.SetFileStorageProvider(typeof(WorkflowFileStorageProvider));
    // other options...
});
```
> 💡 Set `EnableAttachmentProcessor = false` to disable built-in attachment processing if you need full control.

## Database Configuration (EF Core support)
Meridian Workflow supports **Entity Framework Core** out of the box, **NOT FULLY TESTED with all providers**:

> You can integrate any EF Core-supported provider (PostgreSQL, SQLite, Oracle, etc.) by configuring the underlying `DbContext`.

```csharp
builder.Services.AddMeridianWorkflow(options =>
{
     options.ConfigureDb(db =>
   {
       db.Use(dbOptions => dbOptions.UseInMemoryDatabase("WorkflowTestDb"));
       // db.Use(dbOptions => dbOptions.UseSqlServer("WorkflowTestDb"));
       db.TablesPrefix = "Meridian_"; // Optional: Set custom table prefix
       db.Schema = "MySchema"; // Optional: Set sechma (required with oracle)
   });
});
```
> 🔒 Schema and table prefixing allow you to isolate workflow data in shared databases.


## Tasks Per Action
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

## 🔧 IWorkflowService<TData>

Provides all operations to manage and execute workflow requests for a specific workflow type.

---

### 📄 Available Methods

| Method                        | Description                                              |
|------------------------------|----------------------------------------------------------|
| `CreateRequestAsync`         | Creates a new workflow request instance                  |
| `DoActionAsync`              | Executes a specific action on a workflow request         |
| `GetRequestAsync`            | Retrieves a workflow request by ID                       |
| `GetUserTasksAsync`          | Gets requests assigned to a specific user                |
| `GetAvailableActions`        | Gets available actions for a request                     |
| `GetCurrentState`            | Gets the current state of a request                      |
| `GetRequestHistoryAsync`     | Returns transition history for a request                 |
| `GetRequestWithHistoryAsync` | Returns request and its history together                 |

---

### 🧩 CreateRequestAsync

**Description:**  
Creates a new workflow request for the current workflow definition.

**Parameters:**

| Name        | Type   | Required | Description                     |
|-------------|--------|----------|---------------------------------|
| `inputData` | TData  | ✅        | Initial workflow input data     |
| `createdBy` | string | ✅        | ID of the user creating request |

**Example:**

```
await workflow.CreateRequestAsync(new LeaveRequestData { LeaveType = "Annual" }, "user123");
```

---

### 🧩 DoActionAsync

**Description:**  
Executes a specific action (e.g., "Approve", "Reject") on an existing request.

**Overloads:**

- With only action and request ID  
- With additional data to update the workflow

**Parameters:**

| Name         | Type          | Required | Description                        |
|--------------|---------------|----------|------------------------------------|
| `requestId`  | Guid          | ✅        | ID of the request to act on        |
| `action`     | string        | ✅        | Action name to execute             |
| `performedBy`| string        | ✅        | User performing the action         |
| `userRoles`  | List<string>  | ✅        | User's roles                       |
| `userGroups` | List<string>  | ✅        | User's groups                      |
| `data`       | TData?        | ❌        | Optional updated request data      |

**Example:**

```
await workflow.DoActionAsync(requestId, "Submit", "user123", roles, groups);  
await workflow.DoActionAsync(requestId, "Update", "user123", roles, groups, newData);
```

---

### 🧩 GetRequestAsync

**Description:**  
Returns the workflow request by ID.

**Parameters:**

| Name        | Type | Required | Description               |
|-------------|------|----------|---------------------------|
| `requestId` | Guid | ✅        | ID of the workflow request|

**Example:**

```
var request = await workflow.GetRequestAsync(id);
```

---

### 🧩 GetUserTasksAsync

**Description:**  
Returns all requests where the user has available actions based on role/group.

**Parameters:**

| Name         | Type          | Required | Description              |
|--------------|---------------|----------|--------------------------|
| `userId`     | string        | ✅        | ID of the user           |
| `userRoles`  | List<string>  | ✅        | List of user's roles     |
| `userGroups` | List<string>  | ✅        | List of user's groups    |

**Example:**

```
var tasks = await workflow.GetUserTasksAsync("user123", roles, groups);
```

---

### 🧩 GetAvailableActions

**Description:**  
Returns the list of actions the user can perform on the request.

**Overloads:**

- By passing the request object  
- By passing the request ID

**Parameters (Overload 1):**

| Name         | Type                             | Required | Description             |
|--------------|----------------------------------|----------|-------------------------|
| `request`    | WorkflowRequestInstance<TData>   | ✅        | Workflow request object |
| `userId`     | string?                          | ❌        | ID of the user          |
| `userRoles`  | List<string>?                    | ❌        | Roles of the user       |
| `userGroups` | List<string>?                    | ❌        | Groups of the user      |

**Parameters (Overload 2):**

| Name         | Type          | Required | Description           |
|--------------|---------------|----------|-----------------------|
| `requestId`  | Guid          | ✅        | ID of the request     |
| `userId`     | string?       | ❌        | ID of the user        |
| `userRoles`  | List<string>? | ❌        | Roles of the user     |
| `userGroups` | List<string>? | ❌        | Groups of the user    |

**Examples:**

```
workflow.GetAvailableActions(request, "user", roles);  
await workflow.GetAvailableActions(requestId, "user", roles);
```

---

### 🧩 GetCurrentState

**Description:**  
Returns the current state of the specified workflow request.

**Parameters:**

| Name      | Type                           | Required | Description         |
|-----------|--------------------------------|----------|---------------------|
| `request` | WorkflowRequestInstance<TData> | ✅        | Request to check    |

**Example:**

```
var state = workflow.GetCurrentState(request);
```

---

### 🧩 GetRequestHistoryAsync

**Description:**  
Returns the full transition history for a request.

**Parameters:**

| Name        | Type | Required | Description         |
|-------------|------|----------|---------------------|
| `requestId` | Guid | ✅        | ID of the request   |

**Example:**

```
var history = await workflow.GetRequestHistoryAsync(request.Id);
```

---

### 🧩 GetRequestWithHistoryAsync

**Description:**  
Returns both request and its full transition history.

**Parameters:**

| Name        | Type | Required | Description         |
|-------------|------|----------|---------------------|
| `requestId` | Guid | ✅        | ID of the request   |

**Example:**

```
var full = await workflow.GetRequestWithHistoryAsync(request.Id);
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

## 📁 Sample Projects
### ✅ Leave Request
> 🚧 **More coming soon...**

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

## 🔍 How is Meridian Different?

Meridian Workflow is not a replacement for heavy orchestration engines like Elsa, nor is it a drop-in alternative to Workflow Core. Each serves a different use case and design philosophy.

### Meridian vs Workflow Core

| Feature                     | **Meridian**                                        | **Workflow Core**                     |
|----------------------------|-----------------------------------------------------|----------------------------------------|
| Workflow Type              | State Machine (User/Approval-driven)                | Step-based/Flow-based                  |
| DSL                        | Fully type-safe Fluent API                          | Fluent + JSON                          |
| Use Case Focus             | Approval workflows, human interaction, business tasks | General-purpose orchestration        |
| Task Handling              | Built-in task system with roles/users/groups        | Requires custom implementation         |
| Authorization              | Built-in role/group/user-based action authorization | Not built-in                           |
| Extensibility              | Hooks, Templates, Pluggable Features                | Middleware/step extensions             |
| Simplicity & Dev Focus     | Lightweight, zero-config, developer-first           | More generic, more boilerplate         |

### Meridian vs Elsa

| Feature                     | **Meridian**                                        | **Elsa Workflows**                     |
|----------------------------|-----------------------------------------------------|----------------------------------------|
| Workflow Type              | Human Workflow / Approval-based                     | Activity-based Orchestration           |
| Designer UI                | ❌ Not available                                     | ✅ Powerful designer (optional)        |
| DSL                        | ✅ Fluent API (C#)                                   | C# or JSON                             |
| Persistence Model          | Optional / Lightweight                              | Required (workflow instance tracking)  |
| Approval & Action Model    | ✅ States, Actions, Users, Tasks                     | ❌ Not native                           |
| Suitable For               | Leave requests, ticket lifecycle, business processes | Long-running workflows, integrations   |

---

> Meridian is designed for **human-centric workflows** like approvals, reviews, multi-step user processes, and role-based transitions — all using clean, extensible code without a designer or runtime engine.

## 📍 Roadmap

The following features are planned or under consideration for future releases:

- [ ] Timeout & Escalation Support
- [ ] Delegation & Reassignment
- [ ] Multi-request Relationships
- [ ] Sub-Workflows & Nested Workflows
- [ ] Conditional Transitions Upgrade  
  (Take a look at **Conditional Actions** in the _Status / Limitations_ section)  
- [ ] JSON-based Workflow Definitions
> 🚧 **More coming soon...**

## 👏 Contributing
Want to help improve Meridian Workflow?
* Report issues
* Submit new DSL extensions
* Create advanced real-world samples
* Help to add reusable hook
* Help to add Timeout / Escalation
* Help to add Delegation / Reassign
* Help to add Multi-Request Relationship
* Help to add Sub-Workflows
* Help to convert Fluent-DSL to JSON-DSL

## ⚠️ Status / Limitations
> Unit tests are not fully completed.  
>Contributions to improve coverage and test edge cases are welcome.

- Conditional Actions:
  - Conditions are evaluated in definition order.
  - No priority system for condition evaluation.
  - No built-in conflict detection for overlapping conditions.
  - No support for transition-specific validation or hooks.

## 📄 License
Apache License 2.0. Free for use in open-source and commercial applications. Includes conditions for redistribution and attribution.


## 🔧 IWorkflowService<TData>

Provides all operations to manage and execute workflow requests for a specific workflow type.