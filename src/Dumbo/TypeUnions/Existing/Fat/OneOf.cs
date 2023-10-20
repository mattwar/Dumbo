//#define ALLOW_NULL
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Dumbo.TypeUnions.Existing.Fat
{
    [DebuggerDisplay("{DebugText}")]
    public readonly struct OneOf<T1, T2> : ITypeUnion<OneOf<T1, T2>>
    {
        private readonly string DebugText => ToString();

        private readonly byte _index;
        private readonly T1 _value1;
        private readonly T2 _value2;

        private OneOf(byte index, T1 value1, T2 value2)
        {
            _index = index;
            _value1 = value1;
            _value2 = value2;
        }

        private int GetIndex(Type type) =>
              type == typeof(T1) ? 1
            : type == typeof(T2) ? 2
            : 0;

        public bool IsType<T>() =>
            GetIndex(typeof(T)) > 0;

        public T AsType<T>() =>
            _index switch
            {
                1 when _value1 is T t => t,
                2 when _value2 is T t => t,
                _ => default!
            };

        public T Get<T>() =>
            _index switch
            {
                1 when _value1 is T t => t,
                2 when _value2 is T t => t,
                _ => throw new InvalidCastException()
            };

        public bool TryGet<T>([NotNullWhen(true)] out T value)
        {
            if (IsType<T>())
            {
                value = Get<T>()!;
                return true;
            }
            value = default!;
            return false;
        }

        public Type? Type =>
            _index switch
            {
                1 => typeof(T1),
                2 => typeof(T2),
                _ => null
            };

        public static bool TryCreate<T>(T value, [NotNullWhen(true)] out OneOf<T1, T2> converted)
        {
            switch (value)
            {
                case T1 t1:
                    converted = new OneOf<T1, T2>(1, t1, default!);
                    return true;
                case T2 t2:
                    converted = new OneOf<T1, T2>(2, default!, t2);
                    return true;
                case ITypeUnion union:
                    if (union.TryGet<T1>(out var u1))
                        return TryCreate(u1, out converted);
                    else if (union.TryGet<T2>(out var u2))
                        return TryCreate(u2, out converted);
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

        public static OneOf<T1, T2> Create<T>(T value) =>
            TryCreate(value, out var converted) ? converted : throw new InvalidCastException();

        public override string ToString() =>
            _index switch
            {
                1 => _value1?.ToString()!,
                2 => _value2?.ToString()!,
                _ => null!
            };

        // conversion
        public static implicit operator OneOf<T1, T2>(T1 value) => Create(value);
        public static implicit operator OneOf<T1, T2>(T2 value) => Create(value);

        public static implicit operator OneOf<T1, T2>(OneOf<T2, T1> value) => Create(value);

        public static explicit operator T1(OneOf<T1, T2> value) => value.Get<T1>();
        public static explicit operator T2(OneOf<T1, T2> value) => value.Get<T2>();

        // equality
        public bool Equals<T>(T? value)
        {
            if (_index == 1 && _value1 is T t1)
            {
                return t1.Equals(value);
            }
            else if (_index == 2 && _value2 is T t2)
            {
                return t2.Equals(value);
            }
            else if (value is ITypeUnion union)
            {
                if (_index == 1)
                    return union.Equals(_value1);
                else if (_index == 2)
                    return union.Equals(_value2);
            }

            return false;
        }

        public override bool Equals([NotNullWhen(true)] object? obj) =>
            Equals(obj);

        public override int GetHashCode() =>
            _index switch
            {
                1 => _value1?.GetHashCode() ?? 0,
                2 => _value2?.GetHashCode() ?? 0,
                _ => 0
            };

        public static bool operator ==(OneOf<T1, T2> value, T1 other) => value.TryGet<T1>(out var tval) && tval.Equals(other);
        public static bool operator !=(OneOf<T1, T2> value, T1 other) => !value.TryGet<T1>(out var tval) || !tval.Equals(other);
        public static bool operator ==(T1 other, OneOf<T1, T2> value) => value.TryGet<T1>(out var tval) && tval.Equals(other);
        public static bool operator !=(T1 other, OneOf<T1, T2> value) => !value.TryGet<T1>(out var tval) || !tval.Equals(other);
        public static bool operator ==(OneOf<T1, T2> value, T2 other) => value.TryGet<T2>(out var tval) && tval.Equals(other);
        public static bool operator !=(OneOf<T1, T2> value, T2 other) => !value.TryGet<T2>(out var tval) || !tval.Equals(other);
        public static bool operator ==(T2 other, OneOf<T1, T2> value) => value.TryGet<T2>(out var tval) && tval.Equals(other);
        public static bool operator !=(T2 other, OneOf<T1, T2> value) => !value.TryGet<T2>(out var tval) || !tval.Equals(other);
    }
}
