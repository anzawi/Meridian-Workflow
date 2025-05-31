namespace Meridian.Application.Extensions;

using Core;
using Core.Enums;
using Core.Interfaces;

/// <summary>
/// Provides extension methods to enhance the functionality of workflow elements,
/// such as workflow actions, states, and definitions, by allowing the addition of hooks,
/// validation, and other behavioral modifications in a DSL (Domain-Specific Language)-style manner.
/// </summary>
public static class WorkflowDslExtensions
{
    /// <summary>
    /// Disables automatic validation for the current workflow action.
    /// </summary>
    /// <typeparam name="TData">The type of workflow data associated with the action, implementing <see cref="IWorkflowData"/>.</typeparam>
    /// <param name="action">The workflow action for which automatic validation is to be disabled.</param>
    /// <returns>The same workflow action with automatic validation disabled.</returns>
    public static WorkflowAction<TData> DisableAutoValidation<TData>(this WorkflowAction<TData> action)
        where TData : class, IWorkflowData
    {
        action.UseAutomaticValidation = false;
        return action;
    }

    /// <summary>
    /// Adds input validation to the workflow action using the specified validation function.
    /// </summary>
    /// <typeparam name="TData">The type of workflow data associated with the action.
    /// It must implement the <see cref="IWorkflowData"/> interface.</typeparam>
    /// <param name="action">The workflow action to which validation will be applied.</param>
    /// <param name="validator">A function that takes the workflow data as input and returns
    /// a list of validation error messages. If the list is empty, the input is considered valid.</param>
    /// <returns>The updated workflow action with the specified validation applied.</returns>
    public static WorkflowAction<TData> WithValidation<TData>(this WorkflowAction<TData> action,
        Func<TData, List<string>> validator)
        where TData : class, IWorkflowData
    {
        action.ValidateInput = validator;
        return action;
    }

    /// <summary>
    /// Adds a hook to a workflow definition with the specified descriptor and hook type.
    /// </summary>
    /// <typeparam name="TData">
    /// The type of the workflow data, which must implement <see cref="IWorkflowData"/>.
    /// </typeparam>
    /// <param name="def">The workflow definition to which the hook will be added.</param>
    /// <param name="descriptor">The hook descriptor defining the hook's properties and behavior.</param>
    /// <param name="hookType">
    /// Optional. Specifies the type of hook to be added. Default value is <see cref="WorkflowHookType.OnRequestCreated"/>.
    /// </param>
    /// <returns>
    /// The updated workflow definition with the added hook.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the specified hook type is not recognized.
    /// </exception>
    public static WorkflowDefinition<TData> AddHook<TData>(
        this WorkflowDefinition<TData> def,
        WorkflowHookDescriptor<TData> descriptor,
        WorkflowHookType hookType = WorkflowHookType.OnRequestCreated)
        where TData : class, IWorkflowData
    {
        switch (hookType)
        {
            case WorkflowHookType.OnRequestCreated:
                def.OnCreateHooks.Add(descriptor);
                break;
            case WorkflowHookType.OnRequestTransition:
                def.OnTransitionHooks.Add(descriptor);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(hookType), hookType, null);
        }

