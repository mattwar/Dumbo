using Dumbo;

namespace Tests;

[TestClass]
public class Decimal64Tests
{
    [TestMethod]
    public void TestConvert()
    {
        TestConvert(0m, new Decimal64(0, 0));
        TestConvert(0.0m, new Decimal64(0, 1));
        TestConvert(1m, new Decimal64(1, 0));
        TestConvert(1.0m, new Decimal64(10, 1));
        TestConvert(20m, new Decimal64(20, 0));
        TestConvert(20.0m, new Decimal64(200, 1));
        TestConvert(2.00m, new Decimal64(200, 2));
        TestConvert(-1m, new Decimal64(-1, 0));
        TestConvert(-1.0m, new Decimal64(-10, 1));

        TestConvert(Decimal64.MinMagnitude, new Decimal64(Decimal64.MinMagnitude, 0));
        TestConvert(Decimal64.MaxMagnitude, new Decimal64(Decimal64.MaxMagnitude, 0));

        TestConvert(new Decimal64(Decimal64.MinMagnitude, 15).ToDecimal(), new Decimal64(Decimal64.MinMagnitude, 15));
        TestConvert(new Decimal64(Decimal64.MaxMagnitude, 15).ToDecimal(), new Decimal64(Decimal64.MaxMagnitude, 15));
    }

    private void TestConvert(decimal value, Decimal64 expected)
    {
        Assert.IsTrue(Decimal64.TryConvert(value, out var actual), "TryConvert");
        Assert.AreEqual(expected, actual);
        Assert.AreEqual(expected.Magnitude, actual.Magnitude, "Magnitude");
        Assert.AreEqual(expected.Scale, actual.Scale, "Scale");
        Assert.AreEqual(expected, Decimal64.Convert(value), "Convert");
    }
}
