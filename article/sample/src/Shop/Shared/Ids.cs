namespace Shop.Shared;

public readonly record struct OrderId(Guid Value)
{
    public static OrderId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}

public readonly record struct CustomerId(string Value)
{
    public override string ToString() => Value;
}
