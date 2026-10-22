namespace Shop.Features.Orders;

/// <summary>Everything the Orders feature registers and exposes — listed, not scanned.</summary>
public static class OrdersModule
{
    public static IServiceCollection AddOrders(this IServiceCollection services) => services
        .AddSingleton<OrderStore>()
        .AddSingleton<DiscountPolicy>()
        .AddTransient<CreateOrderHandler>()
        .AddTransient<GetOrderHandler>()
        .AddTransient<PayOrderHandler>()
        .AddTransient<ShipOrderHandler>();

    public static IEndpointRouteBuilder MapOrders(this IEndpointRouteBuilder app)
    {
        var orders = app.MapGroup("/orders");
        orders.MapCreateOrder();
        orders.MapGetOrder();
        orders.MapPayOrder();
        orders.MapShipOrder();
        return app;
    }
}
