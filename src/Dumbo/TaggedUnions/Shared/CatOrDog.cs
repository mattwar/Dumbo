namespace Dumbo.TaggedUnions.Shared;

public readonly struct CatOrDog
{
    private enum Kind { Cat = 1, Dog };

    private readonly Kind _kind;
    private readonly string _value1;
    private readonly int _value2;
    private readonly bool _value3;

    private CatOrDog(Kind kind, string value1, int value2, bool value3)
    {
        _kind = kind;
        _value1 = value1;
        _value2 = value2;
        _value3 = value3;
    }

    public static CatOrDog Cat(string name, int sleepingSpots) =>
        new CatOrDog(Kind.Cat, name, sleepingSpots, default!);

    public static CatOrDog Dog(string name, bool isTrained) =>
        new CatOrDog(Kind.Dog, name, default!, isTrained);

    public bool IsCat => _kind == Kind.Cat;
    public bool IsDog => _kind == Kind.Dog;

    public bool TryGetCat(out string name, out int sleepingSpots)
    {
        if (IsCat)
        {
            name = _value1;
            sleepingSpots = _value2;
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
            name = _value1;
            isTrained = _value3;
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
