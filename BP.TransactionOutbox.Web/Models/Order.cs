using Amazon.DynamoDBv2.DataModel;
using BP.DynamoDbLib;

namespace BP.TransactionOutboxAspire.Web.Models;

public class Order : EntityBase, ISingleTable
{
    [DynamoDBHashKey] public required string PK { get; set; }
    [DynamoDBRangeKey] public required string SK { get; set; }

    public required long Id { get; set; }

    public static Order FromKey(long id) =>
        new()
        {
            Id = id,
            PK = $"{Prefix}{id}",
            SK = "1"
        };

    public static Order Create(string customerName)
    {
        var id = DateTime.UtcNow.Ticks;
        var order = new Order
        {
            Id = id,
            Status = "Created",
            CustomerName = customerName,
            PK = $"{Prefix}{id}",
            SK = "1"
        };
        order.AddIntegrationEvent(new OrderStatusChangedEvent
        {
            Id = order.Id,
            Status = order.Status
        });
        return order;
    }

    private string _status = null!;

    public string Status
    {
        get => _status;
        set => SetField(ref _status, value);
    }

    private string _customerName = null!;

    public string CustomerName
    {
        get => _customerName;
        set => SetField(ref _customerName, value);
    }

    public void Submit()
    {
        if (Status != "Submitted")
        {
            Status = "Submitted";
            AddIntegrationEvent(new OrderStatusChangedEvent
            {
                Id = Id,
                Status = Status
            });
        }
    }

    public static string Prefix => "Order|";
    public static string PrimaryKeyName => nameof(PK);
}