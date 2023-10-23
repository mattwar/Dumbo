//#define ALLOW_NULL
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Dumbo.TypeUnions.Existing.Boxed;

[DebuggerDisplay("{DebugText}")]
public readonly struct OneOf<T1, T2> : ITypeUnion<OneOf<T1, T2>>
{
    private readonly string DebugText => ToString();

    private readonly object _value;

    private OneOf(object value)
    {
        _value = value;
    }

    public static OneOf<T1, T2> Create(T1 value) =>
        new OneOf<T1, T2>(value!);

    public static OneOf<T1, T2> Create(T2 value) =>
        new OneOf<T1, T2>(value!);

    public static OneOf<T1, T2> Create<T>(T value) =>
        TryCreate(value, out var converted)
            ? converted
            : throw new InvalidCastException();

    public static bool TryCreate<T>(T value, [NotNullWhen(true)] out OneOf<T1, T2> converted)
    {
        switch (value)
        {
            case T1 t1:
                converted = Create(t1);
                return true;
            case T2 t2:
                converted = Create(t2);
                return true;
            case ITypeUnion union:
                if (union.TryGet<T1>(out var u1))
                    return TryCreate(u1, out converted);
                else if (union.TryGet<T2>(out var u2))
                    return TryCreate(u2, out converted);
                break;
#if ALLOW_NULL
            case null:
                converted = default;
                return true;
#endif
        }

        converted = default;
        return false;
    }

    public bool IsType<T>() => _value is T;

    public T AsType<T>() => _value is T t ? t : default!;

    public T Get<T>() => _value is T t ? t : throw new InvalidCastException();

    public bool TryGet<T>([NotNullWhen(true)] out T value)
    {
        if (_value is T t)
        {
            value = t;
            return true;
        }
        value = default!;
        return false;
    }

    public Type? Type => _value?.GetType();


    public override string ToString() => _value?.ToString()!;

    // equality
    public bool Equals<T>(T? other)
    {
        if (_value is T tval)
        {
            return tval.Equals(other);
        }
        else if (other is ITypeUnion union && union.TryGet<object>(out var obj))
        {
            return Equals(_value, obj);
        }
        else
        {
            return Equals(_value, other);
        }
    }

    public override bool Equals([NotNullWhen(true)] object? other) =>
        Equals(other);

    public override int GetHashCode() => _value?.GetHashCode() ?? 0;

    // operators
    public static implicit operator OneOf<T1, T2>(T1 value) => Create(value);
    public static implicit operator OneOf<T1, T2>(T2 value) => Create(value);
    public static implicit operator OneOf<T1, T2>(OneOf<T2, T1> value) => Create(value);

    public static explicit operator T1(OneOf<T1, T2> value) => value.Get<T1>();
    public static explicit operator T2(OneOf<T1, T2> value) => value.Get<T2>();

    public static bool operator ==(OneOf<T1, T2> value, T1 other) => value.TryGet<T1>(out var tval) && tval.Equals(other);
    public static bool operator !=(OneOf<T1, T2> value, T1 other) => !value.TryGet<T1>(out var tval) || !tval.Equals(other);
    public static bool operator ==(T1 other, OneOf<T1, T2> value) => value.TryGet<T1>(out var tval) && tval.Equals(other);
    public static bool operator !=(T1 other, OneOf<T1, T2> value) => !value.TryGet<T1>(out var tval) || !tval.Equals(other);
    public static bool operator ==(OneOf<T1, T2> value, T2 other) => value.TryGet<T2>(out var tval) && tval.Equals(other);
    public static bool operator !=(OneOf<T1, T2> value, T2 other) => !value.TryGet<T2>(out var tval) || !tval.Equals(other);
    public static bool operator ==(T2 other, OneOf<T1, T2> value) => value.TryGet<T2>(out var tval) && tval.Equals(other);
    public static bool operator !=(T2 other, OneOf<T1, T2> value) => !value.TryGet<T2>(out var tval) || !tval.Equals(other);
}

