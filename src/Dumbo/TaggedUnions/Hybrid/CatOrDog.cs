namespace Dumbo.TaggedUnions.Hybrid
{
    public readonly struct CatOrDog
    {
        private enum Kind { Cat = 1, Dog };

        private readonly Kind _kind;
        private readonly Variant _value1;
        private readonly Variant _value2;

        private CatOrDog(Kind kind, Variant value1, Variant value2)
        {
            _kind = kind;
            _value1 = value1;
            _value2 = value2;
        }

        public static CatOrDog Cat(string name, int sleepingSpots) =>
            new CatOrDog(Kind.Cat, name, sleepingSpots);

        public static CatOrDog Dot(string name, bool isTrained) =>
            new CatOrDog(Kind.Dog, name, isTrained);

        public bool IsCat => _kind == Kind.Cat;
        public bool IsDog => _kind == Kind.Dog;

        public bool TryGetCat(out string name, out int sleepingSpots)
        {
            if (IsCat)
            {
                name = (string)_value1;
                sleepingSpots = (int)_value2;
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
                name = (string)_value1;
                isTrained = (bool)_value2;
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

        public (string name, bool isTrained) GetDot() =>
            TryGetDog(out var name, out var isTrained)
                ? (name, isTrained)
                : throw new InvalidCastException();
    }
}
