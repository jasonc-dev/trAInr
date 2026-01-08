namespace trAInr.Domain.ValueObjects;

/// <summary>
///     Value object representing Rate of Perceived Exertion (RPE).
///     RPE scale typically ranges from 1-10, where 10 is maximum effort.
/// </summary>
public record struct RPE
{
    private RPE(int value)
    {
        if (value < 1 || value > 10)
            throw new ArgumentException("RPE must be between 1 and 10", nameof(value));

        Value = value;
    }

    public int Value { get; init; }

    public static RPE Create(int value)
    {
        return new RPE(value);
    }

    public static implicit operator int(RPE rpe)
    {
        return rpe.Value;
    }

    public static implicit operator RPE(int value)
    {
        return Create(value);
    }
}