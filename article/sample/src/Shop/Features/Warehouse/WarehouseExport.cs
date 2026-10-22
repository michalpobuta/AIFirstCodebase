using System.Globalization;
using System.Text;
using Shop.Features.Orders;

namespace Shop.Features.Warehouse;

/// <summary>
/// The CSV feed the warehouse system polls every five minutes. Its parser isn't ours:
/// column order and status codes are a contract, pinned by WarehouseExportTests.
/// </summary>
public sealed class WarehouseExport(OrderStore orders)
{
    public string ToCsv()
    {
        var csv = new StringBuilder("ORDER;STATUS;TOTAL\n");
        foreach (var order in orders.All())
            csv.Append(CultureInfo.InvariantCulture, $"{order.Id};{CodeFor(order.Status)};{order.Total:F2}\n");

        return csv.ToString();
    }

    // No `_ =>` arm on purpose: a new OrderStatus fails the build here until someone decides its code.
    private static string CodeFor(OrderStatus status) => status switch
    {
        OrderStatus.Pending => "NEW",
        OrderStatus.Paid => "PAID",
        OrderStatus.Shipped => "SHP",
    };
}
