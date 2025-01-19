using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;

namespace BP.TransactionOutboxAspire.Web.Messaging;

public static class MessagingConstants
{
    public const string MessageType = "BP.MessageType";
}

public class SqsDispatcher(IAmazonSQS sqs, IOptions<AwsResources> opts)
{
    public async Task Dispatch<T>(
        T message,
        CancellationToken ct = default,
        string messageGroupId = "TheOne")
        where T : class
    {
        await sqs.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl = opts.Value.QueueUrl,
            MessageBody = JsonSerializer.Serialize(message),
            MessageGroupId = messageGroupId,
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                [MessagingConstants.MessageType] = new()
                {
                    DataType = "String",
                    StringValue = message.GetType().Name
                }
            }
        }, ct);
    }
}