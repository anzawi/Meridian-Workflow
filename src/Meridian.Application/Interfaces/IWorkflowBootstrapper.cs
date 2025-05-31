namespace Meridian.Application.Interfaces;

public interface IWorkflowBootstrapper
{
    void Register(IWorkflowDefinitionBuilder builder);
}