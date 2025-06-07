namespace Meridian.Infrastructure.Services;

using Application.Interfaces;
using Core;
using Core.Builders;
using Core.Interfaces;
using Core.Interfaces.DslBuilder;

/// <inheritdoc />
public class WorkflowDefinitionBuilder : IWorkflowDefinitionBuilder
{
    private readonly List<(string DefinitionId, object Engine, Type DataType)> _engines = [];
    /// <inheritdoc />
    public void Define<TData>(string definitionId, Action<IWorkflowDefinitionBuilder<TData>> configure)
        where TData : class, IWorkflowData
    {
        var def =  WorkflowDefinitionBuilder<TData>.Create(definitionId, configure);
        var engine = new WorkflowEngine<TData>(def);
        this._engines.Add((definitionId, engine, typeof(TData)));
    }

    /// <summary>
    /// Retrieves the collection of workflow engines that have been defined.
    /// </summary>
    /// <returns>A list of tuples containing the workflow definition ID, the workflow engine instance, and the data type associated with the workflow engine.</returns>
    public List<(string DefinitionId, object Engine, Type DataType)> BuildEngines() => this._engines;
}