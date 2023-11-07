//#define ALLOW_NULL
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Dumbo.TypeUnions.Existing.Fat;

[DebuggerDisplay("{DebugText}")]
public readonly struct OneOf<T1, T2> : 
    ITypeUnion<OneOf<T1, T2>>
{
    private readonly string DebugText => ToString();

    private readonly byte _index;
    private readonly T1 _value1;
    private readonly T2 _value2;

    private OneOf(byte index, T1 value1, T2 value2)
    {
        _index = index;
        _value1 = value1;
        _value2 = value2;
    }

    #region Non-Generic API
    public static OneOf<T1, T2> Create(T1 value) =>
        new OneOf<T1, T2>(1, value, default!);

    public static OneOf<T1, T2> Create(T2 value) =>
        new OneOf<T1, T2>(2, default!, value);

    public Type? Type =>
        _index switch
        {
            1 => typeof(T1),
            2 => typeof(T2),
            _ => null
        };

    public int TypeIndex => _index;
    public bool IsType1 => _index == 1;
    public bool IsType2 => _index == 2;

    public bool TryGetType1([NotNullWhen(true)] out T1 value)
    {
        if (IsType1)
        {
            value = _value1!;
            return true;
        }

        value = default!;
        return false;
    }

    public bool TryGetType2([NotNullWhen(true)] out T2 value)
    {
        if (IsType2)
        {
            value = _value2!;
            return true;
        }

        value = default!;
        return false;
    }

    public T1 GetType1() =>
        TryGetType1(out var value)
            ? value
            : throw new InvalidCastException();

    public T2 GetType2() =>
        TryGetType2(out var value)
            ? value
            : throw new InvalidCastException();

    public T1 AsType1() =>
        TryGetType1(out var value)
            ? value
            : default!;

    public T2 AsType2() =>
        TryGetType2(out var value)
            ? value
            : default!;

    public override string ToString() =>
        _index switch
        {
            1 => _value1?.ToString()!,
            2 => _value2?.ToString()!,
            _ => null!
        };

    public Variant ToVariant() =>
        _index switch
        {
            1 => Variant.Create(_value1),
            2 => Variant.Create(_value2),
            _ => Variant.Null
        };

    public bool Equals(T1 value) =>
        TryGetType1(out var value1) && EqualityComparer<T1>.Default.Equals(value1, value);

    public bool Equals(T2 value) =>
        TryGetType2(out var value1) && EqualityComparer<T2>.Default.Equals(value1, value);

    public bool Equals(OneOf<T1, T2> value) =>
        _index switch
        {
            1 => value.Equals(_value1),
            2 => value.Equals(_value2),
            _ => false
        };

    public override bool Equals([NotNullWhen(true)] object? value) =>
        Equals<object?>(value);

    public override int GetHashCode() =>
        _index switch
        {
            1 => _value1?.GetHashCode() ?? 0,
            2 => _value2?.GetHashCode() ?? 0,
            _ => 0
        };
    #endregion

    #region Generic API
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
            default:
                if (TypeUnionAccessor<T>.TryGetAccessor(out var accessor))
                {
                    if (accessor.TryGet<T1>(in value, out var t1val))
                    {
                        converted = Create(t1val);
                        return true;
                    }
                    else if (accessor.TryGet<T2>(in value, out var t2val))
                    {
                        converted = Create(t2val);
                        return true;
                    }
                }
                break;
        }

        converted = default;
        return false;
    }

    public bool IsType<T>() =>
        _index switch
        {
            1 => typeof(T) == typeof(T1) || _value1 is T,
            2 => typeof(T) == typeof(T2) || _value2 is T,
            _ => false
        };

    public T AsType<T>() =>
        _index switch
        {
            1 when _value1 is T t => t,
            2 when _value2 is T t => t,
            _ => default!
        };

    public T Get<T>() =>
        _index switch
        {
            1 when _value1 is T t => t,
            2 when _value2 is T t => t,
            _ => throw new InvalidCastException()
        };

    public bool TryGet<T>([NotNullWhen(true)] out T value)
    {
        switch (_index)
        {
            case 1 when _value1 is T t1:
                value = t1;
                return true;
            case 2 when _value2 is T t2:
                value = t2;
                return true;
            default:
                if (TypeUnionFactory<T>.TryGetFactory(out var factory))
                {
                    switch (_index)
                    {
                        case 1:
                            return factory.TryCreate(_value1, out value);
                        case 2:
                            return factory.TryCreate(_value2, out value);
                    }
                }
                break;
        }

        value = default!;
        return false;
    }

    // equality
    public bool Equals<T>(T? value)
    {
        if (_index == 1 && _value1 is T t1)
        {
            return t1.Equals(value);
        }
        else if (_index == 2 && _value2 is T t2)
        {
            return t2.Equals(value);
        }
        else if (value is ITypeUnion union)
        {
            if (_index == 1)
                return union.Equals(_value1);
            else if (_index == 2)
                return union.Equals(_value2);
        }

        return false;
    }
    #endregion

    // operators
    public static implicit operator OneOf<T1, T2>(T1 value) => Create(value);
    public static implicit operator OneOf<T1, T2>(T2 value) => Create(value);

    public static explicit operator T1(OneOf<T1, T2> value) => value.GetType1();
    public static explicit operator T2(OneOf<T1, T2> value) => value.GetType2();
}

