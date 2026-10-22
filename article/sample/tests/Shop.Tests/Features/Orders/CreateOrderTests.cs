using Shop.Features.Orders;

namespace Shop.Tests.Features.Orders;

public sealed class CreateOrderTests
{
    private readonly TestShop _shop = new();

    [Fact]
    public void Handle_ValidOrder_StoresPendingOrderWithTotalAndCreationTime()
    {
        var result = _shop.CreateOrder.Handle(new CreateOrder(TestShop.Regular, [new("SKU-1", 2, 25m)]));

        var order = _shop.Orders.Find(result.Value)!;
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(50m, order.Total);
        Assert.Equal(_shop.Clock.GetUtcNow(), order.CreatedAt);
    }

    [Fact]
    public void Handle_UnknownCustomer_IsRejectedAsInvalid()
    {
        var result = _shop.CreateOrder.Handle(new CreateOrder(new CustomerId("C-404"), [new("SKU-1", 1, 10m)]));

        Assert.Equal(ErrorKind.Invalid, result.Error?.Kind);
    }

    [Fact]
    public void Handle_NoLines_IsRejectedAsInvalid()
    {
        var result = _shop.CreateOrder.Handle(new CreateOrder(TestShop.Regular, []));

        Assert.Equal(ErrorKind.Invalid, result.Error?.Kind);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    public void Handle_NonPositiveQuantityOrPrice_IsRejectedAsInvalid(int quantity, int unitPrice)
    {
        var result = _shop.CreateOrder.Handle(new CreateOrder(TestShop.Regular, [new("SKU-1", quantity, unitPrice)]));

        Assert.Equal(ErrorKind.Invalid, result.Error?.Kind);
    }
}
