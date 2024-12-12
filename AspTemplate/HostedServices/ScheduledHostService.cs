// public class ScheduledHostService : BackgroundService
// {
//     private readonly ILogger<ScheduledHostService> _logger;

//     public ScheduledHostService(ILogger<ScheduledHostService> logger, IServiceScopeFactory scopeFactory)
//     {
//         _logger = logger;
//     }

//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         while (!stoppingToken.IsCancellationRequested)
//         {
//             _logger.LogInformation("Scheduled task is running at: {time}", DateTimeOffset.Now);

//             // Perform your scheduled task logic here

//             await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken); // Delay for 15 minutes
//         }
//     }
// }