        return def;
    }

    /// <summary>
    /// Adds a workflow hook to the specified workflow state. The hook will be triggered based on the provided hook type (OnStateEnter or OnStateExit).
    /// </summary>
    /// <typeparam name="TData">The type of workflow data associated with the workflow state.</typeparam>
    /// <param name="state">The workflow state to which the hook will be added.</param>
    /// <param name="descriptor">The descriptor of the hook specifying its behavior, execution mode, and configuration.</param>
    /// <param name="hookType">The type of state hook to determine when the hook should be executed (e.g., OnStateEnter, OnStateExit).</param>
    /// <returns>The updated workflow state with the added hook.</returns>
    public static WorkflowState<TData> AddHook<TData>(
        this WorkflowState<TData> state,
        WorkflowHookDescriptor<TData> descriptor,
        StateHookType hookType)
        where TData : class, IWorkflowData
    {
        switch (hookType)
        {
            case StateHookType.OnStateEnter:
                state.OnEnterHooks.Add(descriptor);
                break;
            case StateHookType.OnStateExit:
                state.OnExitHooks.Add(descriptor);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(hookType), hookType, null);
        }

        return state;
    }

    /// <summary>
    /// Adds the specified hook descriptor to all actions within the workflow state.
    /// </summary>
    /// <typeparam name="TData">The type of the workflow data.</typeparam>
    /// <param name="state">The workflow state to which the hook will be added.</param>
    /// <param name="descriptor">The hook descriptor to be added to all actions within the state.</param>
    /// <returns>The updated workflow state with the hook added to all actions.</returns>
    public static WorkflowState<TData> AddHookForAllActions<TData>(
        this WorkflowState<TData> state,
        WorkflowHookDescriptor<TData> descriptor)
        where TData : class, IWorkflowData
    {
        foreach (var action in state.Actions)
        {
            action.AddHook(descriptor);
        }

        return state;
    }

    /// Adds a hook to execute during the workflow action's execution phase.
    /// <param name="action">The workflow action to which the hook will be added.</param>
    /// <param name="descriptor">The descriptor that contains the hook definition and execution details.</param>
    /// <typeparam name="TData">The type of the workflow data associated with the action.</typeparam>
    /// <returns>The updated workflow action with the added hook.</returns>
    public static WorkflowAction<TData> AddHook<TData>(
        this WorkflowAction<TData> action,
        WorkflowHookDescriptor<TData> descriptor)
        where TData : class, IWorkflowData
    {
        action.OnExecuteHooks.Add(descriptor);
        return action;
    }

    /// <summary>
    /// Prints the details of the workflow definition to the console for debugging purposes.
    /// This method should not be used in production environments and is intended only for development.
    /// </summary>
    /// <typeparam name="TData">The type of the workflow data associated with this workflow definition.</typeparam>
    /// <param name="def">The workflow definition to be printed to the console.</param>
    public static void PrintToConsole<TData>(this WorkflowDefinition<TData> def)
        where TData : class, IWorkflowData
    {
        const int defaultWidth = 70;
        var separator = new string('═', defaultWidth);

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine(separator);
        Console.WriteLine("This output is generated by 'PrintToConsole'!");
        Console.WriteLine("⚠ Do NOT use this in production. This is for development purposes only.");
        Console.WriteLine(separator);
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Workflow: {def.Id}");
        Console.WriteLine(separator + "\n");
        Console.ResetColor();

        PrintHookGroup("On Request Created Hooks", def.OnCreateHooks);
        PrintHookGroup("On Request Transition Hooks", def.OnTransitionHooks);
        Console.WriteLine();

        foreach (var state in def.States)
        {
            var lines = new List<string>
            {
                "\x1b[1;36mState: " + state.Name + "\x1b[0m", // Cyan
            };


            if (state.OnEnterHooks.Count != 0 || state.OnExitHooks.Count != 0)
            {
                lines.Add("Hooks:");
                if (state.OnEnterHooks.Count != 0)
                    lines.Add($"  ⮡ OnEnter: {FormatHookList(state.OnEnterHooks)}");
                if (state.OnExitHooks.Count != 0)
                    lines.Add($"  ⮡ OnExit:  {FormatHookList(state.OnExitHooks)}");
            }

            if (state.Actions.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                lines.Add("• No actions");
                Console.ResetColor();
            }
            else
            {
                foreach (var action in state.Actions)
                {
                    lines.Add($"\x1b[1;32m• Action: {action.Name}\x1b[0m"); // Green

                    if (action.IsAuto)
                        lines.Add($"   ⮡ Auto Condition: {action.Condition}");

                    if (action.ValidateInput != null)
                        lines.Add($"   ⮡ Custom Validation: ENABLED");

                    if (action.UseAutomaticValidation)
                        lines.Add($"   ⮡ Auto Validation: ENABLED");

                    lines.Add($"   ⮡ Next State → {action.NextState}");

                    if (action.AssignedUsers.Count != 0 || action.AssignedRoles.Count != 0 ||
                        action.AssignedGroups.Count != 0)
                    {
                        lines.Add("   ⮡ Assigned To:");
                        if (action.AssignedUsers.Count != 0)
                            lines.Add($"      • Users:  {string.Join(", ", action.AssignedUsers)}");
                        if (action.AssignedRoles.Count != 0)
                            lines.Add($"      • Roles:  {string.Join(", ", action.AssignedRoles)}");
                        if (action.AssignedGroups.Count != 0)
                            lines.Add($"      • Groups: {string.Join(", ", action.AssignedGroups)}");
                    }

                    if (action.OnExecuteHooks.Count != 0)
                        lines.Add($"   ⮡ OnExecute Hooks: {FormatHookList(action.OnExecuteHooks)}");

                    lines.Add(""); // spacing
                }
            }

            var boxWidth = Math.Max(defaultWidth, lines.Max(line => StripAnsi(line).Length) + 4);
            var border = new string('─', boxWidth);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"╭{border}╮");
            foreach (var line in lines)
            {
                Console.WriteLine($"│ {PadAnsi(line, boxWidth - 2)} │");
            }

            Console.WriteLine($"╰{border}╯\n");
            Console.ResetColor();
        }

        return;

        void PrintHookGroup(string title, List<WorkflowHookDescriptor<TData>> hooks)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{title}:");
            Console.ResetColor();

            if (hooks.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("  ⮡ None");
                Console.ResetColor();
            }
            else
            {
                foreach (var hook in hooks)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"  ⮡ {hook.Hook.GetType().Name}");
                    Console.ResetColor();
                }
            }

            Console.WriteLine(); // spacing
        }

        string FormatHookList(IEnumerable<WorkflowHookDescriptor<TData>> hooks) =>
            $"[{string.Join(", ", hooks.Select(h => h.Hook.GetType().Name))}]";

        string StripAnsi(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, @"\x1B\[[0-9;]*[mGKF]", "");
        }

        string PadAnsi(string input, int totalWidth)
        {
            var plain = StripAnsi(input);
            var padRight = totalWidth - plain.Length;
            return input + new string(' ', Math.Max(0, padRight));
        }
    }
}