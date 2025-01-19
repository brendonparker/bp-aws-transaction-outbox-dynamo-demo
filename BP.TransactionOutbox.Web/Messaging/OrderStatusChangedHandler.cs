using BP.TransactionOutboxAspire.Web.Models;

namespace BP.TransactionOutboxAspire.Web.Messaging;

public class OrderStatusChangedHandler(
    ILogger<OrderStatusChangedHandler> log,
    AppDbContext dbContext) : IHandler<OrderStatusChangedEvent>
{
    public async Task Handle(OrderStatusChangedEvent message, CancellationToken ct)
    {
        if (message.Status != "Submitted")
        {
            log.LogInformation("Only interested in submitted events.");
            return;
        }

        var order = await dbContext.Orders.LoadAsync(Order.FromKey(message.Id), ct);
        if (order == null)
        {
            log.LogError("Could not find order");
            return;
        }

        order.ProcessedAt = DateTime.UtcNow;
        await dbContext.SaveAsync();
    }
}