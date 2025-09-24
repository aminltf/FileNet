namespace FileNet.Domain.Primitives;

public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj) =>
        obj is ValueObject other && Equals(other);

    public bool Equals(ValueObject? other)
    {
        if (other is null || other.GetType() != GetType()) return false;

        using var a = GetEqualityComponents().GetEnumerator();
        using var b = other.GetEqualityComponents().GetEnumerator();
        while (a.MoveNext() && b.MoveNext())
        {
            if (a.Current is null ^ b.Current is null) return false;
            if (a.Current is not null && !a.Current.Equals(b.Current)) return false;
        }
        return !a.MoveNext() && !b.MoveNext();
    }

    public override int GetHashCode() =>
        GetEqualityComponents().Aggregate(1, HashCode.Combine);

    public static bool operator ==(ValueObject? x, ValueObject? y) =>
        x is null ? y is null : x.Equals(y);
    public static bool operator !=(ValueObject? x, ValueObject? y) => !(x == y);
}
