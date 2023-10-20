using System.Diagnostics.CodeAnalysis;

namespace Dumbo.TaggedUnions.Hybrid
{
    public readonly struct Option<TValue>
    {
        private readonly Variant _value;

        private Option(Variant value)
        {
            _value = value;
        }

        public static Option<TValue> Some(TValue value) =>
            new Option<TValue>(Variant.Create(value));

        public static Option<TValue> None() =>
            new Option<TValue>(default);

        public bool IsSome => _value is TValue;
        public bool IsNone => _value.IsNull;

        public bool TryGetSome([NotNullWhen(true)] out TValue value)
        {
            if (_value.TryGet<TValue>(out var tval))
            {
                value = tval;
                return true;
            }
            value = default!;
            return false;
        }

        public static implicit operator Option<TValue>(TValue value) =>
            Some(value);
    }
}
