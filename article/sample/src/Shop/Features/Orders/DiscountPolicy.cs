using Shop.Features.Customers;
using Shop.Shared;

namespace Shop.Features.Orders;

/// <summary>VIP customers get 10% off orders of 500.00 or more — agreed with sales in 2023.</summary>
public sealed class DiscountPolicy(CustomerDirectory customers)
{
    public const decimal VipThreshold = 500m;
    public const decimal VipDiscount = 0.10m;

    public decimal TotalFor(CustomerId customerId, decimal subtotal)
    {
        var isVip = customers.Find(customerId)?.IsVip ?? false;
        var total = isVip && subtotal >= VipThreshold ? subtotal * (1 - VipDiscount) : subtotal;

        // Finance reconciles against invoices rounded half away from zero, not banker's rounding.
        return Math.Round(total, 2, MidpointRounding.AwayFromZero);
    }
}
