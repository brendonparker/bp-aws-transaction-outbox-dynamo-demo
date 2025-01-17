using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace BP.DynamoDbLib;

public class DbSet<TEntity>(IDynamoDBContext dbContext) : IDbSet where TEntity : IEntity
{
    private readonly HashSet<TEntity> _trackedEntities = [];
    private readonly HashSet<TEntity> _addedEntities = [];

    public void Add(TEntity entity) =>
        _addedEntities.Add(entity);

    public async Task<TEntity?> LoadAsync(TEntity key)
    {
        var entity = await dbContext.LoadAsync(key);
        if (entity is null) return entity;
        _trackedEntities.Add(entity);
        return entity;
    }

    public IAsyncSearch<TEntity> FromQueryAsync(QueryOperationConfig queryOperationConfig) =>
        WrappedAsyncSearch(dbContext.FromQueryAsync<TEntity>(queryOperationConfig));

    public IAsyncSearch<TEntity> ScanAsync(IEnumerable<ScanCondition> scanConditions) =>
        WrappedAsyncSearch(dbContext.ScanAsync<TEntity>(scanConditions));

    private IAsyncSearch<TEntity> WrappedAsyncSearch(IAsyncSearch<TEntity> query) =>
        new AsyncSearchWrapper<TEntity>(query, entities =>
        {
            foreach (var entity in entities)
            {
                _trackedEntities.Add(entity);
            }
        });

    public ITransactWrite? CreateWrite()
    {
        var entityCount = 0;
        entityCount += _addedEntities.Count;
        entityCount += _trackedEntities.Count(x => x.IsDirty());
        if (entityCount == 0) return null;
        
        var transactWrite = dbContext.CreateTransactWrite<TEntity>();
        transactWrite.AddSaveItems(_addedEntities);

        foreach (var entity in _trackedEntities)
        {
            if (entity.IsDirty())
            {
                transactWrite.AddSaveItem(entity);
            }
        }

        return transactWrite;
    }
}