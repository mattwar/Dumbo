//#define ALLOW_NULL
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Dumbo
{
    [DebuggerDisplay("{DebugText}")]
    public readonly struct OneOf<T1, T2> : ITypeUnion<OneOf<T1, T2>>
    {
        private readonly string DebugText => ToString();

        private readonly Variant _value;

        private OneOf(Variant value)
        {
            _value = value;
        }

        public bool IsType<T>() => _value.IsType<T>();
        public T AsType<T>() => _value.AsType<T>();
        public T Get<T>() => _value.Get<T>();
        public bool TryGet<T>([NotNullWhen(true)] out T value) => _value.TryGet<T>(out value);
        public Type? Type => _value.Type;

        public static bool TryConvertFrom<T>(T value, [NotNullWhen(true)] out OneOf<T1, T2> converted)
        {
            switch (value)
            {
                case T1 t1:
                    converted = new OneOf<T1, T2>(Variant.Create(t1));
                    return true;
                case T2 t2:
                    converted = new OneOf<T1, T2>(Variant.Create(t2));
                    return true;
                case ITypeUnion union:
                    if (union.TryGet<T1>(out var u1))
                        return TryConvertFrom(u1, out converted);
                    else if (union.TryGet<T2>(out var u2))
                        return TryConvertFrom(u2, out converted);
                    break;
#if ALLOW_NULL
                case null:
                    converted = default;
                    return true;
#endif
            }

            converted = default;
            return false;
        }

        public static OneOf<T1, T2> ConvertFrom<T>(T value) =>
            TryConvertFrom(value, out var converted) ? converted : throw new InvalidCastException();

        public override string ToString() => _value.ToString();

        // conversion
        public static implicit operator OneOf<T1, T2>(T1 value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2>(T2 value) => ConvertFrom(value);

        public static implicit operator OneOf<T1, T2>(OneOf<T2, T1> value) => ConvertFrom(value);

        public static explicit operator T1(OneOf<T1, T2> value) => value.Get<T1>();
        public static explicit operator T2(OneOf<T1, T2> value) => value.Get<T2>();

        public bool Equals<T>(T value) =>
            TryConvertFrom(value, out var converted)
                && _value.Equals(converted._value);

        public override bool Equals([NotNullWhen(true)] object? obj) =>
            Equals<object?>(obj);

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(OneOf<T1, T2> value, T1 other) => value.TryGet<T1>(out var tval) && tval.Equals(other);
        public static bool operator !=(OneOf<T1, T2> value, T1 other) => !value.TryGet<T1>(out var tval) || !tval.Equals(other);
        public static bool operator ==(T1 other, OneOf<T1, T2> value) => value.TryGet<T1>(out var tval) && tval.Equals(other);
        public static bool operator !=(T1 other, OneOf<T1, T2> value) => !value.TryGet<T1>(out var tval) || !tval.Equals(other);
        public static bool operator ==(OneOf<T1, T2> value, T2 other) => value.TryGet<T2>(out var tval) && tval.Equals(other);
        public static bool operator !=(OneOf<T1, T2> value, T2 other) => !value.TryGet<T2>(out var tval) || !tval.Equals(other);
        public static bool operator ==(T2 other, OneOf<T1, T2> value) => value.TryGet<T2>(out var tval) && tval.Equals(other);
        public static bool operator !=(T2 other, OneOf<T1, T2> value) => !value.TryGet<T2>(out var tval) || !tval.Equals(other);
    }

    [DebuggerDisplay("{DebugText}")]
    public readonly struct OneOf<T1, T2, T3> : ITypeUnion<OneOf<T1, T2, T3>>
    {
        private readonly string DebugText => ToString();

        private readonly Variant _value;

        private OneOf(Variant value)
        {
            _value = value;
        }

        public bool IsType<T>() => _value.IsType<T>();
        public T AsType<T>() => _value.AsType<T>();
        public T Get<T>() => _value.Get<T>();
        public bool TryGet<T>([NotNullWhen(true)] out T value) => _value.TryGet<T>(out value);
        public Type? Type => _value.Type;

        public static bool TryConvertFrom<T>(T value, [NotNullWhen(true)] out OneOf<T1, T2, T3> converted)
        {
            switch (value)
            {
                case T1 t1:
                    converted = new OneOf<T1, T2, T3>(Variant.Create(t1));
                    return true;
                case T2 t2:
                    converted = new OneOf<T1, T2, T3>(Variant.Create(t2));
                    return true;
                case T3 t3:
                    converted = new OneOf<T1, T2, T3>(Variant.Create(t3));
                    return true;
                case ITypeUnion union:
                    if (union.TryGet<T1>(out var u1))
                        return TryConvertFrom(u1, out converted);
                    else if (union.TryGet<T2>(out var u2))
                        return TryConvertFrom(u2, out converted);
                    else if (union.TryGet<T3>(out var u3))
                        return TryConvertFrom(u3, out converted);
                    break;
#if ALLOW_NULL
                case null:
                    converted = default;
                    return true;
#endif
            }

            converted = default;
            return false;
        }

        public static OneOf<T1, T2, T3> ConvertFrom<T>(T other) =>
            TryConvertFrom(other, out var converted) ? converted : throw new InvalidCastException();

        public override string ToString() => _value.ToString();

        // conversion
        public static implicit operator OneOf<T1, T2, T3>(T1 value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3>(T2 value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3>(T3 value) => ConvertFrom(value);

        public static implicit operator OneOf<T1, T2, T3>(OneOf<T1, T2> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3>(OneOf<T2, T1> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3>(OneOf<T1, T3> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3>(OneOf<T3, T1> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3>(OneOf<T2, T3> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3>(OneOf<T3, T2> value) => ConvertFrom(value);

        public static implicit operator OneOf<T1, T2, T3>(OneOf<T1, T3, T2> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3>(OneOf<T2, T1, T3> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3>(OneOf<T2, T3, T1> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3>(OneOf<T3, T1, T2> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3>(OneOf<T3, T2, T1> value) => ConvertFrom(value);

        public static explicit operator T1(OneOf<T1, T2, T3> value) => value.Get<T1>();
        public static explicit operator T2(OneOf<T1, T2, T3> value) => value.Get<T2>();
        public static explicit operator T3(OneOf<T1, T2, T3> value) => value.Get<T3>();

        // equality
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            TryConvertFrom(obj, out var converted)
                && _value.Equals(converted._value);

        public bool Equals<T>(T value) =>
            TryConvertFrom(value, out var converted)
                && _value.Equals(converted._value);

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(OneOf<T1, T2, T3> value, T1 other) => value.TryGet<T1>(out var tval) && tval.Equals(other);
        public static bool operator !=(OneOf<T1, T2, T3> value, T1 other) => !value.TryGet<T1>(out var tval) || !tval.Equals(other);
        public static bool operator ==(T1 other, OneOf<T1, T2, T3> value) => value.TryGet<T1>(out var tval) && tval.Equals(other);
        public static bool operator !=(T1 other, OneOf<T1, T2, T3> value) => !value.TryGet<T1>(out var tval) || !tval.Equals(other);
        public static bool operator ==(OneOf<T1, T2, T3> value, T2 other) => value.TryGet<T2>(out var tval) && tval.Equals(other);
        public static bool operator !=(OneOf<T1, T2, T3> value, T2 other) => !value.TryGet<T2>(out var tval) || !tval.Equals(other);
        public static bool operator ==(T2 other, OneOf<T1, T2, T3> value) => value.TryGet<T2>(out var tval) && tval.Equals(other);
        public static bool operator !=(T2 other, OneOf<T1, T2, T3> value) => !value.TryGet<T2>(out var tval) || !tval.Equals(other);
        public static bool operator ==(OneOf<T1, T2, T3> value, T3 other) => value.TryGet<T3>(out var tval) && tval.Equals(other);
        public static bool operator !=(OneOf<T1, T2, T3> value, T3 other) => !value.TryGet<T3>(out var tval) || !tval.Equals(other);
        public static bool operator ==(T3 other, OneOf<T1, T2, T3> value) => value.TryGet<T3>(out var tval) && tval.Equals(other);
        public static bool operator !=(T3 other, OneOf<T1, T2, T3> value) => !value.TryGet<T3>(out var tval) || !tval.Equals(other);
    }

    [DebuggerDisplay("{DebugText}")]
    public readonly struct OneOf<T1, T2, T3, T4> : ITypeUnion<OneOf<T1, T2, T3, T4>>
    {
        private readonly string DebugText => ToString();

        private readonly Variant _value;

        private OneOf(Variant value)
        {
            _value = value;
        }

        public bool IsType<T>() => _value.IsType<T>();
        public T AsType<T>() => _value.AsType<T>();
        public T Get<T>() => _value.Get<T>();
        public bool TryGet<T>([NotNullWhen(true)] out T value) => _value.TryGet<T>(out value);
        public Type? Type => _value.Type;

        public static bool TryConvertFrom<T>(T value, [NotNullWhen(true)] out OneOf<T1, T2, T3, T4> converted)
        {
            switch (value)
            {
                case T1 t1:
                    converted = new OneOf<T1, T2, T3, T4>(Variant.Create(t1));
                    return true;
                case T2 t2:
                    converted = new OneOf<T1, T2, T3, T4>(Variant.Create(t2));
                    return true;
                case T3 t3:
                    converted = new OneOf<T1, T2, T3, T4>(Variant.Create(t3));
                    return true;
                case T4 t4:
                    converted = new OneOf<T1, T2, T3, T4>(Variant.Create(t4));
                    return true;
                case ITypeUnion union:
                    if (union.TryGet<T1>(out var u1))
                        return TryConvertFrom(u1, out converted);
                    else if (union.TryGet<T2>(out var u2))
                        return TryConvertFrom(u2, out converted);
                    else if (union.TryGet<T3>(out var u3))
                        return TryConvertFrom(u3, out converted);
                    else if (union.TryGet<T4>(out var u4))
                        return TryConvertFrom(u4, out converted);
                    break;
#if ALLOW_NULL
                case null:
                    converted = default;
                    return true;
#endif
            }

            converted = default;
            return false;
        }

        public static OneOf<T1, T2, T3, T4> ConvertFrom<T>(T other) =>
            TryConvertFrom(other, out var converted) ? converted : throw new InvalidCastException();

        public override string ToString() => _value.ToString();

        // conversion
        public static implicit operator OneOf<T1, T2, T3, T4>(T1 value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(T2 value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(T3 value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(T4 value) => ConvertFrom(value);

        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T2> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T3> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T4> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T1> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T3> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T4> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T1> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T2> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T4> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T1> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T2> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T3> value) => ConvertFrom(value);

        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T2, T3> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T2, T4> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T3, T2> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T3, T4> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T4, T2> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T4, T3> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T1, T3> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T1, T4> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T3, T1> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T3, T4> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T4, T1> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T4, T3> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T1, T2> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T1, T4> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T2, T1> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T2, T4> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T4, T1> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T4, T2> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T1, T2> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T1, T3> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T2, T1> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T2, T3> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T3, T1> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T3, T2> value) => ConvertFrom(value);

        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T2, T4, T3> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T3, T2, T4> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T3, T4, T2> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T4, T2, T3> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T1, T4, T3, T2> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T1, T3, T4> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T1, T4, T3> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T3, T1, T4> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T3, T4, T1> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T4, T1, T3> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T2, T4, T3, T1> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T1, T2, T4> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T1, T4, T2> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T2, T1, T4> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T2, T4, T1> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T4, T1, T2> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T3, T4, T2, T1> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T1, T2, T3> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T1, T3, T2> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T2, T1, T3> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T2, T3, T1> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T3, T1, T2> value) => ConvertFrom(value);
        public static implicit operator OneOf<T1, T2, T3, T4>(OneOf<T4, T3, T2, T1> value) => ConvertFrom(value);

        public static explicit operator T1(OneOf<T1, T2, T3, T4> value) => value.Get<T1>();
        public static explicit operator T2(OneOf<T1, T2, T3, T4> value) => value.Get<T2>();
        public static explicit operator T3(OneOf<T1, T2, T3, T4> value) => value.Get<T3>();
        public static explicit operator T4(OneOf<T1, T2, T3, T4> value) => value.Get<T4>();

        // equality
        public override bool Equals([NotNullWhen(true)] object? obj) =>
            TryConvertFrom(obj, out var converted)
                && _value.Equals(converted._value);

        public bool Equals<T>(T value) =>
            TryConvertFrom(value, out var converted)
                && _value.Equals(converted._value);

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(OneOf<T1, T2, T3, T4> value, T1 other) => value.TryGet<T1>(out var tval) && tval.Equals(other);
        public static bool operator !=(OneOf<T1, T2, T3, T4> value, T1 other) => !value.TryGet<T1>(out var tval) || !tval.Equals(other);
        public static bool operator ==(T1 other, OneOf<T1, T2, T3, T4> value) => value.TryGet<T1>(out var tval) && tval.Equals(other);
        public static bool operator !=(T1 other, OneOf<T1, T2, T3, T4> value) => !value.TryGet<T1>(out var tval) || !tval.Equals(other);
        public static bool operator ==(OneOf<T1, T2, T3, T4> value, T2 other) => value.TryGet<T2>(out var tval) && tval.Equals(other);
        public static bool operator !=(OneOf<T1, T2, T3, T4> value, T2 other) => !value.TryGet<T2>(out var tval) || !tval.Equals(other);
        public static bool operator ==(T2 other, OneOf<T1, T2, T3, T4> value) => value.TryGet<T2>(out var tval) && tval.Equals(other);
        public static bool operator !=(T2 other, OneOf<T1, T2, T3, T4> value) => !value.TryGet<T2>(out var tval) || !tval.Equals(other);
        public static bool operator ==(OneOf<T1, T2, T3, T4> value, T3 other) => value.TryGet<T3>(out var tval) && tval.Equals(other);
        public static bool operator !=(OneOf<T1, T2, T3, T4> value, T3 other) => !value.TryGet<T3>(out var tval) || !tval.Equals(other);
        public static bool operator ==(T3 other, OneOf<T1, T2, T3, T4> value) => value.TryGet<T3>(out var tval) && tval.Equals(other);
        public static bool operator !=(T3 other, OneOf<T1, T2, T3, T4> value) => !value.TryGet<T3>(out var tval) || !tval.Equals(other);
        public static bool operator ==(OneOf<T1, T2, T3, T4> value, T4 other) => value.TryGet<T4>(out var tval) && tval.Equals(other);
        public static bool operator !=(OneOf<T1, T2, T3, T4> value, T4 other) => !value.TryGet<T4>(out var tval) || !tval.Equals(other);
        public static bool operator ==(T4 other, OneOf<T1, T2, T3, T4> value) => value.TryGet<T4>(out var tval) && tval.Equals(other);
        public static bool operator !=(T4 other, OneOf<T1, T2, T3, T4> value) => !value.TryGet<T4>(out var tval) || !tval.Equals(other);
    }
}