[DebuggerDisplay("{DebugText}")]
public readonly struct OneOf<T1, T2, T3> : ITypeUnion<OneOf<T1, T2, T3>>
{
    private readonly string DebugText => ToString();

    private readonly byte _index;
    private readonly T1 _value1;
    private readonly T2 _value2;
    private readonly T3 _value3;

    private OneOf(byte index, T1 value1, T2 value2, T3 value3)
    {
        _index = index;
        _value1 = value1;
        _value2 = value2;
        _value3 = value3;
    }

    #region Non-Generic API
    public static OneOf<T1, T2, T3> Create(T1 value) =>
        new OneOf<T1, T2, T3>(1, value, default!, default!);

    public static OneOf<T1, T2, T3> Create(T2 value) =>
        new OneOf<T1, T2, T3>(2, default!, value, default!);

    public static OneOf<T1, T2, T3> Create(T3 value) =>
        new OneOf<T1, T2, T3>(3, default!, default!, value);

    public Type? Type =>
        _index switch
        {
            1 => typeof(T1),
            2 => typeof(T2),
            3 => typeof(T3),
            _ => null
        };

    public int TypeIndex => _index;
    public bool IsType1 => _index == 1;
    public bool IsType2 => _index == 2;
    public bool IsType3 => _index == 3;

    public bool TryGetType1([NotNullWhen(true)] out T1 value)
    {
        if (IsType1)
        {
            value = _value1!;
            return true;
        }

        value = default!;
        return false;
    }

    public bool TryGetType2([NotNullWhen(true)] out T2 value)
    {
        if (IsType2)
        {
            value = _value2!;
            return true;
        }

        value = default!;
        return false;
    }

    public bool TryGetType3([NotNullWhen(true)] out T3 value)
    {
        if (IsType3)
        {
            value = _value3!;
            return true;
        }

        value = default!;
        return false;
    }

    public T1 GetType1() =>
        TryGetType1(out var value)
            ? value
            : throw new InvalidCastException();

    public T2 GetType2() =>
        TryGetType2(out var value)
            ? value
            : throw new InvalidCastException();

    public T3 GetType3() =>
        TryGetType3(out var value)
            ? value
            : throw new InvalidCastException();

    public T1 AsType1() =>
        TryGetType1(out var value)
            ? value
            : default!;

    public T2 AsType2() =>
        TryGetType2(out var value)
            ? value
            : default!;

    public T3 AsType3() =>
        TryGetType3(out var value)
            ? value
            : default!;

    public override string ToString() =>
        _index switch
        {
            1 => _value1?.ToString()!,
            2 => _value2?.ToString()!,
            3 => _value3?.ToString()!,
            _ => null!
        };

    public Variant ToVariant() =>
        _index switch
        {
            1 => Variant.Create(_value1),
            2 => Variant.Create(_value2),
            3 => Variant.Create(_value3),
            _ => Variant.Null
        };

    public bool Equals(T1 value) =>
        TryGetType1(out var value1) && EqualityComparer<T1>.Default.Equals(value1, value);

    public bool Equals(T2 value) =>
        TryGetType2(out var value1) && EqualityComparer<T2>.Default.Equals(value1, value);

    public bool Equals(T3 value) =>
        TryGetType3(out var value1) && EqualityComparer<T3>.Default.Equals(value1, value);

    public bool Equals(OneOf<T1, T2, T3> union) =>
        _index switch
        {
            1 => union.Equals(_value1),
            2 => union.Equals(_value2),
            3 => union.Equals(_value3),
            _ => false
        };

    public override bool Equals([NotNullWhen(true)] object? value) =>
        Equals<object?>(value);

    public override int GetHashCode() =>
        _index switch
        {
            1 => _value1?.GetHashCode() ?? 0,
            2 => _value2?.GetHashCode() ?? 0,
            3 => _value3?.GetHashCode() ?? 0,
            _ => 0
        };
    #endregion

    #region Generic API
    public static OneOf<T1, T2, T3> Create<T>(T value) =>
        TryCreate(value, out var converted) 
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
            default:
                if (TypeUnionAccessor<T>.TryGetAccessor(out var accessor))
                {
                    if (accessor.TryGet<T1>(in value, out var t1val))
                    {
                        converted = Create(t1val);
                        return true;
                    }
                    else if (accessor.TryGet<T2>(in value, out var t2val))
                    {
                        converted = Create(t2val);
                        return true;
                    }
                    else if (accessor.TryGet<T3>(in value, out var t3val))
                    {
                        converted = Create(t3val);
                        return true;
                    }
                }
                break;
        }

        converted = default;
        return false;
    }

    public bool IsType<T>() =>
        _index switch
        {
            1 => typeof(T) == typeof(T1) || _value1 is T,
            2 => typeof(T) == typeof(T2) || _value2 is T,
            3 => typeof(T) == typeof(T3) || _value3 is T,
            _ => false
        };

    public T AsType<T>() =>
        _index switch
        {
            1 when _value1 is T t => t,
            2 when _value2 is T t => t,
            3 when _value3 is T t => t,
            _ => default!
        };

    public T Get<T>() =>
        _index switch
        {
            1 when _value1 is T t => t,
            2 when _value2 is T t => t,
            3 when _value3 is T t => t,
            _ => throw new InvalidCastException()
        };

    public bool TryGet<T>([NotNullWhen(true)] out T value)
    {
        switch (_index)
        {
            case 1 when _value1 is T t1:
                value = t1;
                return true;
            case 2 when _value2 is T t2:
                value = t2;
                return true;
            case 3 when _value3 is T t3:
                value = t3;
                return true;
            default:
                if (TypeUnionFactory<T>.TryGetFactory(out var factory))
                {
                    switch (_index)
                    {
                        case 1:
                            return factory.TryCreate(_value1, out value);
                        case 2:
                            return factory.TryCreate(_value2, out value);
                        case 3:
                            return factory.TryCreate(_value3, out value);
                    }
                }
                break;
        }

        value = default!;
        return false;
    }

    // equality
    public bool Equals<T>(T? value)
    {
        if (_index == 1 && _value1 is T t1)
        {
            return t1.Equals(value);
        }
        else if (_index == 2 && _value2 is T t2)
        {
            return t2.Equals(value);
        }
        else if (_index == 3 && _value3 is T t3)
        {
            return t3.Equals(value);
        }
        else if (value is ITypeUnion union)
        {
            if (_index == 1)
                return union.Equals(_value1);
            else if (_index == 2)
                return union.Equals(_value2);
            else if (_index == 3)
                return union.Equals(_value3);
        }

        return false;
    }
    #endregion

    // operators
    public static implicit operator OneOf<T1, T2, T3>(T1 value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3>(T2 value) => Create(value);
    public static implicit operator OneOf<T1, T2, T3>(T3 value) => Create(value);

    public static explicit operator T1(OneOf<T1, T2, T3> union) => union.GetType1();
    public static explicit operator T2(OneOf<T1, T2, T3> union) => union.GetType2();
    public static explicit operator T3(OneOf<T1, T2, T3> union) => union.GetType3();
}