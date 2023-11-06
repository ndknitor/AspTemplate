
using Microsoft.EntityFrameworkCore;
using NewTemplate.Context;

public class PreloadHostedService : IHostedService
{
    private readonly ILogger<PreloadHostedService> logger;
    private readonly IServiceScope serviceScope;
    public PreloadHostedService(ILogger<PreloadHostedService> logger, IServiceScopeFactory scopeFactory)
    {
        this.logger = logger;
        serviceScope = scopeFactory.CreateScope();
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var context = serviceScope.ServiceProvider.GetRequiredService<EtdbContext>();
        await context.Seat.OrderBy(s => s.SeatId).FirstOrDefaultAsync();
        logger.LogInformation("Preload data complete");
        context.Dispose();
        serviceScope.Dispose();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation($"Preload data service disposed at: {DateTime.Now}");
        return Task.CompletedTask;
    }
}