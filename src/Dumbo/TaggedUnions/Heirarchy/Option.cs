using System.Diagnostics.CodeAnalysis;

namespace Dumbo.TaggedUnions.Heirarchy;

public abstract record Option<TValue>()
{
    public record Some(TValue value) : Option<TValue>;
    public record None() : Option<TValue> { public readonly None Instance = new None(); }

    public bool IsSome => this is Some;
    public bool IsNone => this is None;

    public static implicit operator Option<TValue>(TValue value) =>
        new Some(value);
}
