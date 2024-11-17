using Microsoft.Extensions.Hosting;

public class QueuedHostedService : BackgroundService
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly IServiceProvider _serviceProvider;

    public QueuedHostedService(IBackgroundTaskQueue taskQueue, IServiceProvider serviceProvider)
    {
        _taskQueue = taskQueue;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem = await _taskQueue.DequeueAsync(stoppingToken);

            try
            {
                await workItem(stoppingToken, _serviceProvider);
            }
            catch (Exception ex)
            {
                // Log lỗi
            }
        }
    }
}
