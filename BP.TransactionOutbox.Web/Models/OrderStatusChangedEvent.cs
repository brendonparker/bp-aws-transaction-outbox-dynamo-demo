namespace BP.TransactionOutboxAspire.Web.Models;

public class OrderStatusChangedEvent
{
    public required long Id { get; init; }
    public required OrderStatus Status { get; init; }
}