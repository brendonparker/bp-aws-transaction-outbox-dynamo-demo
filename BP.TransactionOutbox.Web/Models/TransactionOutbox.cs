using System.Text.Json;
using Amazon.DynamoDBv2.DataModel;
using BP.DynamoDbLib;

namespace BP.TransactionOutboxAspire.Web.Models;

public class TransactionOutbox : IEntity, ISingleTable
{
    public static TransactionOutbox Create(object @event)
    {
        var id = DateTime.UtcNow.Ticks;
        return new TransactionOutbox
        {
            Id = id,
            PK = $"{Prefix}{id}",
            SK = "1",
            Type = @event.GetType().Name,
            Payload = JsonSerializer.SerializeToElement(@event)
        };
    }

    public required long Id { get; init; }
    public required string Type { get; init; }

    [DynamoDBProperty(typeof(JsonElementConverter))]
    public required JsonElement Payload { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    [DynamoDBHashKey] public required string PK { get; set; }
    [DynamoDBRangeKey] public required string SK { get; set; }

    public bool IsDirty() => true;

    public void ClearDirty()
    {
    }

    public static string Prefix => "Outbox|";
    public static string PrimaryKeyName => nameof(PK);
}