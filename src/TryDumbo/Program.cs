
// Example of have union type be erased at runtime, but using a extension-like type for type specific union API.

using Erased = Dumbo.Variant;
using AB = Dumbo.TypeUnions.Erased.Hybrid.OneOf<Animal, Automobile>;
using BA = Dumbo.TypeUnions.Erased.Hybrid.OneOf<Automobile, Animal>;

Erased u = (AB)new Animal.Cat("Mr Pickles");

switch (((AB)u).Tag)
{
    case 1:
        var animal = ((AB)u).GetType1();
        Console.WriteLine($"Its an animal: {animal}");
        break;
    case 2:
        var automobile = ((AB)u).GetType2();
        Console.WriteLine($"Its an automobile: {automobile}");
        break;
    default:
        Console.WriteLine("Its unknown");
        break;
}

switch (((BA)u).Tag)
{
    case 1:
        var automobile = ((BA)u).GetType1();
        Console.WriteLine($"Its an automobile: {automobile}");
        break;
    case 2:
        var animal = ((BA)u).GetType2();
        Console.WriteLine($"Its an animal: {animal}");
        break;
    default:
        Console.WriteLine("Its unknown");
        break;
}


Console.ReadKey();

public record Animal
{
    public record Cat(string Name) : Animal;
    public record Dog(string Name) : Animal;
}

public record Automobile
{
    public record Ford(string Model) : Automobile;
    public record Mercedes(string Model) : Automobile;
}