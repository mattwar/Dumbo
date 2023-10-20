using System.Diagnostics.CodeAnalysis;

namespace Dumbo.TypeUnion.Overlapped;

// pre-exising types
public record struct Some<TValue>(TValue value);
public record struct None() { public static readonly None Instance = new None(); }

public readonly struct Option<TValue> : ITypeUnion<Option<TValue>>
{
    private enum Kind { Type1 = 1, Type2 };

    private readonly Kind _kind;
    private readonly TValue _type1_value;

    private Option(Kind kind, TValue type1_value)
    {
        _kind = kind;
        _type1_value = type1_value;
    }

    public static Option<TValue> Create(Some<TValue> value) =>
        new Option<TValue>(Kind.Type1, value.value);

    public static Option<TValue> Create(None none) =>
        new Option<TValue>(Kind.Type2, default!);

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


    private bool TryGetType1(out Some<TValue> value)
    {
        if (_kind == Kind.Type1 && _type1_value is TValue t1val)
        {
            value = new Some<TValue>(t1val);
            return true;
        }
        value = default!;
        return false;
    }

    private Some<TValue> GetType1() =>
        TryGetType1(out var type1)
            ? type1
            : throw new InvalidCastException();

    private bool TryGetType2(out None value)
    {
        if (_kind == Kind.Type2)
        {
            value = new None();
            return true;
        }
        value = default!;
        return false;
    }

    private None GetType2() =>
        TryGetType2(out var type2)
            ? type2
            : throw new InvalidCastException();

    public bool IsType<T>() =>
        _kind switch
        {
            Kind.Type1 => GetType1() is T,
            Kind.Type2 => GetType2() is T,
            _ => false
        };

    public bool TryGet<T>([NotNullWhen(true)] out T value)
    {
        if (_kind == Kind.Type1 && GetType1() is T t1val)
        {
            value = t1val;
            return true;
        }
        else if (_kind == Kind.Type2 && GetType2() is T t2val)
        {
            value = t2val;
            return true;
        }
        value = default!;
        return false;
    }

    public T Get<T>() =>
        TryGet<T>(out var value)
            ? value
            : throw new InvalidCastException();

    public static implicit operator Option<TValue>(Some<TValue> value) => Create(value);
    public static implicit operator Option<TValue>(None value) => Create(value);
}