[DebuggerDisplay("{DebugText}")]
public readonly struct OneOf<T1, T2, T3> : ITypeUnion<OneOf<T1, T2, T3>>
{
    private readonly string DebugText => ToString();

    private readonly object _value;

    private OneOf(object value)
    {
        _value = value;
    }

    public static OneOf<T1, T2, T3> Create(T1 value) =>
        new OneOf<T1, T2, T3>(value!);

    public static OneOf<T1, T2, T3> Create(T2 value) =>
        new OneOf<T1, T2, T3>(value!);

    public static OneOf<T1, T2, T3> Create(T3 value) =>
        new OneOf<T1, T2, T3>(value!);

    public static OneOf<T1, T2, T3> Create<T>(T other) =>
        TryCreate(other, out var converted)
            ? converted
            : throw new InvalidCastException();

    public static bool TryCreate<T>(T value, [NotNullWhen(true)] out OneOf<T1, T2, T3> converted)
    {
        switch (value)
        {
            case T1 t1:
                converted = Create(t1);
                return true;
            case T2 t2:
                converted = Create(t2);
                return true;
            case T3 t3:
                converted = Create(t3);
                return true;
            case ITypeUnion union:
                if (union.TryGet<T1>(out var u1))
                    return TryCreate(u1, out converted);
                else if (union.TryGet<T2>(out var u2))
                    return TryCreate(u2, out converted);
                else if (union.TryGet<T3>(out var u3))
                    return TryCreate(u3, out converted);
                break;
#if ALLOW_NULL
            case null:
                converted = default;
                return true;
#endif
        }

        converted = default;
        return false;
    }

    public bool IsType<T>() => _value is T;

    public T AsType<T>() => _value is T t ? t : default!;

    public T Get<T>() => _value is T t ? t : throw new InvalidCastException();

    public bool TryGet<T>([NotNullWhen(true)] out T value)
    {
        if (_value is T t)
        {
            value = t;
            return true;
        }
        value = default!;
        return false;
    }

    public Type? Type => _value?.GetType();

    public override string ToString() => _value?.ToString()!;

    // equality
    public bool Equals<T>(T? other)
    {
        if (_value is T tval)
        {
            return tval.Equals(other);
        }
        else if (other is ITypeUnion union && union.TryGet<object>(out var obj))
        {
            return Equals(_value, obj);
        }
        else
        {
            return Equals(_value, other);
        }
    }

    public override bool Equals([NotNullWhen(true)] object? other) =>
        Equals(other);

    public override int GetHashCode() => _value?.GetHashCode() ?? 0;

    // operators
    public static implicit operator OneOf<T1, T2, T3>(T1 value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3>(T2 value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3>(T3 value) => Create(value);

    public static implicit operator OneOf<T1, T2, T3>(OneOf<T1, T2> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3>(OneOf<T2, T1> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3>(OneOf<T1, T3> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3>(OneOf<T3, T1> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3>(OneOf<T2, T3> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3>(OneOf<T3, T2> value) => Create(value);

    public static implicit operator OneOf<T1, T2, T3>(OneOf<T1, T3, T2> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3>(OneOf<T2, T1, T3> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3>(OneOf<T2, T3, T1> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3>(OneOf<T3, T1, T2> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3>(OneOf<T3, T2, T1> value) => Create(value);

    public static explicit operator T1(OneOf<T1, T2, T3> value) => value.Get<T1>();
    public static explicit operator T2(OneOf<T1, T2, T3> value) => value.Get<T2>();
    public static explicit operator T3(OneOf<T1, T2, T3> value) => value.Get<T3>();

    public static bool operator ==(OneOf<T1, T2, T3> value, T1 other) => value.TryGet<T1>(out var tval) && tval.Equals(other);
    public static bool operator !=(OneOf<T1, T2, T3> value, T1 other) => !value.TryGet<T1>(out var tval) || !tval.Equals(other);
    public static bool operator ==(T1 other, OneOf<T1, T2, T3> value) => value.TryGet<T1>(out var tval) && tval.Equals(other);
    public static bool operator !=(T1 other, OneOf<T1, T2, T3> value) => !value.TryGet<T1>(out var tval) || !tval.Equals(other);
    public static bool operator ==(OneOf<T1, T2, T3> value, T2 other) => value.TryGet<T2>(out var tval) && tval.Equals(other);
    public static bool operator !=(OneOf<T1, T2, T3> value, T2 other) => !value.TryGet<T2>(out var tval) || !tval.Equals(other);
    public static bool operator ==(T2 other, OneOf<T1, T2, T3> value) => value.TryGet<T2>(out var tval) && tval.Equals(other);
    public static bool operator !=(T2 other, OneOf<T1, T2, T3> value) => !value.TryGet<T2>(out var tval) || !tval.Equals(other);
    public static bool operator ==(OneOf<T1, T2, T3> value, T3 other) => value.TryGet<T3>(out var tval) && tval.Equals(other);
    public static bool operator !=(OneOf<T1, T2, T3> value, T3 other) => !value.TryGet<T3>(out var tval) || !tval.Equals(other);
    public static bool operator ==(T3 other, OneOf<T1, T2, T3> value) => value.TryGet<T3>(out var tval) && tval.Equals(other);
    public static bool operator !=(T3 other, OneOf<T1, T2, T3> value) => !value.TryGet<T3>(out var tval) || !tval.Equals(other);
}

#if false
[DebuggerDisplay("{DebugText}")]
public readonly struct OneOf<T1, T2, T3, T4> : ITypeUnion<OneOf<T1, T2, T3, T4>>
{
    private readonly string DebugText => ToString();

    private readonly object _value;

    private OneOf(object value)
    {
        _value = value;
    }

    public static OneOf<T1, T2, T3, T4> Create(T1 value) =>
        new OneOf<T1, T2, T3, T4>(value!);

    public static OneOf<T1, T2, T3, T4> Create(T2 value) =>
        new OneOf<T1, T2, T3, T4>(value!);

    public static OneOf<T1, T2, T3, T4> Create(T3 value) =>
        new OneOf<T1, T2, T3, T4>(value!);

    public static OneOf<T1, T2, T3, T4> Create(T4 value) =>
        new OneOf<T1, T2, T3, T4>(value!);

    public static OneOf<T1, T2, T3, T4> Create<T>(T other) =>
        TryCreate(other, out var converted) 
            ? converted 
            : throw new InvalidCastException();

    public static bool TryCreate<T>(T value, [NotNullWhen(true)] out OneOf<T1, T2, T3, T4> converted)
    {
        switch (value)
        {
            case T1 t1:
                converted = Create(t1);
                return true;
            case T2 t2:
                converted = Create(t2);
                return true;
            case T3 t3:
                converted = Create(t3);
                return true;
            case T4 t4:
                converted = Create(t4);
                return true;
            case ITypeUnion union:
                if (union.TryGet<T1>(out var u1))
                    return TryCreate(u1, out converted);
                else if (union.TryGet<T2>(out var u2))
                    return TryCreate(u2, out converted);
                else if (union.TryGet<T3>(out var u3))
                    return TryCreate(u3, out converted);
                else if (union.TryGet<T4>(out var u4))
                    return TryCreate(u4, out converted);
                break;
#if ALLOW_NULL
            case null:
                converted = default;
                return true;
#endif
        }

        converted = default;
        return false;
    }

    public bool IsType<T>() => _value is T;

    public T AsType<T>() => _value is T t ? t : default!;

    public T Get<T>() => _value is T t ? t : throw new InvalidCastException();

    public bool TryGet<T>([NotNullWhen(true)] out T value)
    {
        if (_value is T t)
        {
            value = t;
            return true;
        }
        value = default!;
        return false;
    }

    public Type? Type => _value?.GetType();

    public override string ToString() => _value?.ToString()!;

    // equality
    public bool Equals<T>(T? other)
    {
        if (_value is T tval)
        {
            return tval.Equals(other);
        }
        else if (other is ITypeUnion union && union.TryGet<object>(out var obj))
        {
            return Equals(_value, obj);
        }
        else
        {
            return Equals(_value, other);
        }
    }

    public override bool Equals([NotNullWhen(true)] object? other) =>
        Equals(other);

    public override int GetHashCode() => _value?.GetHashCode() ?? 0;

    // operators
    public static implicit operator OneOf<T1, T2, T3, T4>(T1 value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(T2 value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(T3 value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(T4 value) => Create(value);

    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T2> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T3> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T4> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T1> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T3> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T4> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T1> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T2> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T4> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T1> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T2> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T3> value) => Create(value);

    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T2, T3> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T2, T4> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T3, T2> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T3, T4> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T4, T2> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T4, T3> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T1, T3> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T1, T4> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T3, T1> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T3, T4> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T4, T1> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T4, T3> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T1, T2> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T1, T4> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T2, T1> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T2, T4> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T4, T1> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T4, T2> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T1, T2> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T1, T3> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T2, T1> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T2, T3> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T3, T1> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T3, T2> value) => Create(value);

    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T2, T4, T3> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T3, T2, T4> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T3, T4, T2> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T4, T2, T3> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T4, T3, T2> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T1, T3, T4> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T1, T4, T3> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T3, T1, T4> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T3, T4, T1> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T4, T1, T3> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T4, T3, T1> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T1, T2, T4> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T1, T4, T2> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T2, T1, T4> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T2, T4, T1> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T4, T1, T2> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T4, T2, T1> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T1, T2, T3> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T1, T3, T2> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T2, T1, T3> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T2, T3, T1> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T3, T1, T2> value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T3, T2, T1> value) => Create(value);

    public static explicit operator T1(OneOf<T1, T2, T3, T4> value) => value.Get<T1>();
    public static explicit operator T2(OneOf<T1, T2, T3, T4> value) => value.Get<T2>();
    public static explicit operator T3(OneOf<T1, T2, T3, T4> value) => value.Get<T3>();
    public static explicit operator T4(OneOf<T1, T2, T3, T4> value) => value.Get<T4>();

    public static bool operator ==(OneOf<T1, T2, T3, T4> value, T1 other) => value.TryGet<T1>(out var tval) && tval.Equals(other);
    public static bool operator !=(OneOf<T1, T2, T3, T4> value, T1 other) => !value.TryGet<T1>(out var tval) || !tval.Equals(other);
    public static bool operator ==(T1 other, OneOf<T1, T2, T3, T4> value) => value.TryGet<T1>(out var tval) && tval.Equals(other);
    public static bool operator !=(T1 other, OneOf<T1, T2, T3, T4> value) => !value.TryGet<T1>(out var tval) || !tval.Equals(other);
    public static bool operator ==(OneOf<T1, T2, T3, T4> value, T2 other) => value.TryGet<T2>(out var tval) && tval.Equals(other);
    public static bool operator !=(OneOf<T1, T2, T3, T4> value, T2 other) => !value.TryGet<T2>(out var tval) || !tval.Equals(other);
    public static bool operator ==(T2 other, OneOf<T1, T2, T3, T4> value) => value.TryGet<T2>(out var tval) && tval.Equals(other);
    public static bool operator !=(T2 other, OneOf<T1, T2, T3, T4> value) => !value.TryGet<T2>(out var tval) || !tval.Equals(other);
    public static bool operator ==(OneOf<T1, T2, T3, T4> value, T3 other) => value.TryGet<T3>(out var tval) && tval.Equals(other);
    public static bool operator !=(OneOf<T1, T2, T3, T4> value, T3 other) => !value.TryGet<T3>(out var tval) || !tval.Equals(other);
    public static bool operator ==(T3 other, OneOf<T1, T2, T3, T4> value) => value.TryGet<T3>(out var tval) && tval.Equals(other);
    public static bool operator !=(T3 other, OneOf<T1, T2, T3, T4> value) => !value.TryGet<T3>(out var tval) || !tval.Equals(other);
    public static bool operator ==(OneOf<T1, T2, T3, T4> value, T4 other) => value.TryGet<T4>(out var tval) && tval.Equals(other);
    public static bool operator !=(OneOf<T1, T2, T3, T4> value, T4 other) => !value.TryGet<T4>(out var tval) || !tval.Equals(other);
    public static bool operator ==(T4 other, OneOf<T1, T2, T3, T4> value) => value.TryGet<T4>(out var tval) && tval.Equals(other);
    public static bool operator !=(T4 other, OneOf<T1, T2, T3, T4> value) => !value.TryGet<T4>(out var tval) || !tval.Equals(other);
}
#endif