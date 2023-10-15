using System.Diagnostics.CodeAnalysis;

namespace Dumbo
{
    public record Some<T>(T value);
    public record None() { public static readonly None Instance = new None(); }

    public struct Option<T> : ITypeUnion<Option<T>>
    {
        private object __value;

        private Option(object value)
        {
            __value = value;
        }

        public static Option<T> None = new Option<T>(Dumbo.None.Instance);

        public static implicit operator Option<T>(Some<T> s) => new Option<T>(s);
        public static implicit operator Option<T>(T t) => new Option<T>(new Some<T>(t));
        public static implicit operator Option<T>(None n) => None;

        public static bool TryCreate<TOther>(TOther other, [NotNullWhen(true)] out Option<T> value)
        {
            switch (other)
            {
                case T v:
                    value = new Option<T>(new Some<T>(v));
                    return true;
                case Some<T> s:
                    value = new Option<T>(s);
                    return true;
                case None n:
                    value = None;
                    return true;
                case ITypeUnion u:
                    if (u.TryGet<T>(out var t))
                        return TryCreate(t, out value);
                    else if (u.TryGet<Some<T>>(out var st))
                        return TryCreate(st, out value);
                    else if (u.TryGet<None>(out var nt))
                        return TryCreate(nt, out value);
                    break;
            }

            value = default!;
            return false;
        }
        public static Option<T> Create<TOther>(TOther other) =>
            TryCreate(other, out var value) ? value : throw new InvalidCastException();

        public bool IsSome => __value is Some<T>;
        public bool IsNone => __value is None || __value is null;

        public bool TryGetSome([NotNullWhen(true)] out T value)
        {
            if (__value is Some<T> s)
            {
                value = s.value!;
                return true;
            }
            value = default!;
            return false;
        }

        public Some<T> GetSome() => (Some<T>)__value;

        static bool ITypeUnion<Option<T>>.TryConvertFrom<T1>(T1 value, out Option<T> converted) =>
            TryCreate(value, out converted);

        static Option<T> ITypeUnion<Option<T>>.ConvertFrom<T1>(T1 value) =>
            Create(value);

        bool ITypeUnion.IsType<TValue>() =>
            __value is TValue;

        bool ITypeUnion.TryGet<TValue>([NotNullWhen(true)] out TValue value)
        {
            if (__value is TValue tval)
            {
                value = tval;
                return true;
            }
            value = default!;
            return false;
        }
    }
}
