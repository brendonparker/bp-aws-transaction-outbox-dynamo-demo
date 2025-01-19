namespace BP.TransactionOutboxAspire.Web.Messaging;

public interface IHandler<in TMessage> where TMessage : class
{
    Task Handle(TMessage message, CancellationToken ct);
}

public class ProcessTxOutbox;