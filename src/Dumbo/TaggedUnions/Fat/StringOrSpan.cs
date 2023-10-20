using System.Diagnostics.CodeAnalysis;

namespace Dumbo.TaggedUnions.Fat;

public readonly ref struct StringOrSpan
{
    private enum Kind { String = 1, Span };

    private readonly Kind _kind;
    private readonly string _valueString;
    private readonly Span<char> _valueSpan;

    private StringOrSpan(Kind kind, string valueString, Span<char> valueSpan)
    {
        _kind = kind;
        _valueString = valueString;
        _valueSpan = valueSpan;
    }

    public static StringOrSpan Create(string value) =>
        new StringOrSpan(Kind.String, value, default);

    public static StringOrSpan Create(Span<char> value) =>
        new StringOrSpan(Kind.Span, default!, value);

    public bool IsString => _kind == Kind.String;
    public bool IsSpan => _kind == Kind.Span;

    public bool TryGetString([NotNullWhen(true)] out string value)
    {
        if (IsString)
        {
            value = _valueString;
            return true;
        }
        value = default!;
        return false;
    }

    public bool TryGetSpan([NotNullWhen(true)] out Span<char> value)
    {
        if (IsSpan)
        {
            value = _valueSpan;
            return true;
        }
        value = default!;
        return false;
    }

    public string GetString() =>
        TryGetString(out var stringVal)
            ? stringVal
            : throw new InvalidCastException();

    public Span<char> GetSpan() =>
        TryGetSpan(out var spanVal)
            ? spanVal
            : throw new InvalidCastException();

    public static implicit operator StringOrSpan(string value) =>
        Create(value);

    public static implicit operator StringOrSpan(Span<char> value) =>
        Create(value);

    public static explicit operator string(StringOrSpan value) =>
        value.GetString();

    public static explicit operator Span<char>(StringOrSpan value) =>
        value.GetSpan();
}
