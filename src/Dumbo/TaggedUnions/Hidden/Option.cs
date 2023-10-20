using System.Diagnostics.CodeAnalysis;

namespace Dumbo.TaggedUnions.Hidden
{
    public abstract record Option<TValue>()
    {
        private record SomeData(TValue value) : Option<TValue>;
        private record NoneData() : Option<TValue>;

        private static readonly NoneData NoneInstance = new NoneData();

        public static Option<TValue> Some(TValue value) => new SomeData(value);
        public static Option<TValue> None() => NoneInstance;

        public bool IsSome => this is SomeData;
        public bool IsNone => this is NoneData;

        public bool TryGetSome(out TValue value)
        {
            if (this is SomeData s)
            {
                value = s.value;
                return true;
            }
            value = default!;
            return false;
        }

        public static implicit operator Option<TValue>(TValue value) =>
            Some(value);
    }
}
