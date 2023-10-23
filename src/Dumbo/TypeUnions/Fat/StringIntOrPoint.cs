using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Dumbo.TypeUnions.Fat;

public struct StringIntOrPoint : ITypeUnion
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
        value = default!;
        return false;
    }
}