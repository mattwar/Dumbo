using Dumbo;
using Dumbo.TypeUnions.Existing.Hybrid;

OneOf<Fruit, Vegetable> a = Fruit.Apple;
OneOf<Fruit, Vegetable> b = Vegetable.Carrot;

Variant vApple = Variant.Create(a);
Variant vVeg = Variant.Create(b);

Vegetable veg = vApple.ConvertTo<Vegetable>(); // good idea?

Console.ReadLine();

enum Fruit
{
    Apple = 1,
    Orange,
    Banana
}

enum Vegetable
{
    Carrot = 1,
    Lettuce,
    Onion
}