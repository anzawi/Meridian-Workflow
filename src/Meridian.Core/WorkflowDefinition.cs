namespace Meridian.Core;

using Enums;
using Exceptions;
using Extensions;
using Interfaces;

/// <summary>
/// Represents the definition of a workflow composed of states, hooks, and transitions.
/// </summary>
/// <typeparam name="TData">
/// The type of data that the workflow operates on.
/// This type must implement the <see cref="IWorkflowData"/> interface.
/// </typeparam>
public class WorkflowDefinition<TData> where TData : class, IWorkflowData
{
    /// <summary>
    /// Gets or sets the unique identifier for the workflow definition.
    /// </summary>
    /// <remarks>
    /// This property represents the unique ID of the workflow definition.
    /// It must be a non-empty string and is critical for distinguishing different workflow definitions.
    /// The value is also used for generating the 'Code' property in Pascal case format.
    /// </remarks>
    public string Id { get; set; }

    /// <summary>
    /// Gets the PascalCase representation of the workflow definition's identifier.
    /// Converts the <see cref="Id"/> property into a format where the first letter of each word is capitalized.
    /// </summary>
    /// <remarks>
    /// Utilizes the <c>ToPascalCase</c> extension method to transform the identifier string.
    /// </remarks>
    public string Code => this.Id.ToPascalCase();

    /// <summary>
    /// Gets or sets the list of states that define the workflow's lifecycle.
    /// </summary>
    /// <remarks>
    /// A workflow must have at least one state, with the initial state being defined
    /// as a start state and at least one state marked as completed. Each state represents
    /// a distinct stage in the workflow and may include configurations such as actions,
    /// entry hooks, exit hooks, and type definitions (e.g., Start, Completed, etc.).
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the workflow does not contain any states, has states with duplicate names,
    /// or lacks a completed state.
    /// </exception>
    public List<WorkflowState<TData>> States { get; set; } = [];

    /// <summary>
    /// A collection of hooks that are executed during the creation of a new workflow request.
    /// </summary>
    /// <remarks>
    /// The <c>OnCreateHooks</c> property manages a list of hooks that are triggered
    /// when a new workflow request is created. These hooks are typically used to
    /// perform operations such as initialization, validation, or other actions that
    /// should take place immediately after a workflow is initiated.
    /// Hooks in this collection are executed asynchronously to allow for extensibility
    /// and custom logic during the workflow creation process.
    /// </remarks>
    /// <typeparam name="TData">
    /// The type of data associated with the workflow, which must implement the <c>IWorkflowData</c> interface.
    /// </typeparam>
    public List<WorkflowHookDescriptor<TData>> OnCreateHooks { get; set; } = [];

    /// <summary>
    /// Represents a collection of hook descriptors that are triggered during transitions
    /// in the workflow lifecycle.
    /// </summary>
    /// <typeparam name="TData">The type of the workflow data associated with the workflow.</typeparam>
    public List<WorkflowHookDescriptor<TData>> OnTransitionHooks { get; set; } = [];

    /// <summary>
    /// Represents a property for storing the historical transition information
    /// during the creation of a workflow instance. This property holds a
    /// <see cref="WorkflowTransition"/> object that encapsulates details
    /// such as the action performed, the state transitioned to, and who
    /// executed the action.
    /// </summary>
    private WorkflowTransition? OnCreateHistory { get; set; } = null;

    /// <summary>
    /// Overrides the default "OnCreateHistory" transition with the provided transition.
    /// </summary>
    /// <param name="history">
    /// The <see cref="WorkflowTransition"/> object to be set as the "OnCreateHistory" transition.
    /// </param>
    /// <returns>
    /// The current instance of <see cref="WorkflowDefinition{TData}"/> for method chaining.
    /// </returns>
    public WorkflowDefinition<TData> OverrideOnCreateHistory(WorkflowTransition history)
    {
        this.OnCreateHistory = history;
        return this;
    }

    /// <summary>
    /// Represents a workflow definition for managing states, transitions, and hooks for a workflow process.
    /// </summary>
    /// <typeparam name="TData">The type of the workflow's associated data, which must implement <see cref="IWorkflowData"/>.</typeparam>
    public WorkflowDefinition(string id)
    {
        this.Id = id;
    }

    /// Adds a new state to the workflow definition.
    /// <param name="name">The name of the state to be added to the workflow definition.</param>
    /// <param name="config">An action to configure the state, allowing additional properties or hooks to be set.</param>
    /// <return>The updated WorkflowDefinition instance including the newly added state.</return>
    public WorkflowDefinition<TData> State(string name, Action<WorkflowState<TData>> config)
    {
        var state = new WorkflowState<TData> { Name = name };
        config(state);
        if (this.States.Count == 0)
        {
            state.IsStarted();
        }

        this.States.Add(state);
        return this;
    }

    /// <summary>
    /// Retrieves the workflow transition that represents the initial history entry
    /// when a new workflow instance is created. If the OnCreateHistory is not set,
    /// it generates a default transition with predefined values.
    /// </summary>
    /// <returns>
    /// A <see cref="WorkflowTransition"/> representing the initial history entry.
    /// If no custom OnCreateHistory is defined, a default transition is returned
    /// based on the first state of the workflow.
    /// </returns>
    public WorkflowTransition GetOnCreateHistory()
    {
        return this.OnCreateHistory ?? new WorkflowTransition
        {
            Action = "Submitted",
            PerformedBy = "Requester",
            ToState = this.States[0].Name,
        };
    }

