using BP.DynamoDbLib;

namespace BP.TransactionOutboxAspire.Web.Messaging;

public class TxOutboxHandler(IServiceProvider serviceProvider, AppDbContext dbContext) : IHandler<ProcessTxOutbox>
{
    public async Task Handle(ProcessTxOutbox message, CancellationToken ct)
    {
        var search = dbContext.TransactionOutboxes.ScanAsync();
        var outboxRecords = await search.GetNextSetAsync(ct);
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