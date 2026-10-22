using System.Globalization;
using Shop.Features.Customers;
using Shop.Features.Orders;

namespace Shop.Tests.Features.Orders;

/// <summary>
/// Characterization: what DiscountPolicy does today, not what it should do. The inputs were proposed;
/// the totals in the .verified.txt were recorded by running the code, and a human sorted every line
/// (keep · bug · delete) before approving it. When a line becomes a real rule, it moves to DiscountPolicyTests.
/// </summary>
public sealed class DiscountPolicyCharacterizationTests
{
    private static readonly (CustomerId Customer, decimal Subtotal)[] Inputs =
    [
        (TestShop.Vip, 0.00m),
        (TestShop.Vip, 499.99m),
        (TestShop.Vip, 500.00m),
        (TestShop.Vip, 500.05m),
        (TestShop.Vip, 999.995m),
        (TestShop.Regular, 500.00m),
        (TestShop.Regular, 10.005m),
        (new CustomerId("C-404"), 500.00m),
    ];

    [Fact]
    public void TotalFor_currently_returns_these_totals()
    {
        var policy = new DiscountPolicy(new CustomerDirectory());

        var table = string.Concat(Inputs.Select(input => string.Create(CultureInfo.InvariantCulture,
            $"{input.Customer};{input.Subtotal};{policy.TotalFor(input.Customer, input.Subtotal)}\n")));

        Snapshot.Matches("CUSTOMER;SUBTOTAL;TOTAL\n" + table);
    }
}
