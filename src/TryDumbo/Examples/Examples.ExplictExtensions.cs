using Dumbo;
using System.Text;
using Erased = Dumbo.Variant;

using AB = Dumbo.TypeUnions.Erased.Hybrid.OneOf<Animal, Automobile>;
using BA = Dumbo.TypeUnions.Erased.Hybrid.OneOf<Automobile, Animal>;

file record Animal
{
    public record Cat(string Name) : Animal;
    public record Dog(string Name) : Animal;
}

file record Automobile
{
    public record Ford(string Model) : Automobile;
    public record Mercedes(string Model) : Automobile;
}

public partial class Examples
{
    /// <summary>
    /// Example of have union type be erased at runtime and use an extension for type specific union API. 
    /// </summary>
    public static void ExplicitExtensions()
    {
        // AB ab = new Animal.Cat("Mr Pickles")
        Erased x = (AB)new Animal.Cat("Mr Pickles");

        // switch (ab) 
        switch (((AB)x).Tag)
        {
            // case Animal animal:
            case 1:
                var animal = ((AB)x).GetType1();
                Console.WriteLine($"Its an animal: {animal}");
                break;
            // case Automobile automobile:
            case 2:
                var automobile = ((AB)x).GetType2();
                Console.WriteLine($"Its an automobile: {automobile}");
                break;
            default:
                Console.WriteLine("Its unknown");
                break;
        }

        // BA ba = ab;
        // switch (ba)
        switch (((BA)x).Tag)
        {
            // case Automobile automobile:
            case 1:
                var automobile = ((BA)x).GetType1();
                Console.WriteLine($"Its an automobile: {automobile}");
                break;
            // case Animal animal:
            case 2:
                var animal = ((BA)x).GetType2();
                Console.WriteLine($"Its an animal: {animal}");
                break;
            default:
                Console.WriteLine("Its unknown");
                break;
        }
    }
}
