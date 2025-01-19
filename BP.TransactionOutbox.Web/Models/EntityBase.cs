using System.Runtime.CompilerServices;
using BP.DynamoDbLib;

namespace BP.TransactionOutboxAspire.Web;

public abstract class EntityBase : IEntity
{
    private readonly IList<object> _integrationEvents = [];

    protected void AddIntegrationEvent(object integrationEvent) =>
        _integrationEvents.Add(integrationEvent);

    public IReadOnlyList<object> IntegrationEvents() =>
        _integrationEvents.AsReadOnly();

    private bool _isDirty;

    protected void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        _isDirty = true;
        field = value;
    }

    public bool IsDirty() => _isDirty;

    public void ClearDirty() => _isDirty = false;
}