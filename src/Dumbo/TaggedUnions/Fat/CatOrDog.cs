namespace Dumbo.TaggedUnions.Fat;

public readonly struct CatOrDog
{
    private enum Kind { Cat = 1, Dog };

    private readonly Kind _kind;
    private readonly string _cat_name;
    private readonly int _cat_sleepingSpots;
    private readonly string _dog_name;
    private readonly bool _dog_isTrained;

    private CatOrDog(Kind kind, string cat_name, int cat_sleepingSpots, string dog_name, bool dog_isTrained)
    {
        _kind = kind;
        _cat_name = cat_name;
        _cat_sleepingSpots = cat_sleepingSpots;
        _dog_name = dog_name;
        _dog_isTrained = dog_isTrained;
    }

    public static CatOrDog Cat(string name, int sleepingSpots) =>
        new CatOrDog(Kind.Cat, name, sleepingSpots, default!, default!);

    public static CatOrDog Dog(string name, bool isTrained) =>
        new CatOrDog(Kind.Dog, default!, default!, name, isTrained);

    public bool IsCat => _kind == Kind.Cat;
    public bool IsDog => _kind == Kind.Dog;

    public bool TryGetCat(out string name, out int sleepingSpots)
    {
        if (IsCat)
        {
            name = _cat_name;
            sleepingSpots = _cat_sleepingSpots;
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
            name = _dog_name;
            isTrained = _dog_isTrained;
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
