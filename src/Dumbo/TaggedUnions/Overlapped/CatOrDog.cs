#define TAG_TYPED
using System.Runtime.InteropServices;
using System.Xml.Linq;

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

#if TAG_TYPED
    public record struct CatData(string Name, int SleepingSpots);
    public record struct DogData(string Name, bool IsTrained);

    public CatOrDog(in CatData cat)
    {
        _kind = Kind.Cat;
        _refs.cat.name = cat.Name;
        _vals.cat.sleepingSpots = cat.SleepingSpots;
    }

    public CatOrDog(in DogData dog)
    {
        _kind = Kind.Dog;
        _refs.dog.name = dog.Name;
        _vals.dog.isTrained = dog.IsTrained;
    }

    public static CatOrDog Cat(string name, int sleepingSpots)
    {
        var cat = new CatData(name, sleepingSpots);
        return new CatOrDog(in cat);
    }

    public static CatOrDog Dog(string name, bool isTrained)
    {
        var dog = new DogData(name, isTrained);
        return new CatOrDog(in dog);
    }
#else
    private CatOrDog(string name, int sleepingSpots)
    {
        _kind = Kind.Cat;
        _refs.cat.name = name;
        _vals.cat.sleepingSpots = sleepingSpots;
    }

    private CatOrDog(string name, bool isTrained)
    {
        _kind = Kind.Dog;
        _refs.dog.name = name;
        _vals.dog.isTrained = isTrained;
    }

    public static CatOrDog Cat(string name, int sleepingSpots) =>
        new CatOrDog(name, sleepingSpots);

    public static CatOrDog Dog(string name, bool isTrained) =>
        new CatOrDog(name, isTrained);
#endif

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

#if TAG_TYPED
    public CatData GetCat() =>
        TryGetCat(out var name, out var sleepingSpots)
            ? new CatData(name, sleepingSpots)
            : throw new InvalidCastException();

    public DogData GetDog() =>
        TryGetDog(out var name, out var isTrained)
            ? new DogData(name, isTrained)
            : throw new InvalidCastException();
#else
    public (string name, int sleepingSpots) GetCat() =>
        TryGetCat(out var name, out var sleepingSpots)
            ? (name, sleepingSpots)
            : throw new InvalidCastException();

    public (string name, bool isTrained) GetDog() =>
        TryGetDog(out var name, out var isTrained)
            ? (name, isTrained)
            : throw new InvalidCastException();
#endif
}
