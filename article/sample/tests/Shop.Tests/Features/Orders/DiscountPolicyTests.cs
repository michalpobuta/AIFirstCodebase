using Shop.Features.Customers;
using Shop.Features.Orders;

namespace Shop.Tests.Features.Orders;

public sealed class DiscountPolicyTests
{
    private readonly DiscountPolicy _policy = new(new CustomerDirectory());

    [Fact]
    public void TotalFor_VipAtThreshold_Gets10PercentOff() =>
        Assert.Equal(450.00m, _policy.TotalFor(TestShop.Vip, 500.00m));

    [Fact]
    public void TotalFor_VipBelowThreshold_PaysFullPrice() =>
        Assert.Equal(499.99m, _policy.TotalFor(TestShop.Vip, 499.99m));

    [Fact]
    public void TotalFor_RegularCustomer_PaysFullPrice() =>
        Assert.Equal(1000.00m, _policy.TotalFor(TestShop.Regular, 1000.00m));

    [Fact]
    public void TotalFor_DiscountEndingInHalfACent_RoundsAwayFromZero() =>
        Assert.Equal(450.05m, _policy.TotalFor(TestShop.Vip, 500.05m)); // 450.045 → 450.05, not 450.04
}
