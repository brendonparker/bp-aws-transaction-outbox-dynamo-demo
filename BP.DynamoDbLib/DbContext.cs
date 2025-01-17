using System.Collections.Concurrent;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Logging;

namespace BP.DynamoDbLib;

public abstract class DbContext(
    IDynamoDBContext dynamoDbContext,
    ILogger? log = null)
{
    private readonly ConcurrentDictionary<Type, IDbSet> _sets = [];

    protected DbSet<TEntity> Set<TEntity>() where TEntity : IEntity
    {
        var result = _sets.GetOrAdd(typeof(TEntity), _ => new DbSet<TEntity>(dynamoDbContext)) as DbSet<TEntity>;
        return result!;
    }

    public async Task SaveAsync()
    {
        var transactionParts = _sets.Values
            .Select(x => x.CreateWrite())
            .Where(x => x != null)
            .ToArray();

        if (transactionParts.Length == 0)
        {
            log?.LogWarning("No added or dirty entities, not saving.");
            return;
        }

        var write = dynamoDbContext.CreateMultiTableTransactWrite(transactionParts);
        await write.ExecuteAsync();
    }
}