# Blazor experiments

## Setting up the project

Check out the project Blazor Server-side. Steps to configure the web application:
1. Provision an Azure SignalR Service - switch it to "classic" mode (in order to support a connected server for BlazorHub as well as serverless client connections for the connected instances ComponentHub)
2. Copy appsettings.local.example.json to appsettings.local.json
3. Configure the connection string for the Blazor hub as well as the connected instances "hub". May be the same.


## Multi-tab multi-client multi server-node distribution of events between Blazor circuits 

Experienting with a multi-tab multi-client multi server-node distribution of events between Blazor circuit.

## Learnings

Although the Azure SingleR Service speaks about connected clients and a server, the server is really just a client that is capable of sending messages to connected clients. The SDK does this by calling the Azure SignalR Service through its REST API. "serverless" server!

## Singletons for cross-circuit state

The fundamental challenge for server-side Blazor apps is its server-side memory footprint. Per circuit (connected client) it keeps state for all Blazor components and all root-scoped services. With preview 9 it has become possible to scope services only for the lifetime of a Blazor component (OwningComponentBase, see https://devblogs.microsoft.com/aspnet/asp-net-core-and-blazor-updates-in-net-core-3-0-preview-9/).

If we would like to communicate across circuits, we need to have some shared subscription. Somehow we must manage this memory footprint cautiously.

# Credits

Credits go to:
* Vladimir Samoilenko for his Material Design Blazor components library. See https://github.com/SamProf/MatBlazor
* Steve Sanderson for his Mission Control application. See https://github.com/SteveSandersonMS/presentation-2019-06-NDCOslo.
We have borrowed elements for the authentication and JavaScript interop scenarios.

