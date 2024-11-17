using System.Threading.Channels;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, IServiceProvider, Task>> _queue;

    public BackgroundTaskQueue()
    {
        var options = new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<Func<CancellationToken, IServiceProvider, Task>>(options);
    }

    public void QueueBackgroundWorkItem(Func<CancellationToken, IServiceProvider, Task> workItem)
    {
        if (workItem == null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        _queue.Writer.TryWrite(workItem);
    }

    public async Task<Func<CancellationToken, IServiceProvider, Task>> DequeueAsync(CancellationToken cancellationToken)
    {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);
        return workItem;
    }
}