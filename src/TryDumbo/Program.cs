using Dumbo;
using Dumbo.TypeUnions.Existing.Fat;
using Dumbo.TypeUnions.Fat;
using System.Drawing;

// Use this space to experiment with unions
var v1 = Variant.Create(100);
var v2 = Variant.Create(100);
var v3 = Variant.Create(200);
var v4 = Variant.Create("100");
var v5 = v3;

Console.WriteLine($"v1 == v2: {v1 == v2}");
Console.WriteLine($"v1 == v3: {v1 == v3}");
Console.WriteLine($"v1 == v4: {v1 == v4}");
Console.WriteLine($"v3 == v5: {v3 == v5}");

Console.ReadLine();