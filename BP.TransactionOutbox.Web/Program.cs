using Amazon;
using Amazon.Util;
using BP.DynamoDbLib;
using BP.TransactionOutboxAspire.Web;
using BP.TransactionOutboxAspire.Web.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddDynamoDbContext<AppDbContext>();

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
    var order = Order.Create("Acme, Inc");
    dbContext.Orders.Add(order);
    await dbContext.SaveAsync();
    return order;
});


app.Run();