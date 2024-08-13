namespace Allowed.Telegram.Bot.Helpers;

public class SemaphoreTimeLocker
{
    private readonly TimeSpan _delay;
    private readonly SemaphoreSlim _semaphore;

    public SemaphoreTimeLocker(int number, TimeSpan delay)
    {
        _semaphore = new SemaphoreSlim(number, number);
        _delay = delay;
    }

    public async Task LockAsync(Func<Task> taskFactory)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        var task = taskFactory();
        _ = task.ContinueWith(async _ =>
        {
            await Task.Delay(_delay);
            _semaphore.Release(1);
        });
        await task;
    }

    public async Task<T> LockAsync<T>(Func<Task<T>> taskFactory)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        var task = taskFactory();
        _ = task.ContinueWith(async _ =>
        {
            await Task.Delay(_delay);
            _semaphore.Release(1);
        });
        return await task;
    }
}