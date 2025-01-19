using System.Text.Json;

namespace BP.TransactionOutboxAspire.Web.Messaging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandler<THandler, TMessage>(this IServiceCollection services)
        where THandler : class, IHandler<TMessage>
        where TMessage : class
    {
        services.AddTransient<THandler>();
        services.AddKeyedSingleton<SqsMessageHandler>(typeof(TMessage).Name,
            (sp, _) => async (ms, ct) =>
            {
                var handler = sp.GetRequiredService<THandler>();
                var message = JsonSerializer.Deserialize<TMessage>(ms.Body);
                await handler.Handle(message!, ct);
            });

        services.AddKeyedSingleton<TransactionOutboxMessageDispatcher>(typeof(TMessage).Name,
            (sp, _) => async (json, ct) =>
            {
                var handler = sp.GetRequiredService<SqsDispatcher>();
                var message = json.Deserialize<TMessage>();
                await handler.Dispatch(message!, ct);
            });

        return services;
    }
}