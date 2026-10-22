using Shop.Features.Customers;
using Shop.Features.Orders;
using Shop.Features.Warehouse;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton(TimeProvider.System)
    .AddCustomers()
    .AddOrders()
    .AddWarehouse();

var app = builder.Build();

app.MapOrders();
app.MapWarehouse();

app.Run();
