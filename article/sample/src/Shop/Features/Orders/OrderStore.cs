using System.Collections.Concurrent;
using Shop.Shared;

namespace Shop.Features.Orders;

/// <summary>In-memory order storage. Thread-safe, lost on restart — this is a demo.</summary>
public sealed class OrderStore
{
    private readonly ConcurrentDictionary<OrderId, Order> _orders = new();

    public void Add(Order order) => _orders[order.Id] = order;

    public Order? Find(OrderId id) => _orders.GetValueOrDefault(id);

    public IReadOnlyList<Order> All() => [.. _orders.Values.OrderBy(order => order.CreatedAt)];
}
