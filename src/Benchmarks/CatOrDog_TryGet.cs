using BenchmarkDotNet.Attributes;
using System.Drawing;
using B = Dumbo.TaggedUnions.Boxed;
using F = Dumbo.TaggedUnions.Fat;
using Hy = Dumbo.TaggedUnions.Hybrid;
using He = Dumbo.TaggedUnions.Heirarchy;
using Hi = Dumbo.TaggedUnions.Hidden;
using O = Dumbo.TaggedUnions.Overlapped;
using S = Dumbo.TaggedUnions.Shared;


namespace Benchmarks
{
    public class CatOrDog_TryGet
    {
        [Params(1000)]
        public int Iterations { get; set; }

        public void Test(Action action)
        {
            for (int i = 0; i < Iterations; i++)
            {
                action();
            }
        }

        [Benchmark]
        public void CatOrDog_Boxed()
        {
            var union = B.CatOrDog.Dog("Fido", true);

            Test(() =>
            {
                if (union.TryGetCat(out var name, out var sleepingSpots))
                {
                }
            });
        }

        [Benchmark]
        public void CatOrDog_Fat()
        {
            var union = F.CatOrDog.Dog("Fido", true);

            Test(() =>
            {
                if (union.TryGetCat(out var name, out var sleepingSpots))
                {
                }
            });
        }

        [Benchmark]
        public void CatOrDog_Hybrid()
        {
            var union = Hy.CatOrDog.Dog("Fido", true);

            Test(() =>
            {
                if (union.TryGetCat(out var name, out var sleepingSpots))
                {
                }
            });
        }

        [Benchmark]
        public void CatOrDog_Overlapped()
        {
            var union = O.CatOrDog.Dog("Fido", true);

            Test(() =>
            {
                if (union.TryGetCat(out var name, out var sleepingSpots))
                {
                }
            });
        }

        [Benchmark]
        public void CatOrDog_Shared()
        {
            var union = S.CatOrDog.Dog("Fido", true);

            Test(() =>
            {
                if (union.TryGetCat(out var name, out var sleepingSpots))
                {
                }
            });
        }
    }
}
