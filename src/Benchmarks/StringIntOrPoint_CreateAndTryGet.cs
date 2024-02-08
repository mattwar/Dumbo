using BenchmarkDotNet.Attributes;
using System.Drawing;
using Dumbo;
using B = Dumbo.TypeUnions.Boxed;
using F = Dumbo.TypeUnions.Fat;
using O = Dumbo.TypeUnions.Overlapped;
using H = Dumbo.TypeUnions.Hybrid;
using EB = Dumbo.TypeUnions.Existing.Boxed;
using EF = Dumbo.TypeUnions.Existing.Fat;
using EH = Dumbo.TypeUnions.Existing.Hybrid;

namespace Benchmarks
{
    public class StringIntOrPoint_CreateAndTryGet
    {
        [Params(1000)]
        public int Iterations { get; set; }

        private void Test(Action action)
        {
            for (int i = 0; i < Iterations; i++)
            {
                action();
            }
        }

        [Benchmark]
        public void String_OneOf_Boxed_Generic()
        {
            Test(() =>
            {
                var union = EB.OneOf<string, int, Point>.Create<string>("one");
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void String_OneOf_Boxed_NonGen()
        {
            Test(() =>
            {
                var union = EB.OneOf<string, int, Point>.Create("one");
                if (union.TryGetType1(out string value))
                {
                }
            });
        }

        [Benchmark]
        public void String_OneOf_Fat_Generic()
        {
            Test(() =>
            {
                var union = EF.OneOf<string, int, Point>.Create<string>("one");
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void String_OneOf_Fat_NonGen()
        {
            Test(() =>
            {
                var union = EF.OneOf<string, int, Point>.Create("one");
                if (union.TryGetType1(out string value))
                {
                }
            });
        }

        [Benchmark]
        public void String_OneOf_Hybrid()
        {
            Test(() =>
            {
                var union = EH.OneOf<string, int, Point>.Create("one");
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void String_StringIntOrPoint_Fat_Generic()
        {
            Test(() =>
            {
                var union = F.StringIntOrPoint.Create<string>("one");
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void String_StringIntOrPoint_Fat_NonGen()
        {
            Test(() =>
            {
                var union = F.StringIntOrPoint.Create("one");
                if (union.TryGetType1(out string value))
                {
                }
            });
        }

        [Benchmark]
        public void String_StringIntOrPoint_Overlapped_Generic()
        {
            Test(() =>
            {
                var union = O.StringIntOrPoint.Create<string>("one");
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void String_StringIntOrPoint_Overlapped_NonGen()
        {
            Test(() =>
            {
                var union = O.StringIntOrPoint.Create("one");
                if (union.TryGetType1(out string value))
                {
                }
            });
        }

        [Benchmark]
        public void String_StringIntOrPoint_Boxed_Generic()
        {
            Test(() =>
            {
                var union = B.StringIntOrPoint.Create<string>("one");
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void String_StringIntOrPoint_Boxed_NonGen()
        {
            Test(() =>
            {
                var union = B.StringIntOrPoint.Create("one");
                if (union.TryGetType1(out string value))
                {
                }
            });
        }

        [Benchmark]
        public void String_StringIntOrPoint_Hybrid_Generic()
        {
            Test(() =>
            {
                var union = H.StringIntOrPoint.Create("one");
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void String_StringIntOrPoint_Hybrid_NonGeneric()
        {
            Test(() =>
            {
                var union = H.StringIntOrPoint.Create("one");
                if (union.TryGet(out string? value))
                {
                }
            });
        }



        [Benchmark]
        public void Int_OneOf_Boxed_Generic()
        {
            Test(() =>
            {
                var union = EB.OneOf<string, int, Point>.Create<int>(1);
                if (union.TryGet<int>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Int_OneOf_Boxed_NonGen()
        {
            Test(() =>
            {
                var union = EB.OneOf<string, int, Point>.Create(1);
                if (union.TryGetType2(out int value))
                {
                }
            });
        }

        [Benchmark]
        public void Int_OneOf_Fat_Generic()
        {
            Test(() =>
            {
                var union = EF.OneOf<string, int, Point>.Create<int>(1);
                if (union.TryGet<int>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Int_OneOf_Fat_NonGen()
        {
            Test(() =>
            {
                var union = EF.OneOf<string, int, Point>.Create(1);
                if (union.TryGetType2(out int value))
                {
                }
            });
        }

        [Benchmark]
        public void Int_OneOf_Hybrid()
        {
            Test(() =>
            {
                var union = EH.OneOf<string, int, Point>.Create(1);
                if (union.TryGet<int>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Int_StringIntOrPoint_Fat_Generic()
        {
            Test(() =>
            {
                var union = F.StringIntOrPoint.Create<int>(1);
                if (union.TryGet<int>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Int_StringIntOrPoint_Fat_NonGen()
        {
            Test(() =>
            {
                var union = F.StringIntOrPoint.Create(1);
                if (union.TryGetType2(out int value))
                {
                }
            });
        }

        [Benchmark]
        public void Int_StringIntOrPoint_Overlapped_Generic()
        {
            Test(() =>
            {
                var union = O.StringIntOrPoint.Create<int>(1);
                if (union.TryGet<int>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Int_StringIntOrPoint_Overlapped_NonGen()
        {
            Test(() =>
            {
                var union = O.StringIntOrPoint.Create(1);
                if (union.TryGetType2(out int value))
                {
                }
            });
        }

        [Benchmark]
        public void Int_StringIntOrPoint_Boxed_Generic()
        {
            Test(() =>
            {
                var union = B.StringIntOrPoint.Create<int>(1);
                if (union.TryGet<int>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Int_StringIntOrPoint_Boxed_NonGen()
        {
            Test(() =>
            {
                var union = B.StringIntOrPoint.Create(1);
                if (union.TryGetType2(out int value))
                {
                }
            });
        }

        [Benchmark]
        public void Int_StringIntOrPoint_Hybrid_Generic()
        {
            Test(() =>
            {
                var union = H.StringIntOrPoint.Create(1);
                if (union.TryGet<int>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Int_StringIntOrPoint_Hybrid_NonGeneric()
        {
            Test(() =>
            {
                var union = H.StringIntOrPoint.Create(1);
                if (union.TryGet(out int value))
                {
                }
            });
        }


        [Benchmark]
        public void Point_OneOf_BoxedGeneric()
        {
            Test(() =>
            {
                var union = EB.OneOf<string, int, Point>.Create<Point>(new Point(1,1));
                if (union.TryGet<Point>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Point_OneOf_Boxed_NonGen()
        {
            Test(() =>
            {
                var union = EB.OneOf<string, int, Point>.Create(new Point(1, 1));
                if (union.TryGetType3(out Point value))
                {
                }
            });
        }

        [Benchmark]
        public void Point_OneOf_Fat_Generic()
        {
            Test(() =>
            {
                var union = EF.OneOf<string, int, Point>.Create<Point>(new Point(1, 1));
                if (union.TryGet<Point>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Point_OneOf_Fat_NonGen()
        {
            Test(() =>
            {
                var union = EF.OneOf<string, int, Point>.Create(new Point(1, 1));
                if (union.TryGetType3(out Point value))
                {
                }
            });
        }

        [Benchmark]
        public void Point_OneOf_Hybrid()
        {
            Test(() =>
            {
                var union = EH.OneOf<string, int, Point>.Create(new Point(1, 1));
                if (union.TryGet<Point>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Point_StringIntOrPoint_Fat_Generic()
        {
            Test(() =>
            {
                var union = F.StringIntOrPoint.Create<Point>(new Point(1,1));
                if (union.TryGet<Point>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Point_StringIntOrPoint_Fat_NonGen()
        {
            Test(() =>
            {
                var union = F.StringIntOrPoint.Create(new Point(1, 1));
                if (union.TryGetType3(out Point value))
                {
                }
            });
        }

        [Benchmark]
        public void Point_StringIntOrPoint_Overlapped_Generic()
        {
            Test(() =>
            {
                var union = O.StringIntOrPoint.Create<Point>(new Point(1,1));
                if (union.TryGet<Point>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Point_StringIntOrPoint_Overlapped_NonGen()
        {
            Test(() =>
            {
                var union = O.StringIntOrPoint.Create(new Point(1,1));
                if (union.TryGetType3(out Point value))
                {
                }
            });
        }

        [Benchmark]
        public void Point_StringIntOrPoint_Boxed_Generic()
        {
            Test(() =>
            {
                var union = B.StringIntOrPoint.Create<Point>(new Point(1, 1));
                if (union.TryGet<Point>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Point_StringIntOrPoint_Boxed_NonGen()
        {
            Test(() =>
            {
                var union = B.StringIntOrPoint.Create(new Point(1, 1));
                if (union.TryGetType3(out Point value))
                {
                }
            });
        }

        [Benchmark]
        public void Point_StringIntOrPoint_Hybrid()
        {
            Test(() =>
            {
                var union = H.StringIntOrPoint.Create(new Point(1, 1));
                if (union.TryGet<Point>(out var value))
                {
                }
            });
        }
    }
}
