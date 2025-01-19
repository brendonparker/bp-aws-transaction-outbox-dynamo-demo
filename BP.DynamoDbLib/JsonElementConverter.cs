using System.Text.Json;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace BP.DynamoDbLib;

public class JsonElementConverter : IPropertyConverter
{
    public DynamoDBEntry ToEntry(object value) =>
        JsonSerializer.Serialize(value);

    public object FromEntry(DynamoDBEntry entry) =>
        JsonSerializer.Deserialize<JsonElement>(entry.AsString());
}