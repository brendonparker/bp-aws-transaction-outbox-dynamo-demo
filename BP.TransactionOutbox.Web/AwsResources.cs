internal class AwsResources
{
    public AwsResourcesTable Table { get; set; } = new();
}

internal class AwsResourcesTable
{
    public string TableName { get; set; } = null!;
}