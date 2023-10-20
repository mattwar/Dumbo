using System.Diagnostics.CodeAnalysis;

namespace Dumbo.TypeUnion.Boxed
{
    // pre-existing types
    public record Cat(string name, int sleepingSpots);
    public record Dog(string name, bool isTrained);

    public readonly struct CatDogOrString : ITypeUnion<CatDogOrString>
    {
        private readonly object _value;

        private CatDogOrString(object value)
        {
            _value = value;
        }

        public static CatDogOrString Create(Cat value) =>
            new CatDogOrString(value);

        public static CatDogOrString Create(Dog value) =>
            new CatDogOrString(value);

        public static CatDogOrString Create(string value) =>
            new CatDogOrString(value);

        public static bool TryCreate<T>(T value, [NotNullWhen(true)] out CatDogOrString union)
        {
            if (value is Cat type1)
            {
                union = Create(type1);
                return true;
            }
            else if (value is Dog type2)
            {
                union = Create(type2);
                return true;
            }
            else if (value is string type3)
            {
                union = Create(type3);
                return true;
            }
            else if (value is ITypeUnion other)
            {
                if (other.TryGet<Cat>(out var utype1))
                    return TryCreate(utype1, out union);
                else if (other.TryGet<Dog>(out var utype2))
                    return TryCreate(utype2, out union);
                else if (other.TryGet<string>(out var utype3))
                    return TryCreate(utype3, out union);
            }

            union = default;
            return false;
        }

        public static CatDogOrString Create<T>(T value) =>
            TryCreate(value, out var union)
                ? union
                : throw new InvalidCastException();

        public bool IsType<T>() => _value is T;

        public bool TryGet<T>([NotNullWhen(true)] out T value)
        {
            if (_value is T t)
            {
                value = t;
                return true;
            }
            value = default!;
            return false;
        }

        public override string ToString() =>
            _value?.ToString()!;

        public static implicit operator CatDogOrString(Cat value) => Create(value);
        public static implicit operator CatDogOrString(Dog value) => Create(value);
        public static implicit operator CatDogOrString(string value) => Create(value);
    }
}
