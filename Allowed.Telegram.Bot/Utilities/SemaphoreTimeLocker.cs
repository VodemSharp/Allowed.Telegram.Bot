namespace Allowed.Telegram.Bot.Utilities;

public class SemaphoreTimeLocker(int number, TimeSpan delay)
{
    private readonly SemaphoreSlim _semaphore = new(number, number);

    public async Task LockAsync(Func<Task> taskFactory)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        var task = taskFactory();
        _ = task.ContinueWith(async _ =>
        {
            await Task.Delay(delay);
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
            await Task.Delay(delay);
            _semaphore.Release(1);
        });
        return await task;
    }
}