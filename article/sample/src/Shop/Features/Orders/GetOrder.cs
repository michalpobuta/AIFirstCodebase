using Shop.Shared;

namespace Shop.Features.Orders;

public sealed record GetOrder(OrderId OrderId);

public sealed record OrderDetails(
    Guid Id, string CustomerId, OrderStatus Status, decimal Total, IReadOnlyList<OrderLine> Lines,
    DateTimeOffset CreatedAt, DateTimeOffset? PaidAt, DateTimeOffset? ShippedAt)
{
    public static OrderDetails From(Order order) => new(
        order.Id.Value, order.CustomerId.Value, order.Status, order.Total, order.Lines,
        order.CreatedAt, order.PaidAt, order.ShippedAt);
}

public sealed class GetOrderHandler(OrderStore orders)
{
    public Result<OrderDetails> Handle(GetOrder query) =>
        orders.Find(query.OrderId) is { } order
            ? OrderDetails.From(order)
            : Error.NotFound($"Order {query.OrderId} not found.");
}

internal static class GetOrderEndpoint
{
    public static void MapGetOrder(this RouteGroupBuilder orders) =>
        orders.MapGet("/{id:guid}", (Guid id, GetOrderHandler handler) =>
            handler.Handle(new GetOrder(new OrderId(id))).ToHttpResult(details => Results.Ok(details)));
}
