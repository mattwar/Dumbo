namespace Dumbo.TaggedUnions.Heirarchy;

public abstract record CatOrDog
{
    public record Cat(string name, int sleepingSpots) : CatOrDog;
    public record Dog(string name, bool isTrained) : CatOrDog;
}
