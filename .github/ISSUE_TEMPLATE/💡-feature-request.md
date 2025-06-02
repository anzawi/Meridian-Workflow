---
name: "\U0001F4A1 Feature Request"
about: Suggest a new idea or improvement for Meridian Workflow
title: ''
labels: ''
assignees: anzawi

---

## 💡 Feature Request

### 🧩 Is your feature request related to a specific problem?

Please describe the problem or limitation you’re facing.  
For example: *"I'm trying to support sub-approval chains, but the current engine doesn't allow dynamic transition mapping based on external data."*

---

### ✅ Describe the solution you'd like

What would you like **Meridian Workflow** to support?

Be as specific as possible — e.g., a new `IHookCondition<T>` interface, dynamic DSL extensions, timeout behavior, etc.

---

### 🔄 Describe alternatives you’ve considered

If you've explored workarounds or different approaches, describe them here.  
Why do they fall short compared to the feature you're requesting?

---

### 🔧 Suggested API or Usage (Optional)

If you have an idea for what the API or DSL might look like, share a quick example:

```csharp
state.Action("AutoApprove", "Approved", action =>
{
    action.IsAuto = true;
    action.Condition = new ExternalUserApprovalCondition();
});
```

### 📎 Additional Context

Add any other context, similar libraries, screenshots, diagrams, or business use cases that justify the feature.

---

### 📋 Checklist

- [ ] I have checked that this feature is not already requested or implemented.
- [ ] I have checked that this feature is not listed in Roadmap.
- [ ] I am willing to help implement this feature if guidance is provided.
- [ ] I believe this feature will benefit more than one use case or scenario.
