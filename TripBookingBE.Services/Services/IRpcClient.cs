public interface IRpcClient
{
    Task<string> CallAsync(string message, CancellationToken cancellationToken = default);
    ValueTask DisposeAsync();
    Task StartAsync();
}