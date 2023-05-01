namespace TelegramBot.Services.Implementations.HttpClients
{
    public class CacheSignal<T>
    {
        private readonly SemaphoreSlim _signal = new(1, 1);

        public Task WaitAsync() => _signal.WaitAsync();

        public void Release() => _signal.Release();
    }
}
