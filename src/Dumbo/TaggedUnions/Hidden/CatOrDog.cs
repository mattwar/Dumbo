namespace Dumbo.TaggedUnions.Hidden;

public abstract record CatOrDog
{
    private record CatData(string name, int sleepingSpots) : CatOrDog;
    private record DogData(string name, bool isTrained) : CatOrDog;

    public static CatOrDog Cat(string name, int sleepingSpots) =>
        new CatData(name, sleepingSpots);

    public static CatOrDog Dog(string name, bool isTrained) =>
        new DogData(name, isTrained);

    public bool IsCat => this is CatData;
    public bool IsDog => this is DogData;

    public bool TryGetCat(out string name, out int sleepingSpots)
    {
        if (this is CatData cat)
        {
            (name, sleepingSpots) = (cat.name, cat.sleepingSpots);
            return true;
        }

        name = default!;
        sleepingSpots = default;
        return false;
    }

    public bool TryGetDog(out string name, out bool isTrained)
    {
        if (this is DogData dog)
        {
            (name, isTrained) = (dog.name, dog.isTrained);
            return true;
        }

        name = default!;
        isTrained = default;
        return false;
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
