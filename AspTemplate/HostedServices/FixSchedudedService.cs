public class FixSchedudedService : BackgroundService
{
    private readonly ILogger<FixSchedudedService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public FixSchedudedService(ILogger<FixSchedudedService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            DateTime now = DateTime.Now;
            DateTime nextRunTime = new DateTime(now.Year, now.Month, now.Day, 11, 0, 0); // Next occurrence at 11:00
            
            if (now > nextRunTime)
            {
                nextRunTime = nextRunTime.AddDays(1); // If it's past 11:00, set it to the next day
            }

            TimeSpan delay = nextRunTime - now;

            _logger.LogInformation("Next scheduled task will run at: {time}", nextRunTime);

            await Task.Delay(delay, stoppingToken); // Delay until next occurrence

            _logger.LogInformation("Scheduled task is running at: {time}", DateTimeOffset.Now);

            // Perform your scheduled task logic here
        }
    }
}
