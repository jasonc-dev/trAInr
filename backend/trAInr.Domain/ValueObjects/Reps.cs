namespace trAInr.Domain.ValueObjects;

/// <summary>
///     Value object representing repetitions for an exercise set.
///     Enforces invariants: reps must be positive.
/// </summary>
public record struct Reps
{
    private Reps(int value)
    {
        if (value <= 0)
            throw new ArgumentException("Reps must be positive", nameof(value));

        Value = value;
    }

    public int Value { get; init; }

    public static Reps Create(int value)
    {
        return new Reps(value);
    }

    public static implicit operator int(Reps reps)
    {
        return reps.Value;
    }

    public static implicit operator Reps(int value)
    {
        return Create(value);
    }
}