using Shop.Shared;

namespace Shop.Features.Orders;

public enum OrderStatus
{
    Pending,
    Paid,
    Shipped,
}

public sealed record OrderLine(string Sku, int Quantity, decimal UnitPrice)
{
    public decimal Amount => Quantity * UnitPrice;
}

/// <summary>An order and the transitions it allows: Pending → Paid → Shipped.</summary>
public sealed class Order(OrderId id, CustomerId customerId, IReadOnlyList<OrderLine> lines, decimal total, DateTimeOffset createdAt)
{
    public OrderId Id { get; } = id;
    public CustomerId CustomerId { get; } = customerId;
    public IReadOnlyList<OrderLine> Lines { get; } = lines;
    public decimal Total { get; } = total;
    public DateTimeOffset CreatedAt { get; } = createdAt;

    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public DateTimeOffset? PaidAt { get; private set; }
    public DateTimeOffset? ShippedAt { get; private set; }

    public Result Pay(DateTimeOffset now)
    {
        if (Status != OrderStatus.Pending)
            return Error.Conflict($"Only pending orders can be paid; order {Id} is {Status}.");

        Status = OrderStatus.Paid;
        PaidAt = now;
        return Result.Success;
    }

    public Result Ship(DateTimeOffset now)
    {
        if (Status != OrderStatus.Paid)
            return Error.Conflict($"Only paid orders can be shipped; order {Id} is {Status}.");

        Status = OrderStatus.Shipped;
        ShippedAt = now;
        return Result.Success;
    }
}
