using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BP.DynamoDbLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDynamoDbContext<TContext>(this IServiceCollection services)
        where TContext : DbContext

    {
        services.TryAddAWSService<IAmazonDynamoDB>();
        services.TryAddSingleton(new DynamoDBContextConfig
        {
            DisableFetchingTableMetadata = true
        });
        services.TryAddSingleton<IDynamoDBContext, DynamoDBContext>();
        services.TryAddTransient<TContext>();
        return services;
    }
}