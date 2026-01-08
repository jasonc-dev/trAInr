namespace trAInr.Domain.ValueObjects;

/// <summary>
///     Value object representing fatigue index.
///     Typically ranges from 0-100, where higher values indicate more fatigue.
/// </summary>
public record struct FatigueIndex
{
    private FatigueIndex(decimal value)
    {
        if (value is < 0 or > 100)
            throw new ArgumentException("FatigueIndex must be between 0 and 100", nameof(value));

        Value = value;
    }

    public decimal Value { get; init; }

    public static FatigueIndex Zero => new(0);

    public static FatigueIndex Create(decimal value)
    {
        return new FatigueIndex(value);
    }

    public static implicit operator decimal(FatigueIndex fatigueIndex)
    {
        return fatigueIndex.Value;
    }

    public static implicit operator FatigueIndex(decimal value)
    {
        return Create(value);
    }
}