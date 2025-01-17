using Amazon.DynamoDBv2.DataModel;
using BP.DynamoDbLib;
using BP.TransactionOutboxAspire.Web.Models;

namespace BP.TransactionOutboxAspire.Web;

public class AppDbContext(
    IDynamoDBContext dynamoDbContext,
    ILogger<AppDbContext> log) : DbContext(dynamoDbContext, log)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<TransactionOutbox> TransactionOutboxes => Set<TransactionOutbox>();
}