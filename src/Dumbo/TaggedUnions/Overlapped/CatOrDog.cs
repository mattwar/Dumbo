using System.Runtime.InteropServices;

namespace Dumbo.TaggedUnions.Overlapped;

public readonly struct CatOrDog
{
    private enum Kind { Cat, Dog };

    public struct CatVals
    {
        public int sleepingSpots;
    }

    public struct CatRefs
    {
        public string name;
    }

    public struct DogVals
    {
        public bool isTrained;
    }

    public struct DogRefs
    {
        public string name;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct OverlappedRefs
    {
        [FieldOffset(0)]
        public CatRefs cat;

        [FieldOffset(0)]
        public DogRefs dog;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct OverlappedVals
    {
        [FieldOffset(0)]
        public CatVals cat;

        [FieldOffset(0)]
        public DogVals dog;
    }

    private readonly Kind _kind;
    private readonly OverlappedRefs _refs;
    private readonly OverlappedVals _vals;

    private CatOrDog(Kind kind, OverlappedRefs refs, OverlappedVals vals)
    {
        _kind = kind;
        _refs = refs;
        _vals = vals;
    }

    private CatOrDog(string name, int sleepingSpots)
    {
        _kind = Kind.Cat;
        _refs.cat.name = name;
        _vals.cat.sleepingSpots = sleepingSpots;
    }

    private CatOrDog(string name, bool isTrained)
    {
        _kind = Kind.Cat;
        _refs.dog.name = name;
        _vals.dog.isTrained = isTrained;
    }

    //public static CatOrDog Cat(string name, int sleepingSpots) =>
    //    new CatOrDog(Kind.Cat, new OverlappedRefs { cat = new CatRefs { name = name } }, new OverlappedVals { cat = new CatVals { sleepingSpots = sleepingSpots } });

    //public static CatOrDog Dog(string name, bool isTrained) =>
    //    new CatOrDog(Kind.Dog, new OverlappedRefs { dog = new DogRefs { name = name } }, new OverlappedVals { dog = new DogVals { isTrained = isTrained } });

    public static CatOrDog Cat(string name, int sleepingSpots) =>
        new CatOrDog(name, sleepingSpots);

    public static CatOrDog Dog(string name, bool isTrained) =>
        new CatOrDog(name, isTrained);

    public bool IsCat => _kind == Kind.Cat;
    public bool IsDog => _kind == Kind.Dog;

    public bool TryGetCat(out string name, out int sleepingSpots)
    {
        if (IsCat)
        {
            name = _refs.cat.name;
            sleepingSpots = _vals.cat.sleepingSpots;
            return true;
        }

        name = default!;
        sleepingSpots = default;
        return false;
    }

    public bool TryGetDog(out string name, out bool isTrained)
    {
        if (IsDog)
        {
            name = _refs.dog.name;
            isTrained = _vals.dog.isTrained;
            return true;
        }

        name = default!;
        isTrained = default;
        return true;
    }

    public (string name, int sleepingSpots) GetCat() =>
        TryGetCat(out var name, out var sleepingSpots)
            ? (name, sleepingSpots)
            : throw new InvalidCastException();

    public (string name, bool isTrained) GetDog() =>
        TryGetDog(out var name, out var isTrained)
            ? (name, isTrained)
            : throw new InvalidCastException();
}
