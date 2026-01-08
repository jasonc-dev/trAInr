namespace trAInr.Domain.ValueObjects;

/// <summary>
///     Value object representing a load (weight) for an exercise set.
///     Enforces invariants: weight must be non-negative.
/// </summary>
public record struct Load
{
    private Load(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("Load cannot be negative", nameof(value));

        Value = value;
    }

    public decimal Value { get; init; }

    public static Load Zero => new(0);

    public static Load Create(decimal value)
    {
        return new Load(value);
    }

    public static implicit operator decimal(Load load)
    {
        return load.Value;
    }

    public static implicit operator Load(decimal value)
    {
        return Create(value);
    }
}