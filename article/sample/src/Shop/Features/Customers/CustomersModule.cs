namespace Shop.Features.Customers;

public static class CustomersModule
{
    public static IServiceCollection AddCustomers(this IServiceCollection services) =>
        services.AddSingleton<CustomerDirectory>();
}
