using System.Diagnostics.CodeAnalysis;

namespace Dumbo.TypeUnion.Fat
{
    // pre-existing types
    public record Cat(string name, int sleepingSpots);
    public record Dog(string name, bool isTrained);

    public readonly struct CatDogOrString : ITypeUnion<CatDogOrString>
    {
        private enum Kind { Type1 = 1, Type2, Type3 };

        private readonly Kind _kind;
        private readonly Cat _type1;
        private readonly Dog _type2;
        private readonly string _type3;

        private CatDogOrString(Kind kind, Cat type1, Dog type2, string type3)
        {
            _kind = kind;
            _type1 = type1;
            _type2 = type2;
            _type3 = type3;
        }

        public static CatDogOrString Create(Cat value) =>
            new CatDogOrString(Kind.Type1, value, default!, default!);

        public static CatDogOrString Create(Dog value) =>
            new CatDogOrString(Kind.Type2, default!, value, default!);

        public static CatDogOrString Create(string value) =>
            new CatDogOrString(Kind.Type3, default!, default!, value);

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

        public bool IsType<T>() =>
            _kind switch
            {
                Kind.Type1 => _type1 is T,
                Kind.Type2 => _type2 is T,
                Kind.Type3 => _type3 is T,
                _ => false
            };

        public bool TryGet<T>([NotNullWhen(true)] out T value)
        {
            if (_kind == Kind.Type1 && _type1 is T t1val)
            {
                value = t1val;
                return true;
            }
            else if (_kind == Kind.Type2 && _type2 is T t2val)
            {
                value = t2val;
                return true;
            }
            else if (_kind == Kind.Type3 && _type3 is T t3val)
            {
                value = t3val;
                return true;
            }
            value = default!;
            return false;
        }

        public override string ToString() =>
            _kind switch
            {
                Kind.Type1 => _type1.ToString(),
                Kind.Type2 => _type2.ToString(),
                Kind.Type3 => _type3.ToString(),
                _ => ""
            };

        public static implicit operator CatDogOrString(Cat value) => Create(value);
        public static implicit operator CatDogOrString(Dog value) => Create(value);
        public static implicit operator CatDogOrString(string value) => Create(value);
    }
}
