using Dumbo;

namespace Tests;

[TestClass]
public class Decimal64Tests
{
    [TestMethod]
    public void TestCreate()
    {
        TestCreate(0m, new Decimal64(0, 0));
        TestCreate(0.0m, new Decimal64(0, 1));
        TestCreate(1m, new Decimal64(1, 0));
        TestCreate(1.0m, new Decimal64(10, 1));
        TestCreate(20m, new Decimal64(20, 0));
        TestCreate(20.0m, new Decimal64(200, 1));
        TestCreate(2.00m, new Decimal64(200, 2));
        TestCreate(-1m, new Decimal64(-1, 0));
        TestCreate(-1.0m, new Decimal64(-10, 1));

        TestCreate(Decimal64.MinMagnitude, new Decimal64(Decimal64.MinMagnitude, 0));
        TestCreate(Decimal64.MaxMagnitude, new Decimal64(Decimal64.MaxMagnitude, 0));

        TestCreate(new Decimal64(Decimal64.MinMagnitude, 15).ToDecimal(), new Decimal64(Decimal64.MinMagnitude, 15));
        TestCreate(new Decimal64(Decimal64.MaxMagnitude, 15).ToDecimal(), new Decimal64(Decimal64.MaxMagnitude, 15));
    }

    private void TestCreate(decimal value, Decimal64 expected)
    {
        Assert.IsTrue(Decimal64.TryCreate(value, out var actual), "TryConvert");
        Assert.AreEqual(expected, actual);
        Assert.AreEqual(expected.Magnitude, actual.Magnitude, "Magnitude");
        Assert.AreEqual(expected.Scale, actual.Scale, "Scale");
        Assert.AreEqual(expected, Decimal64.Create(value), "Convert");
    }
}
