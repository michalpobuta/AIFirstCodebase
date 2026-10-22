using Shop.Features.Customers;
using Shop.Shared;

namespace Shop.Features.Orders;

public sealed record CreateOrder(CustomerId CustomerId, IReadOnlyList<CreateOrder.Line> Lines)
{
    public sealed record Line(string Sku, int Quantity, decimal UnitPrice);
}

public sealed class CreateOrderHandler(
    OrderStore orders, CustomerDirectory customers, DiscountPolicy discounts, TimeProvider clock)
{
    public Result<OrderId> Handle(CreateOrder command)
    {
        if (customers.Find(command.CustomerId) is null)
            return Error.Invalid($"Unknown customer {command.CustomerId}.");
        if (command.Lines.Count == 0)
            return Error.Invalid("An order needs at least one line.");
        if (command.Lines.Any(line => line.Quantity <= 0 || line.UnitPrice <= 0))
            return Error.Invalid("Quantity and unit price must be positive.");

        var lines = command.Lines.Select(line => new OrderLine(line.Sku, line.Quantity, line.UnitPrice)).ToList();
        var total = discounts.TotalFor(command.CustomerId, lines.Sum(line => line.Amount));
        var order = new Order(OrderId.New(), command.CustomerId, lines, total, clock.GetUtcNow());

        orders.Add(order);
        return order.Id;
    }
}

internal static class CreateOrderEndpoint
{
    public sealed record Request(string CustomerId, IReadOnlyList<CreateOrder.Line> Lines);

    public static void MapCreateOrder(this RouteGroupBuilder orders) =>
        orders.MapPost("/", (Request request, CreateOrderHandler handler) =>
            handler.Handle(new CreateOrder(new CustomerId(request.CustomerId), request.Lines))
                .ToHttpResult(id => Results.Created($"/orders/{id}", new { id })));
}
