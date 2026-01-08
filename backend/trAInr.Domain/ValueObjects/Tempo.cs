namespace trAInr.Domain.ValueObjects;

/// <summary>
///     Value object representing tempo for an exercise (e.g., 3-1-2-0).
///     Format: Eccentric-Pause-Concentric-Pause (in seconds).
/// </summary>
public record struct Tempo
{
    private Tempo(int eccentric, int pause1, int concentric, int pause2)
    {
        if (eccentric < 0 || pause1 < 0 || concentric < 0 || pause2 < 0)
            throw new ArgumentException("Tempo values cannot be negative");

        Eccentric = eccentric;
        Pause1 = pause1;
        Concentric = concentric;
        Pause2 = pause2;
    }

    public int Eccentric { get; init; }
    public int Pause1 { get; init; }
    public int Concentric { get; init; }
    public int Pause2 { get; init; }

    public static Tempo Create(int eccentric, int pause1, int concentric, int pause2)
    {
        return new Tempo(eccentric, pause1, concentric, pause2);
    }

    public static Tempo Create(int eccentric, int pause1, int concentric)
    {
        return new Tempo(eccentric, pause1, concentric, 0);
    }

    public static Tempo Create(int eccentric, int concentric)
    {
        return new Tempo(eccentric, 0, concentric, 0);
    }

    public override string ToString()
    {
        return $"{Eccentric}-{Pause1}-{Concentric}-{Pause2}";
    }
}