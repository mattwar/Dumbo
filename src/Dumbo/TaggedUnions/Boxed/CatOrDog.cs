namespace Dumbo.TaggedUnions.Boxed;

public readonly struct CatOrDog
{
    private enum Kind { Cat = 1, Dog };

    private readonly Kind _kind;
    private readonly object? _value1;
    private readonly object? _value2;

    private CatOrDog(Kind kind, object? value1, object? value2)
    {
        _kind = kind;
        _value1 = value1;
        _value2 = value2;
    }

    public static CatOrDog Cat(string name, int sleepingSpots) =>
        new CatOrDog(Kind.Cat, name, sleepingSpots);

    public static CatOrDog Dog(string name, bool isTrained) =>
        new CatOrDog(Kind.Dog, name, isTrained);

    public bool IsCat => _kind == Kind.Cat;
    public bool IsDog => _kind == Kind.Dog;

    public bool TryGetCat(out string name, out int sleepingSpots)
    {
        if (IsCat && _value1 is string vname && _value2 is int vspots)
        {
            name = vname;
            sleepingSpots = vspots;
            return true;
        }

        name = default!;
        sleepingSpots = default;
        return false;
    }

    public bool TryGetDog(out string name, out bool isTrained)
    {
        if (IsDog && _value1 is string vname && _value2 is bool vtrain)
        {
            name = vname;
            isTrained = vtrain;
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
