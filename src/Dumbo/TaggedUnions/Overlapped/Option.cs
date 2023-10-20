using System.Diagnostics.CodeAnalysis;

namespace Dumbo.TaggedUnions.Overlapped;

public readonly struct Option<TValue>
{
    private readonly TValue _value;
    private readonly bool _hasValue;

    private Option(TValue value, bool hasValue)
    {
        _value = value;
        _hasValue = hasValue;
    }

    public static Option<TValue> Some(TValue value) =>
        new Option<TValue>(value, true);

    public static Option<TValue> None = new Option<TValue>(default!, false);

    public bool IsSome => _hasValue;
    public bool IsNone => !_hasValue;

    public bool TryGetSome([NotNullWhen(true)] out TValue value)
    {
        if (_hasValue)
        {
            value = _value!;
            return true;
        }
        value = default!;
        return false;
    }

    public static implicit operator Option<TValue>(TValue value) =>
        Some(value);
}
