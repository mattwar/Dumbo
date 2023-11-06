using Dumbo;
using System.Drawing;

namespace Tests;

[TestClass]
public class TypeUnionTests
{
    public record Apple(string name);
    public record struct Orange(string name);

    [TestMethod]
    public void TestOneOf()
    {
        TestUnion<Dumbo.TypeUnions.Existing.Fat.OneOf<int, string, Point>>()
            .Value(1)
            .Value("One")
            .Value(new Point(1, 1));

        TestUnion<Dumbo.TypeUnions.Existing.Boxed.OneOf<int, string, Point>>()
            .Value(1)
            .Value("One")
            .Value(new Point(1, 1));

        TestUnion<Dumbo.TypeUnions.Existing.Hybrid.OneOf<int, string, Point>>()
            .Value(1)
            .Value("One")
            .Value(new Point(1, 1));
    }

    [TestMethod]
    public void TestCatOrDog()
    {
    }

    private UnionTester<TUnion> TestUnion<TUnion>()
        where TUnion : ITypeUnion<TUnion> 
    {
        return new UnionTester<TUnion>();
    }

    private class UnionTester<TUnion>
        where TUnion : ITypeUnion<TUnion>
    {
        /// <summary>
        /// That the union can be created with the value, type tested and retrieved back as the same value.
        /// </summary>
        public UnionTester<TUnion> Value<TValue>(TValue expected)
        {
            Assert.IsTrue(TUnion.TryCreate(expected, out var oneOf1), "TryCreate");
            Assert.IsTrue(oneOf1.IsType<TValue>(), "IsType");
            Assert.IsTrue(oneOf1.TryGet<TValue>(out var actual1), "TryGet");
            Assert.AreEqual(expected, actual1);
            return this;
        }
    }
}
