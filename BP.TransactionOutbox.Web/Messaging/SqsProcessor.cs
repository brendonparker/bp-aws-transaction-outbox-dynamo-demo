using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;

namespace BP.TransactionOutboxAspire.Web.Messaging;

public class SqsProcessor(
    IAmazonSQS sqs,
    IOptions<AwsResources> opts,
    IServiceProvider serviceProvider,
    ILogger<SqsProcessor> log) : BackgroundService
{
    private string QueueUrl => opts.Value.QueueUrl;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var received = await sqs.ReceiveMessageAsync(new ReceiveMessageRequest(QueueUrl)
            {
                MaxNumberOfMessages = 1,
                MessageAttributeNames = ["All"],
                VisibilityTimeout = 60,
                WaitTimeSeconds = 10,

            }, stoppingToken);

            if (received.Messages == null)
            {
                await Task.Delay(2000, stoppingToken);
                continue;
            }

            foreach (var message in received.Messages)
            {
                using var scope = serviceProvider.CreateScope();
                var messageType =
                    message.MessageAttributes?.GetValueOrDefault(MessagingConstants.MessageType, null)?.StringValue;
                if (messageType == null)
                {
                    log.LogError("Missing message type");
                    await sqs.DeleteMessageAsync(QueueUrl, message.ReceiptHandle, stoppingToken);
                    continue;
                }

                var handlerInvoker = scope.ServiceProvider.GetRequiredKeyedService<SqsMessageHandler>(messageType);
                await handlerInvoker(message, stoppingToken);
                await sqs.DeleteMessageAsync(QueueUrl, message.ReceiptHandle, stoppingToken);
            }
        }
    }
}

internal delegate Task SqsMessageHandler(Message message, CancellationToken ct);

internal delegate Task TransactionOutboxMessageDispatcher(JsonElement json, CancellationToken ct);