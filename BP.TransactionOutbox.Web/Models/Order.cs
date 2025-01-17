using Amazon.DynamoDBv2.DataModel;
using BP.DynamoDbLib;

namespace BP.TransactionOutboxAspire.Web.Models;

public class Order : EntityBase, ISingleTable
{
    [DynamoDBHashKey] public required string PK { get; set; }
    [DynamoDBRangeKey] public required string SK { get; set; }

    public static Order FromKey(long id) =>
        new()
        {
            PK = $"{Prefix}|{id}",
            SK = "1"
        };

    private string _status = null!;

    public string Status
    {
        get => _status;
        set => SetField(ref _status, value);
    }

    public void Submit()
    {
        if (Status != "Submitted")
        {
            Status = "Submitted";
        }
    }

    public static string Prefix => "Order|";
    public static string PrimaryKeyName => nameof(PK);
}