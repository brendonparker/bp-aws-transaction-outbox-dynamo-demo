using BP.DynamoDbLib;

namespace BP.TransactionOutboxAspire.Web.Messaging.Handlers;

public class TxOutboxHandler(
    ILogger<TxOutboxHandler> log,
    IServiceProvider serviceProvider,
    AppDbContext dbContext) : IHandler<ProcessTxOutbox>
{
    public async Task Handle(ProcessTxOutbox message, CancellationToken ct)
    {
        var search = dbContext.TransactionOutboxes.ScanAsync();
        var outboxRecords = await search.GetNextSetAsync(ct);

        if (!outboxRecords.Any())
        {
            log.LogWarning("No outbox records found");
            return;
        }

        await using var scope = serviceProvider.CreateAsyncScope();

        foreach (var record in outboxRecords)
        {
            var dispatcher =
                scope.ServiceProvider.GetRequiredKeyedService<TransactionOutboxMessageDispatcher>(record.Type);
            await dispatcher(record.Payload, ct);

            dbContext.TransactionOutboxes.Remove(record);
        }

        await dbContext.SaveAsync();
    }
}