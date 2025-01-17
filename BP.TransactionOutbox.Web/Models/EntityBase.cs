using System.Runtime.CompilerServices;
using BP.DynamoDbLib;

namespace BP.TransactionOutboxAspire.Web.Models;

public abstract class EntityBase : IEntity
{
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