using Shop.Features.Orders;

namespace Shop.Tests.Features.Orders;

public sealed class ShipOrderTests
{
    private readonly TestShop _shop = new();

    [Fact]
    public void Handle_PaidOrder_MarksItShippedAtTheCurrentTime()
    {
        var id = _shop.PlacePaidOrder(TestShop.Regular, 100m);
        _shop.Clock.Advance(TimeSpan.FromHours(2));

        var result = _shop.ShipOrder.Handle(new ShipOrder(id));

        Assert.True(result.IsSuccess);
        var order = _shop.Orders.Find(id)!;
        Assert.Equal(OrderStatus.Shipped, order.Status);
        Assert.Equal(_shop.Clock.GetUtcNow(), order.ShippedAt);
    }

    [Fact]
    public void Handle_PendingOrder_IsRejectedAsConflict()
    {
        var id = _shop.PlaceOrder(TestShop.Regular, 100m);

        var result = _shop.ShipOrder.Handle(new ShipOrder(id));

        Assert.Equal(ErrorKind.Conflict, result.Error?.Kind);
    }
}
