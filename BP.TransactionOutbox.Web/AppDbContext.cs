using System.Text.Json;
using Amazon.DynamoDBv2.DataModel;
using Amazon.SQS.Model;
using BP.DynamoDbLib;
using BP.TransactionOutboxAspire.Web.Messaging;
using BP.TransactionOutboxAspire.Web.Models;

namespace BP.TransactionOutboxAspire.Web;

public class AppDbContext(
    IDynamoDBContext dynamoDbContext,
    SqsDispatcher dispatcher,
    ILogger<AppDbContext> log) : DbContext(dynamoDbContext, log)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<TransactionOutbox> TransactionOutboxes => Set<TransactionOutbox>();

    public override async Task SaveAsync()
    {
        var hadTxOutbox = false;
        foreach (var entity in Entities<EntityBase>())
        {
            foreach (var integrationEvent in entity.IntegrationEvents())
            {
                hadTxOutbox = true;
                TransactionOutboxes.Add(TransactionOutbox.Create(integrationEvent));
            }
        }

        await base.SaveAsync();
        if (hadTxOutbox)
        {
            try
            {
                await dispatcher.Dispatch(new ProcessTxOutbox());
            }
            catch (Exception e)
            {
                log.LogError(e, "Failed to send message");
            }
        }
    }
}