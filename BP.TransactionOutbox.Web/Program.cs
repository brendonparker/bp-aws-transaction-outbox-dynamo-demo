using Amazon;
using Amazon.Util;
using BP.DynamoDbLib;
using BP.TransactionOutboxAspire.Web;
using BP.TransactionOutboxAspire.Web.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddDynamoDbLib();
builder.Services.AddTransient<AppDbContext>();

AwsResources awsResources = new();
builder.Configuration.Bind("AWS:Resources", awsResources);
AWSConfigsDynamoDB.Context.AddMapping(new TypeMapping(typeof(TransactionOutbox), awsResources.Table.TableName));
AWSConfigsDynamoDB.Context.AddMapping(new TypeMapping(typeof(Order), awsResources.Table.TableName));

var app = builder.Build();

app.MapGet("/", () => awsResources);
app.MapGet("/orders", async (AppDbContext dbContext) =>
{
    var search = dbContext.Orders.ScanAsync();
    return await search.GetNextSetAsync();
});
app.MapGet("/outbox", async (AppDbContext dbContext) =>
{
    var search = dbContext.TransactionOutboxes.ScanAsync();
    return await search.GetNextSetAsync();
});
app.MapPost("/order/{id}/submit",
    async Task<Results<Ok<Order>, NotFound>> ([FromRoute] long id, AppDbContext dbContext) =>
    {
        var order = await dbContext.Orders.LoadAsync(Order.FromKey(id));
        if (order == null)
        {
            return TypedResults.NotFound();
        }

        order.Submit();
        await dbContext.SaveAsync();
        return TypedResults.Ok(order);
    });
app.MapPost("/order", async (AppDbContext dbContext) =>
{
    var id = DateTime.UtcNow.Ticks;
    var order = Order.FromKey(id);
    var tx = new TransactionOutbox
    {
        PK = $"Outbox|{id}",
        SK = "1",
        Message = "Test"
    };

    dbContext.Orders.Add(order);
    dbContext.TransactionOutboxes.Add(tx);
    await dbContext.SaveAsync();

    return order;
});


app.Run();