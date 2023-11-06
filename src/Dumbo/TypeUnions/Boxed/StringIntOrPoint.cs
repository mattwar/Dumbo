using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Dumbo.TypeUnions.Boxed;

public struct StringIntOrPoint : ITypeUnion<StringIntOrPoint>
{
    private readonly object _value;

    private StringIntOrPoint(object value)
    {
        _value = value;
    }

    public static StringIntOrPoint Create(string value) =>
        new StringIntOrPoint(value);

    public static StringIntOrPoint Create(int value) =>
        new StringIntOrPoint(value);

    public static StringIntOrPoint Create(Point value) =>
        new StringIntOrPoint(value);

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
        _value is T;

    public bool TryGet<T>([NotNullWhen(true)] out T value)
    {
        if (_value is T t)
        {
            value = t;
            return true;
        }
        else if (TypeUnionFactory<T>.TryGetFactory(out var factory))
        {
            return factory.TryCreate(_value, out value);
        }

        value = default!;
        return false;
    }

    public T Get<T>() =>
        TryGet<T>(out var value)
            ? value
            : throw new InvalidCastException();

    public static implicit operator StringIntOrPoint(string value) => Create(value);
    public static implicit operator StringIntOrPoint(int value) => Create(value);
    public static implicit operator StringIntOrPoint(Point value) => Create(value);

    public static explicit operator string(StringIntOrPoint value) => value.Get<string>();
    public static explicit operator int(StringIntOrPoint value) => value.Get<int>();
    public static explicit operator Point(StringIntOrPoint value) => value.Get<Point>();

    public override string ToString() =>
        _value?.ToString() ?? "";

    public Variant ToVariant() =>
        Variant.Create(_value);
}