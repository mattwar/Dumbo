using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Dumbo.TypeUnion.Overlapped
{
    public record struct Cat(string name, int sleepingSpots);
    public record struct Dog(string name, bool isTrained);

    public readonly struct CatDogOrString : ITypeUnion<CatDogOrString>
    {
        private enum Kind { Type1 = 1, Type2, Type3 };

        public struct Type1Vals
        {
            public int sleepingSpots;
        }

        public struct Type1Refs
        {
            public string name;
        }

        public struct Type2Vals
        {
            public bool isTrained;
        }

        public struct Type2Refs
        {
            public string name;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct OverlappedRefs
        {
            [FieldOffset(0)]
            public Type1Refs type1;

            [FieldOffset(0)]
            public Type2Refs type2;

            [FieldOffset(0)]
            public string type3;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct OverlappedVals
        {
            [FieldOffset(0)]
            public Type1Vals type1;

            [FieldOffset(0)]
            public Type2Vals type2;
        }

        private readonly Kind _kind;
        private readonly OverlappedRefs _refs;
        private readonly OverlappedVals _vals;

        private CatDogOrString(Kind kind, OverlappedRefs refs, OverlappedVals vals)
        {
            _kind = kind;
            _refs = refs;
            _vals = vals;
        }

        public static CatDogOrString Create(Cat cat) =>
            new CatDogOrString(Kind.Type1, new OverlappedRefs { type1 = new Type1Refs { name = cat.name } }, new OverlappedVals { type1 = new Type1Vals { sleepingSpots = cat.sleepingSpots } });

        public static CatDogOrString Create(Dog dog) =>
            new CatDogOrString(Kind.Type2, new OverlappedRefs { type2 = new Type2Refs { name = dog.name } }, new OverlappedVals { type2 = new Type2Vals { isTrained = dog.isTrained } });

        public static CatDogOrString Create(string value) =>
            new CatDogOrString(Kind.Type3, new OverlappedRefs { type3 = value }, default);

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


        public bool TryGetType1(out Cat cat)
        {
            if (_kind == Kind.Type1)
            {
                cat = new Cat(_refs.type1.name, _vals.type1.sleepingSpots);
                return true;
            }

            cat = default;
            return false;
        }

        private bool TryGetType2(out Dog dog)
        {
            if (_kind == Kind.Type2)
            {
                dog = new Dog(_refs.type2.name, _vals.type2.isTrained);
                return true;
            }

            dog = default;
            return false;
        }

        private bool TryGetType3(out string value)
        {
            if (_kind == Kind.Type3)
            {
                value = _refs.type3;
                return true;
            }

            value = default!;
            return false;
        }

        private Cat GetType1() =>
            TryGetType1(out var type1)
                ? type1
                : throw new InvalidCastException();

        private Dog GetType2() =>
            TryGetType2(out var type2)
                ? type2
                : throw new InvalidCastException();

        private string GetType3() =>
            TryGetType3(out var type3)
                ? type3
                : throw new InvalidCastException();

        public bool IsType<T>()
        {
            switch (_kind)
            {
                case Kind.Type1:
                    return GetType1() is T;
                case Kind.Type2:
                    return GetType2() is T;
                case Kind.Type3:
                    return GetType3() is T;
                default:
                    return false;
            }
        }

        public bool TryGet<T>([NotNullWhen(true)] out T value)
        {
            switch (_kind)
            {
                case Kind.Type1 when GetType1() is T type1:
                    value = type1;
                    return true;
                case Kind.Type2 when GetType2() is T type2:
                    value = type2;
                    return true;
                case Kind.Type3 when GetType3() is T type3:
                    value = type3;
                    return true;
            }
            value = default!;
            return false;
        }

        public override string ToString() =>
            _kind switch
            {
                Kind.Type1 => GetType1().ToString(),
                Kind.Type2 => GetType2().ToString(),
                Kind.Type3 => GetType3().ToString(),
                _ => ""
            };

        public static implicit operator CatDogOrString(Cat value) => Create(value);
        public static implicit operator CatDogOrString(Dog value) => Create(value);
        public static implicit operator CatDogOrString(string value) => Create(value);
    }
}
