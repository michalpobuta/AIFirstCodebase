using Shop.Shared;

namespace Shop.Features.Customers;

public sealed record Customer(CustomerId Id, string Name, bool IsVip);

/// <summary>Customers known to the shop. Seeded in memory — the demo has no customer management.</summary>
public sealed class CustomerDirectory
{
    private readonly Dictionary<CustomerId, Customer> _customers = new Customer[]
    {
        new(new CustomerId("C-001"), "Ada Lovelace", IsVip: true),
        new(new CustomerId("C-002"), "Linus Torvalds", IsVip: false),
    }.ToDictionary(customer => customer.Id);

    public Customer? Find(CustomerId id) => _customers.GetValueOrDefault(id);
}
