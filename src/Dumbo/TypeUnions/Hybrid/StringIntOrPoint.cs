using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Dumbo.TypeUnions.Hybrid;

public struct StringIntOrPoint : ITypeUnion
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

    public bool IsType<T>() =>
        _value.IsType<T>();

    public bool TryGet<T>([NotNullWhen(true)] out T value) =>
        _value.TryGet<T>(out value);
}