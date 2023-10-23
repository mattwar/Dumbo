using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Dumbo.TypeUnions.Boxed;

public struct StringIntOrPoint : ITypeUnion
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

    public bool IsType<T>() =>
        _value is T;

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
}