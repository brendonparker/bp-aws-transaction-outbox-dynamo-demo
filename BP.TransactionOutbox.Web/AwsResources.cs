namespace BP.TransactionOutboxAspire.Web;

public class AwsResources
{
    public string TableName { get; set; } = null!;
    public string QueueUrl { get; set; } = null!;
}