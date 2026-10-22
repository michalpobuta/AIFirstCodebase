namespace Shop.Features.Warehouse;

public static class WarehouseModule
{
    public static IServiceCollection AddWarehouse(this IServiceCollection services) =>
        services.AddTransient<WarehouseExport>();

    public static IEndpointRouteBuilder MapWarehouse(this IEndpointRouteBuilder app)
    {
        app.MapGet("/warehouse/export.csv", (WarehouseExport export) => Results.Text(export.ToCsv(), "text/csv"));
        return app;
    }
}
