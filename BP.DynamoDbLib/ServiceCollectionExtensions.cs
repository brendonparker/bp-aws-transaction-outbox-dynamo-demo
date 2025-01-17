using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.DependencyInjection;

namespace BP.DynamoDbLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDynamoDbLib(this IServiceCollection services) =>
        services
            .AddAWSService<IAmazonDynamoDB>()
            .AddSingleton(new DynamoDBContextConfig { DisableFetchingTableMetadata = true, })
            .AddSingleton<IDynamoDBContext, DynamoDBContext>();
}