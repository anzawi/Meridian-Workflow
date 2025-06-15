using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Meridian.Infrastructure.DatabaseContext;

internal class WorkflowMigrationService(IServiceProvider sp) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        await db.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
