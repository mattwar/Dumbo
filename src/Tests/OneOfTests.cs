using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dumbo;

namespace Tests
{
    [TestClass]
    public class OneOfTests
    {
        public record Apple(string name);
        public record Orange(string name);

        [TestMethod]
        public void TestTwo()
        {
            TestTwo(1, "one");
            TestTwo(1, 1.0);
            TestTwo(new Apple("Granny Smith"), new Orange("Navel"));
        }

        private void TestTwo<T1, T2>(T1 expected1, T2 expected2)
        {
            var oneOf1 = OneOf<T1, T2>.ConvertFrom<T1>(expected1);
            Assert.AreEqual(typeof(T1), oneOf1.Type);
            Assert.IsTrue(oneOf1.IsType<T1>());
            Assert.IsTrue(oneOf1.TryGet<T1>(out var actual1));
            Assert.AreEqual(expected1, actual1);

            var oneOf2 = OneOf<T1, T2>.ConvertFrom<T2>(expected2);
            Assert.AreEqual(typeof(T2), oneOf2.Type);
            Assert.IsTrue(oneOf2.IsType<T2>());
            Assert.IsTrue(oneOf2.TryGet<T2>(out var actual2));
            Assert.AreEqual(expected2, actual2);
        }
    }
}
