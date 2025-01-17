using Amazon.DynamoDBv2.DataModel;
using BP.DynamoDbLib;

namespace BP.TransactionOutboxAspire.Web.Models;

public class TransactionOutbox : IEntity, ISingleTable
{
    [DynamoDBHashKey] public required string PK { get; set; }
    [DynamoDBRangeKey] public required string SK { get; set; }
    public required string Message { get; set; }
    public bool IsDirty() => true;

    public void ClearDirty()
    {
    }

    public static string Prefix => "Outbox";
    public static string PrimaryKeyName => nameof(PK);
}