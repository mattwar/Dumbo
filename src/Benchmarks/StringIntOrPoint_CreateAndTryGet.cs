using BenchmarkDotNet.Attributes;
using System.Drawing;
using Dumbo;
using B = Dumbo.TypeUnions.Boxed;
using F = Dumbo.TypeUnions.Fat;
using H = Dumbo.TypeUnions.Hybrid;
using EB = Dumbo.TypeUnions.Existing.Boxed;
using EF = Dumbo.TypeUnions.Existing.Fat;
using EH = Dumbo.TypeUnions.Existing.Hybrid;

namespace Benchmarks
{
    internal class StringIntOrPoint_CreateAndTryGet
    {
        [Benchmark]
        public void Int_OneOf_Boxed()
        {
            var union = EB.OneOf<string, int, Point>.Create(1);
            if (union.TryGet(out int value))
            {
            }
        }

        [Benchmark]
        public void Int_OneOf_Fat()
        {
            var union = EF.OneOf<string, int, Point>.Create(1);
            if (union.TryGet(out int value))
            {
            }
        }

        [Benchmark]
        public void Int_OneOf_Hybrid()
        {
            var union = EH.OneOf<string, int, Point>.Create(1);
            if (union.TryGet(out int value))
            {
            }
        }

        [Benchmark]
        public void Int_StringIntOrPoint_Fat()
        {
            var union = F.StringIntOrPoint.Create(1);
            if (union.TryGet(out int value))
            {
            }
        }

        [Benchmark]
        public void Int_StringIntOrPoint_Boxed()
        {
            var union = B.StringIntOrPoint.Create(1);
            if (union.TryGet(out int value))
            {
            }
        }

        [Benchmark]
        public void Int_StringIntOrPoint_Hybrid()
        {
            var union = H.StringIntOrPoint.Create(1);
            if (union.TryGet(out int value))
            {
            }
        }
    }
}
