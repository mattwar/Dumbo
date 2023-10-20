namespace Dumbo.TaggedUnions.Heirarchy
{
    public abstract record CatOrDog()
    {
        public record Cat(string name, int sleepingSpots) : CatOrDog;
        public record Dog(string name, bool isTrained) : CatOrDog;

        public bool IsCat => this is Cat;
        public bool IsDog => this is Dog;
    }
}
