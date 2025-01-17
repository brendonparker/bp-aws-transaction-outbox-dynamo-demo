using Amazon.DynamoDBv2.DataModel;

namespace BP.DynamoDbLib;

internal class AsyncSearchWrapper<T>(IAsyncSearch<T> search, Action<List<T>> onLoaded) : IAsyncSearch<T>
{
    public async Task<List<T>> GetNextSetAsync(CancellationToken cancellationToken = default)
    {
        var set = await search.GetNextSetAsync(cancellationToken);
        onLoaded(set);
        return set;
    }

    public async Task<List<T>> GetRemainingAsync(CancellationToken cancellationToken = default)
    {
        var remaining = await search.GetRemainingAsync(cancellationToken);
        onLoaded(remaining);
        return remaining;
    }

    public bool IsDone => search.IsDone;
    public string PaginationToken => search.PaginationToken;
}