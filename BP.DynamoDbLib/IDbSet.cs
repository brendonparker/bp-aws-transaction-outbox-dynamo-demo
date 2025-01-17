using Amazon.DynamoDBv2.DataModel;

namespace BP.DynamoDbLib;

public interface IDbSet
{
    ITransactWrite? CreateWrite();
}