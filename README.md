# Shuttle.Recall.Samples

Samples demonstrating the use of Shuttle.Recall Event Sourcing and Event Processing.

## Orders

A multi-project sample (`Orders/Orders.slnx`) showing a realistic, message-driven event-sourced service backed by SQL Server:

- **`Orders`** — the `Order` domain/aggregate (`Register`, `AddItem`, raising `Registered`/`ItemAdded` events).
- **`Orders.Server`** — a host that registers `Shuttle.Recall` with `Shuttle.Recall.SqlServer.Storage` and `Shuttle.Recall.SqlServer.EventProcessing`, and an `OrderHandler` projection that builds an EF Core read model (`Orders.Data`). It also wires up `Shuttle.Hopper` (using Azure Storage Queues / Azurite) to handle `CreateOrder` commands, demonstrating both eventual and immediate-consistency (`WithImmediateConsistency()`) saves, as well as an outbox-style failure/retry path.
- **`Orders.Client`** — a Terminal.Gui console client that sends `CreateOrder` commands over the bus.
- **`Orders.Data`** — the EF Core read-model project; see its own `README.md` for migration commands.

Bring up SQL Server and Azurite (see the sample's own configuration) and run `Orders.Server` followed by `Orders.Client`.
