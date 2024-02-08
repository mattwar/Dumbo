using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Dumbo.TypeUnions.Hybrid;

public struct StringIntOrPoint : ITypeUnion<StringIntOrPoint>
{
    private readonly Variant _value;

    private StringIntOrPoint(Variant value)
    {
        _value = value;
    }

    public static StringIntOrPoint Create(string value) =>
        new StringIntOrPoint(value);

    public static StringIntOrPoint Create(int value) =>
        new StringIntOrPoint(value);

    public static StringIntOrPoint Create(Point value) =>
        new StringIntOrPoint(Variant.Create(value));

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

    public bool IsString =>
        _value.IsString;

    public bool IsInt =>
        _value.IsInt32;

    public bool IsPoint =>
        _value.IsType<Point>();

    public bool TryGet([NotNullWhen(true)] out string? value) =>
        _value.TryGet(out value);

    public bool TryGet(out int value) =>
        _value.TryGet(out value);

    public bool TryGet(out Point value) =>
        _value.TryGet(out value);

    public string GetString() =>
        _value.GetString();

    public int GetInt() =>
        _value.GetInt32();

    public Point GetPoint() =>
        _value.Get<Point>();

    public bool IsType<T>() =>
        _value.IsType<T>();

    public bool TryGet<T>([NotNullWhen(true)] out T value)
    {
        if (_value.TryGet(out value))
        {
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
        _value.ToString();

    public Variant ToVariant() =>
        _value;
}