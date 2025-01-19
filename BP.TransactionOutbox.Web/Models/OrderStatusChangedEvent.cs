namespace BP.TransactionOutboxAspire.Web.Models;

public class OrderStatusChangedEvent
{
    public required long Id { get; init; }
    public required string Status { get; init; }
}