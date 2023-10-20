using System.Diagnostics.CodeAnalysis;

namespace Dumbo.TaggedUnions.Boxed
{
    public readonly struct Option<TValue>
    {
        private enum Kind { None = 0, Some };

        private readonly Kind _kind;
        private readonly object? _value;

        private Option(Kind kind, object? value)
        {
            _kind = kind;
            _value = value;
        }

        public static Option<TValue> Some(TValue value) =>
            new Option<TValue>(Kind.Some, value);

        public static Option<TValue> None() => 
            new Option<TValue>(Kind.None, default);

        public bool IsSome => _kind == Kind.Some;
        public bool IsNone => _kind == Kind.None;

        public bool TryGetSome([NotNullWhen(true)] out TValue value)
        {
            if (IsSome && _value is TValue tval)
            {
                value = tval!;
                return true;
            }
            value = default!;
            return false;
        }

        public static implicit operator Option<TValue>(TValue value) =>
            Some(value);
    }
}
