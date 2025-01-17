using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace BP.DynamoDbLib;

public static class DbSetExtensions
{
    public static IAsyncSearch<TEntity> ScanAsync<TEntity>(this DbSet<TEntity> set)
        where TEntity : IEntity, ISingleTable
        =>
            set.ScanAsync([
                new(TEntity.PrimaryKeyName, ScanOperator.BeginsWith, TEntity.Prefix)
            ]);
}