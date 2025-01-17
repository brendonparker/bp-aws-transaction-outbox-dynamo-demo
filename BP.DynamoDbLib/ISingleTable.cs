namespace BP.DynamoDbLib;

public interface ISingleTable
{
    public static abstract string Prefix { get; }
    public static abstract string PrimaryKeyName { get; }
}