    /// <summary>
    /// Validates the workflow definition by performing a series of checks to ensure its consistency and correctness.
    /// This includes validation of the workflow definition ID, states, actions, and transitions.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the workflow definition ID is null, empty, or whitespace,
    /// if there are no states defined, if invalid state types are present,
    /// or if transitions and next states are improperly configured.
    /// </exception>
    public void Validate()
    {
        this.ValidateDefinition();
        this.ValidateStates();
        this.ValidateActions();
        this.ValidateNextStates();
    }


    /// <summary>
    /// Validates the current workflow definition to ensure it meets the required conditions.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the workflow definition does not have a valid non-empty ID.
    /// Thrown when the workflow definition does not define at least one state.
    /// </exception>
    /// <remarks>
    /// This method checks the integrity of the workflow definition by ensuring that:
    /// 1. An ID is assigned, and it is not empty or whitespace.
    /// 2. At least one state is defined in the workflow.
    /// </remarks>
    private void ValidateDefinition()
    {
        if (string.IsNullOrWhiteSpace(this.Id))
            throw new WorkflowDefinitionException(this.Id, "Workflow definition must have a non-empty ID.");

        if (this.States.Count == 0)
            throw new WorkflowDefinitionException(this.Id, "Workflow must define at least one state.");
    }

    /// <summary>
    /// Validates the states defined in the workflow to ensure they conform to the required rules and constraints:
    /// 1. Ensures that state names are not empty or duplicate.
    /// 2. Verifies that the first state is of type 'Start'.
    /// 3. Confirms that the workflow contains at least one state with a type of 'Completed'.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when any of the following conditions are met:
    /// - A state name is empty.
    /// - A state name is duplicated within the workflow.
    /// - The first state is not of type 'Start'.
    /// - The workflow lacks at least one state of type 'Completed'.
    /// </exception>
    private void ValidateStates()
    {
        var stateNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var state in this.States)
        {
            if (string.IsNullOrWhiteSpace(state.Name))
                throw new WorkflowDefinitionException(this.Id, "has an empty name State!.");

            if (!stateNames.Add(state.Name))
                throw new WorkflowStateException(this.Id, state.Name, "Duplicate state name.");
        }

        if (this.States.First().Type != StateType.Start)
        {
            this.States.First().IsStarted();
        }

        if (this.States.All(s => s.Type != StateType.Completed))
        {
            throw new WorkflowDefinitionException(this.Id, "must have a completed state, use 'state.IsCompleted()' to set the state type to Completed.");
        }
    }

    /// <summary>
    /// Validates all actions within the workflow's states to ensure their configurations are correct.
    /// </summary>
    /// <remarks>
    /// This method performs several checks on actions defined within each state of the workflow:
    /// - Ensures that every action has a non-empty name.
    /// - Ensures that action names are unique within each state.
    /// - Ensures that auto-actions have a defined condition.
    /// - Ensures that non-auto actions do not have conditions.
    /// - Ensures that each action specifies a next state.
    /// If any of these conditions are not met, an <see cref="InvalidOperationException"/> is thrown.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if any of the following validation rules are violated:
    /// - An action within a state has an empty name.
    /// - Duplicate action names exist within a single state.
    /// - An auto-action is missing a condition.
    /// - A non-auto action has a defined condition.
    /// - An action does not define a next state.
    /// </exception>
    private void ValidateActions()
    {
        foreach (var state in this.States)
        {
            var actionNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var action in state.Actions)
            {
                if (string.IsNullOrWhiteSpace(action.Name))
                    throw new WorkflowStateException(this.Id, state.Name,
                        "Has an empty name Action.");

                if (!actionNames.Add(action.Name))
                    throw new WorkflowStateException(this.Id, state.Name,
                        $"Has a duplicated action '{action.Name}'.");

                switch (action)
                {
                    case { IsAuto: true, Condition: null }:
                        throw new WorkflowActionException(this.Id, state.Name, action.Name,
                            "Is Auto-action and must have a condition.");
                    case { IsAuto: false, Condition: not null }:
                        throw new WorkflowActionException(this.Id, state.Name, action.Name,
                            "Has a condition but is not marked as Auto-Action.");
                }

                if (string.IsNullOrWhiteSpace(action.NextState))
                    throw new WorkflowActionException(this.Id, state.Name, action.Name,
                        "Must define a next state.");
            }
        }
    }

    /// <summary>
    /// Validates that all the actions defined within each state of the workflow
    /// reference valid next states. This ensures that every `NextState` value in
    /// the actions corresponds to an existing state in the workflow definition.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when an action in any state references a `NextState` that is not
    /// defined among the workflow's states.
    /// </exception>
    private void ValidateNextStates()
    {
        var stateNames = this.States.Select(s => s.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var state in this.States)
        {
            foreach (var action in state.Actions.Where(action => !stateNames.Contains(action.NextState)))
            {
                throw new WorkflowActionException(this.Id, state.Name, action.Name,
                    $"References undefined next state '{action.NextState}'.");
            }
        }
    }
}