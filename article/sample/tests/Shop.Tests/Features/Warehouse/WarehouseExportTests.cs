using Shop.Features.Orders;

namespace Shop.Tests.Features.Warehouse;

/// <summary>
/// The warehouse system parses this CSV. The approved snapshot next to this file is the contract:
/// a change to it is a change to someone else's integration, so a human approves it.
/// </summary>
public sealed class WarehouseExportTests
{
    [Fact]
    public void ToCsv_OrdersInEveryStatus_MatchesTheFormatTheWarehouseParses()
    {
        var shop = new TestShop();
        shop.PlaceOrder(TestShop.Regular, 120.00m);
        shop.Clock.Advance(TimeSpan.FromMinutes(1));
        shop.PlacePaidOrder(TestShop.Regular, 80.50m);
        shop.Clock.Advance(TimeSpan.FromMinutes(1));
        var shipped = shop.PlacePaidOrder(TestShop.Vip, 600.00m);
        shop.ShipOrder.Handle(new ShipOrder(shipped));

        Snapshot.Matches(shop.WarehouseExport.ToCsv());
    }
}
