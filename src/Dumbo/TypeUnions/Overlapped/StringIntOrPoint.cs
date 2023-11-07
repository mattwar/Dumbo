using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Dumbo.TypeUnions.Overlapped;

public struct StringIntOrPoint : ITypeUnion<StringIntOrPoint>
{
    private readonly int _index;
    private readonly RefData _refData;
    private readonly ValData _valData;

    [StructLayout(LayoutKind.Explicit)]
    private struct RefData
    {
        [FieldOffset(0)]
        public string _value1;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct ValData
    {
        [FieldOffset(0)]
        public int _value2;

        [FieldOffset(0)]
        public Point _value3;
    }

    private StringIntOrPoint(String value)
    {
        _index = 1;
        _refData._value1 = value;
    }

    private StringIntOrPoint(int value)
    {
        _index = 2;
        _valData._value2 = value;
    }

    private StringIntOrPoint(Point value)
    {
        _index = 3;
        _valData._value3 = value;
    }

    #region Non-Generic API
    public static StringIntOrPoint Create(string value) =>
        new StringIntOrPoint(value);

    public static StringIntOrPoint Create(int value) =>
        new StringIntOrPoint(value);

    public static StringIntOrPoint Create(Point value) =>
        new StringIntOrPoint(value);

    public int TypeIndex => _index;
    public bool IsType1 => _index == 1;
    public bool IsType2 => _index == 2;
    public bool IsType3 => _index == 3;

    public bool TryGetType1([NotNullWhen(true)] out string value)
    {
        if (IsType1)
        {
            value = _refData._value1;
            return true;
        }

        value = default!;
        return false;
    }

    public bool TryGetType2([NotNullWhen(true)] out int value)
    {
        if (IsType2)
        {
            value = _valData._value2;
            return true;
        }

        value = default!;
        return false;
    }

    public bool TryGetType3([NotNullWhen(true)] out Point value)
    {
        if (IsType3)
        {
            value = _valData._value3;
            return true;
        }

        value = default!;
        return false;
    }

    public string GetType1() =>
        TryGetType1(out var value)
            ? value
            : throw new InvalidCastException();

    public int GetType2() =>
        TryGetType2(out var value)
            ? value
            : throw new InvalidCastException();

    public Point GetType3() =>
        TryGetType3(out var value)
            ? value
            : throw new InvalidCastException();

    public string AsType1() =>
        TryGetType1(out var value)
            ? value
            : default!;

    public int AsType2() =>
        TryGetType2(out var value)
            ? value
            : default!;

    public Point AsType3() =>
        TryGetType3(out var value)
            ? value
            : default!;

    public override string ToString() =>
        _index switch
        {
            1 => _refData._value1.ToString(),
            2 => _valData._value2.ToString(),
            3 => _valData._value3.ToString(),
            _ => ""
        };

    public Variant ToVariant() =>
        _index switch
        {
            1 => Variant.Create(_refData._value1),
            2 => Variant.Create(_valData._value2),
            3 => Variant.Create(_valData._value3),
            _ => Variant.Null
        };

    #endregion

    #region Generic API

    public static StringIntOrPoint Create<T>(T value) =>
        TryCreate<T>(value, out var union)
            ? union
            : throw new InvalidCastException();

    public static bool TryCreate<T>(T value, [NotNullWhen(true)] out StringIntOrPoint union)
    {
        switch (value)
        {
            case string sval:
                union = Create(sval);
                return true;
            case int ival:
                union = Create(ival);
                return true;
            case Point pval:
                union = Create(pval);
                return true;
            default:
                if (TypeUnionAccessor<T>.TryGetAccessor(out var accessor))
                {
                    if (accessor.TryGet<string>(in value, out var usval))
                    {
                        union = Create(usval);
                        return true;
                    }
                    else if (accessor.TryGet<int>(in value, out var uival))
                    {
                        union = Create(uival);
                        return true;
                    }
                    else if (accessor.TryGet<Point>(in value, out var upval))
                    {
                        union = Create(upval);
                        return true;
                    }
                }
                break;
        }

        union = default;
        return false;
    }

    public bool IsType<T>() =>
        _index switch
        {
            1 => _refData._value1 is T,
            2 => _valData._value2 is T,
            3 => _valData._value3 is T,
            _ => false
        };

    public bool TryGet<T>([NotNullWhen(true)] out T value)
    {
        switch (_index)
        {
            case 1:
                if (_refData._value1 is T t1val)
                {
                    value = t1val;
                    return true;
                }
                break;
            case 2:
                if (_valData._value2 is T t2val)
                {
                    value = t2val;
                    return true;
                }
                break;
            case 3:
                if (_valData._value3 is T t3val)
                {
                    value = t3val;
                    return true;
                }
                break;
        }

        if (TypeUnionFactory<T>.TryGetFactory(out var factory))
        {
            switch (_index)
            {
                case 1:
                    return factory.TryCreate(_refData._value1, out value);
                case 2:
                    return factory.TryCreate(_valData._value2, out value);
                case 3:
                    return factory.TryCreate(_valData._value3, out value);
            }
        }

        value = default!;
        return false;
    }

    public T Get<T>() =>
        TryGet<T>(out var value)
            ? value
            : throw new InvalidCastException();

    #endregion

    public static implicit operator StringIntOrPoint(string value) => Create(value);
    public static implicit operator StringIntOrPoint(int value) => Create(value);
    public static implicit operator StringIntOrPoint(Point value) => Create(value);

    public static explicit operator string(StringIntOrPoint value) => value.GetType1();
    public static explicit operator int(StringIntOrPoint value) => value.GetType2();
    public static explicit operator Point(StringIntOrPoint value) => value.GetType3();
}