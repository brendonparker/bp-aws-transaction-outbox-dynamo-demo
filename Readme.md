# Transaction Outbox Demo

Yet another transaction outbox demo experiment.

This time using the following tech:
 - Aspire (for infrastructure management)
 - DynamoDB for persistence layer

## Callouts

1. I attempted a single-table design/approach. It is a very bad one. More thought should be put into the query patterns and how entities are partitioned.
2. The transaction outbox records are consumed/deleted, rather than marked as procesed. Ideally we keep those for auditability.
3. I took a stab at writing some convenience library around interactions with DynamoDB. I tried to give it an API feel similar to EntityFramework, with a similar unit of work pattern and use of DbSet (Repositories).
This is very minimal, the entities are responsible for their own dirty flag management.

## Further Questions/Explorations...

When you run this locally, it will create the required AWS resources. However, the compute is still running locally. It isn't clear to me how to "deploy" this in its entirety. For example, how to make the API run in lambda or ECS.