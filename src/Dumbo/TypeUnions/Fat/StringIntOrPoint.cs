using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Dumbo.TypeUnions.Fat;

public struct StringIntOrPoint : ITypeUnion<StringIntOrPoint>
{
    private enum Kind { Type1 = 1, Type2, Type3 }

    private readonly Kind _kind;
    private readonly string _value1;
    private readonly int _value2;
    private readonly Point _value3;

    private StringIntOrPoint(Kind kind, string value1, int value2, Point value3)
    {
        _kind = kind;
        _value1 = value1;
        _value2 = value2;
        _value3 = value3;
    }

    public static StringIntOrPoint Create(string value) =>
        new StringIntOrPoint(Kind.Type1, value, default, default);

    public static StringIntOrPoint Create(int value) =>
        new StringIntOrPoint(Kind.Type2, default!, value, default);

    public static StringIntOrPoint Create(Point value) =>
        new StringIntOrPoint(Kind.Type3, default!, default, value);

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
        _kind switch
        {
            Kind.Type1 => typeof(T) == typeof(int) || _value1 is T,
            Kind.Type2 => typeof(T) == typeof(string) || _value2 is T,
            Kind.Type3 => typeof(T) == typeof(Point) || _value3 is T,
            _ => false
        };

    public bool TryGet<T>([NotNullWhen(true)] out T value)
    {
        switch (_kind)
        {
            case Kind.Type1:
                if (_value1 is T t1val)
                {
                    value = t1val;
                    return true;
                }
                break;
            case Kind.Type2:
                if (_value2 is T t2val)
                {
                    value = t2val;
                    return true;
                }
                break;
            case Kind.Type3:
                if (_value3 is T t3val)
                {
                    value = t3val;
                    return true;
                }
                break;
        }

        if (TypeUnionFactory<T>.TryGetFactory(out var factory))
        {
            switch (_kind)
            {
                case Kind.Type1:
                    return factory.TryCreate(_value1, out value);
                case Kind.Type2:
                    return factory.TryCreate(_value2, out value);
                case Kind.Type3:
                    return factory.TryCreate(_value3, out value);
            }
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
        _kind switch
        {
            Kind.Type1 => _value1.ToString(),
            Kind.Type2 => _value2.ToString(),
            Kind.Type3 => _value3.ToString(),
            _ => ""
        };

    public Variant ToVariant() =>
        _kind switch
        {
            Kind.Type1 => Variant.Create(_value1),
            Kind.Type2 => Variant.Create(_value2),
            Kind.Type3 => Variant.Create(_value3),
            _ => Variant.Null
        };
}