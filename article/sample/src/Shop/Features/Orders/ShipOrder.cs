using Shop.Shared;

namespace Shop.Features.Orders;

public sealed record ShipOrder(OrderId OrderId);

public sealed class ShipOrderHandler(OrderStore orders, TimeProvider clock)
{
    public Result Handle(ShipOrder command) =>
        orders.Find(command.OrderId) is { } order
            ? order.Ship(clock.GetUtcNow())
            : Error.NotFound($"Order {command.OrderId} not found.");
}

internal static class ShipOrderEndpoint
{
    public static void MapShipOrder(this RouteGroupBuilder orders) =>
        orders.MapPost("/{id:guid}/ship", (Guid id, ShipOrderHandler handler) =>
            handler.Handle(new ShipOrder(new OrderId(id))).ToHttpResult());
}
