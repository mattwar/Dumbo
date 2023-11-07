using BenchmarkDotNet.Attributes;
using System.Drawing;
using Dumbo;

namespace Benchmarks
{
    public class Variant_TryGet
    {
        [Params(1000)]
        public int Iterations { get; set; }

        private record ReferenceType(string a, int b);
        private record struct SmallStructMixed(string a, int b);
        private record struct WrapperStruct(string a);
        private record struct SmallStructNoRefs(int a, int b);
        private record struct LargeStructNoRefs(int a, int b, int c, int d);
        private record struct LargeStructMixed(string a, int b, string c, int d);

        private enum I8Enum : sbyte { A = 1, B, C };
        private enum I16Enum : short { A = 1, B, C };
        private enum I32Enum : int { A = 1, B, C };
        private enum I64Enum : long { A = 1, B, C };
        private enum UI8Enum : byte { A = 1, B, C };
        private enum UI16Enum : ushort { A = 1, B, C };
        private enum UI32Enum : uint { A = 1, B, C };
        private enum UI64Enum : ulong { A = 1, B, C };

        private void Test<T>(T value)
        {
            var v = Variant.Create(value);

            for (int i = 0; i < Iterations; i++)
            {
                if (v.TryGet<T>(out value))
                {
                }
            }
        }

        [Benchmark]
        public void Variant_TryGet_Int()
        {
            Test(1);
        }

        [Benchmark]
        public void Variant_TryGet_Enum()
        {
            Test(UI32Enum.A);
        }

        [Benchmark]
        public void Variant_TryGet_Nullable_Int()
        {
            Test<int?>(1);
        }

        [Benchmark]
        public void Variant_TryGet_Decimal_Small()
        {
            Test(1.0m);
        }

        [Benchmark]
        public void Variant_TryGet_String()
        {
            Test("one");
        }

        [Benchmark]
        public void Variant_TryGet_ReferenceType()
        {
            Test(new ReferenceType("one", 1));
        }

        [Benchmark]
        public void Variant_TryGet_SmallStructMixed()
        {
            Test(new SmallStructMixed("one", 1));
        }

        [Benchmark]
        public void Variant_TryGet_WrapperStruct()
        {
            Test(new WrapperStruct("one"));
        }

        [Benchmark]
        public void Variant_TryGet_SmallStructNoRefs()
        {
            Test(new SmallStructNoRefs(1, 2));
        }

        [Benchmark]
        public void Variant_TryGet_LargeStructNoRefs()
        {
            Test(new LargeStructNoRefs(1, 2, 3, 4));
        }

        [Benchmark]
        public void Variant_TryGet_LargeStructMixed()
        {
            Test(new LargeStructMixed("one", 1, "two", 2));
        }
    }
}
