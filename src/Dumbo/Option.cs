using System.Diagnostics.CodeAnalysis;

namespace Dumbo
{
    public record Some<TValue>(TValue value);
    public record None() { public static readonly None Instance = new None(); }

    public struct Option<TValue> : ITypeUnion<Option<TValue>>
    {
        private object __value;

        private Option(object value)
        {
            __value = value;
        }

        public static Option<TValue> None = new Option<TValue>(Dumbo.None.Instance);

        public static implicit operator Option<TValue>(Some<TValue> s) => new Option<TValue>(s);
        public static implicit operator Option<TValue>(TValue t) => new Option<TValue>(new Some<TValue>(t));
        public static implicit operator Option<TValue>(None n) => None;

        public static bool TryCreate<TOther>(TOther other, [NotNullWhen(true)] out Option<TValue> value)
        {
            switch (other)
            {
                case TValue v:
                    value = new Option<TValue>(new Some<TValue>(v));
                    return true;
                case Some<TValue> s:
                    value = new Option<TValue>(s);
                    return true;
                case None n:
                    value = None;
                    return true;
                case ITypeUnion u:
                    if (u.TryGet<TValue>(out var t))
                        return TryCreate(t, out value);
                    else if (u.TryGet<Some<TValue>>(out var st))
                        return TryCreate(st, out value);
                    else if (u.TryGet<None>(out var nt))
                        return TryCreate(nt, out value);
                    break;
            }

            value = default!;
            return false;
        }
        public static Option<TValue> Create<TOther>(TOther other) =>
            TryCreate(other, out var value) ? value : throw new InvalidCastException();

        public bool IsSome => __value is Some<TValue>;
        public bool IsNone => __value is None || __value is null;

        public bool TryGetSome([NotNullWhen(true)] out TValue value)
        {
            if (__value is Some<TValue> s)
            {
                value = s.value!;
                return true;
            }
            value = default!;
            return false;
        }

        public Some<TValue> GetSome() => (Some<TValue>)__value;

        #region ITypeUnion
        static bool ITypeUnion<Option<TValue>>.TryConvertFrom<T1>(T1 value, out Option<TValue> converted) =>
            TryCreate(value, out converted);

        static Option<TValue> ITypeUnion<Option<TValue>>.ConvertFrom<T1>(T1 value) =>
            Create(value);

        bool ITypeUnion.IsType<T>() =>
            __value is T;

        bool ITypeUnion.TryGet<T>([NotNullWhen(true)] out T value)
        {
            if (__value is T tval)
            {
                value = tval;
                return true;
            }
            value = default!;
            return false;
        }
        #endregion
    }
}
