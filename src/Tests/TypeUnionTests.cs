using Dumbo;
using Dumbo.TypeUnions;

namespace Tests
{
    [TestClass]
    public class TypeUnionTests
    {
        public record Apple(string name);
        public record struct Orange(string name);

        [TestMethod]
        public void TestOneOf()
        {
            TestOneOf<Dumbo.TypeUnions.Existing.Fat.OneOf<int, string>>();
            TestOneOf<Dumbo.TypeUnions.Existing.Boxed.OneOf<int, string>>();
            TestOneOf<Dumbo.TypeUnions.Existing.Hybrid.OneOf<int, string>>();
        }

        private void TestOneOf<TOneOf>()
            where TOneOf : ITypeUnion<TOneOf>
        {
            TestUnion<TOneOf>.TestValue(1);
            TestUnion<TOneOf>.TestValue("one");
        }

        private static class TestUnion<TUnion>
            where TUnion : ITypeUnion<TUnion>
        {
            public static void TestValue<TValue>(TValue expected)
            {
                var oneOf1 = TUnion.Create(expected);
                Assert.IsTrue(oneOf1.IsType<TValue>());
                Assert.IsTrue(oneOf1.TryGet<TValue>(out var actual1));
                Assert.AreEqual(expected, actual1);
            }
        }
    }
}
