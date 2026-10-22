using Shop.Features.Orders;

namespace Shop.Tests.Features.Orders;

public sealed class PayOrderTests
{
    private readonly TestShop _shop = new();

    [Fact]
    public void Handle_PendingOrder_MarksItPaidAtTheCurrentTime()
    {
        var id = _shop.PlaceOrder(TestShop.Regular, 100m);
        _shop.Clock.Advance(TimeSpan.FromMinutes(5));

        var result = _shop.PayOrder.Handle(new PayOrder(id));

        Assert.True(result.IsSuccess);
        var order = _shop.Orders.Find(id)!;
        Assert.Equal(OrderStatus.Paid, order.Status);
        Assert.Equal(_shop.Clock.GetUtcNow(), order.PaidAt);
    }

    [Fact]
    public void Handle_AlreadyPaidOrder_IsRejectedAsConflict()
    {
        var id = _shop.PlacePaidOrder(TestShop.Regular, 100m);

        var result = _shop.PayOrder.Handle(new PayOrder(id));

        Assert.Equal(ErrorKind.Conflict, result.Error?.Kind);
    }

    [Fact]
    public void Handle_UnknownOrder_IsRejectedAsNotFound()
    {
        var result = _shop.PayOrder.Handle(new PayOrder(OrderId.New()));

        Assert.Equal(ErrorKind.NotFound, result.Error?.Kind);
    }
}
