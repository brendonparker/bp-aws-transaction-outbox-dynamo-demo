using Amazon;
using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.SQS;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;

var builder = DistributedApplication.CreateBuilder(args);

var awsConfig = builder.AddAWSSDKConfig()
    .WithProfile("bp_dev_2025")
    .WithRegion(RegionEndpoint.USEast2);


var stack = builder.AddAWSCDKStack("bp-txoutbox-dynamodb")
    .WithReference(awsConfig);

var table = stack
    .AddDynamoDBTable("Table", new TableProps
    {
        TableName = "bp-txoutbox-table",
        PartitionKey = new Attribute
        {
            Name = "PK",
            Type = AttributeType.STRING
        },
        SortKey = new Attribute
        {
            Name = "SK",
            Type = AttributeType.STRING
        },
        RemovalPolicy = RemovalPolicy.DESTROY,
        BillingMode = BillingMode.PAY_PER_REQUEST
    });

var queue =
    stack.AddSQSQueue("Queue", new QueueProps
    {
        QueueName = "bp-txoutbox-queue.fifo",
        ContentBasedDeduplication = true,
    });

builder
    .AddProject<Projects.BP_TransactionOutbox_Web>("API")
    .WithReference(table, t => t.TableName, "TableName", "AWS:Resources")
    .WithReference(queue, s => s.QueueUrl, "QueueUrl", "AWS:Resources");

builder.Build().Run();