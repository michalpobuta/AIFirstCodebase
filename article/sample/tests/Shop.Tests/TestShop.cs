using Microsoft.Extensions.Time.Testing;
using Shop.Features.Customers;
using Shop.Features.Orders;
using Shop.Features.Warehouse;

namespace Shop.Tests;

/// <summary>The shop wired up the way <c>Program.cs</c> wires it, with a clock the test controls.</summary>
internal sealed class TestShop
{
    public static readonly CustomerId Vip = new("C-001");
    public static readonly CustomerId Regular = new("C-002");

    private readonly CustomerDirectory _customers = new();

    public FakeTimeProvider Clock { get; } = new(new DateTimeOffset(2026, 11, 20, 9, 0, 0, TimeSpan.Zero));
    public OrderStore Orders { get; } = new();

    public CreateOrderHandler CreateOrder => new(Orders, _customers, new DiscountPolicy(_customers), Clock);
    public GetOrderHandler GetOrder => new(Orders);
    public PayOrderHandler PayOrder => new(Orders, Clock);
    public ShipOrderHandler ShipOrder => new(Orders, Clock);
    public WarehouseExport WarehouseExport => new(Orders);

    /// <summary>Places a one-line order for <paramref name="amount"/> and returns its id.</summary>
    public OrderId PlaceOrder(CustomerId customer, decimal amount) =>
        CreateOrder.Handle(new CreateOrder(customer, [new("SKU-1", 1, amount)])).Value;

    public OrderId PlacePaidOrder(CustomerId customer, decimal amount)
    {
        var id = PlaceOrder(customer, amount);
        PayOrder.Handle(new PayOrder(id));
        return id;
    }
}
