using Shop.Shared;

namespace Shop.Features.Orders;

public sealed record PayOrder(OrderId OrderId);

public sealed class PayOrderHandler(OrderStore orders, TimeProvider clock)
{
    public Result Handle(PayOrder command) =>
        orders.Find(command.OrderId) is { } order
            ? order.Pay(clock.GetUtcNow())
            : Error.NotFound($"Order {command.OrderId} not found.");
}

internal static class PayOrderEndpoint
{
    public static void MapPayOrder(this RouteGroupBuilder orders) =>
        orders.MapPost("/{id:guid}/pay", (Guid id, PayOrderHandler handler) =>
            handler.Handle(new PayOrder(new OrderId(id))).ToHttpResult());
}
