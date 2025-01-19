using Amazon.DynamoDBv2.DataModel;

namespace BP.DynamoDbLib;

public interface IDbSet
{
    IEnumerable<IEntity> TrackedEntities();
    ITransactWrite? CreateWrite();
}