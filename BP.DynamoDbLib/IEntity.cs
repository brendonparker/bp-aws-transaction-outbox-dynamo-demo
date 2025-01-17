namespace BP.DynamoDbLib;

public interface IEntity
{
    public bool IsDirty();
    public void ClearDirty();
}