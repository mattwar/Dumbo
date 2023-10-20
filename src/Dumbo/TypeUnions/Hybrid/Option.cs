using System.Diagnostics.CodeAnalysis;

namespace Dumbo.TypeUnions.Hybrid;

// pre-exising types
public record Some<TValue>(TValue value);
public record None() { public static readonly None Instance = new None(); }

public readonly struct Option<TValue> : ITypeUnion<Option<TValue>>
{
    private readonly Variant _value;

    private Option(Variant value)
    {
        _value = value;
    }

    public static Option<TValue> Create(Some<TValue> value) =>
        new Option<TValue>(Variant.Create(value));

    public static Option<TValue> Create(None none) =>
        new Option<TValue>(Variant.Create(none));

    public static bool TryCreate<TOther>(TOther other, [NotNullWhen(true)] out Option<TValue> value)
    {
        switch (other)
        {
            case Some<TValue> type1:
                value = Create(type1);
                return true;
            case None type2:
                value = Create(type2);
                return true;
            case ITypeUnion u:
                if (u.TryGet<TValue>(out var t))
                    return TryCreate(t, out value);
                else if (u.TryGet<Some<TValue>>(out var st))
                    return TryCreate(st, out value);
                else if (u.TryGet<None>(out var nt))
                    return TryCreate(nt, out value);
                break;
        }

        value = default!;
        return false;
    }
    public static Option<TValue> Create<TOther>(TOther other) =>
        TryCreate(other, out var value) ? value : throw new InvalidCastException();

    public bool IsType<T>() =>
        _value.IsType<T>();

    public bool TryGet<T>([NotNullWhen(true)] out T value) =>
        _value.TryGet(out value);

    public T Get<T>() =>
        TryGet<T>(out var value)
            ? value
            : throw new InvalidCastException();

    public static implicit operator Option<TValue>(Some<TValue> value) => Create(value);
    public static implicit operator Option<TValue>(None value) => Create(value);
}
