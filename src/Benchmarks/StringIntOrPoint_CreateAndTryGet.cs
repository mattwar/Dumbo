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
        public void String_OneOf_Boxed()
        {
            Test(() =>
            {
                var union = EB.OneOf<string, int, Point>.Create("one");
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void String_OneOf_Fat()
        {
            Test(() =>
            {
                var union = EF.OneOf<string, int, Point>.Create("one");
                if (union.TryGet<string>(out var value))
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
        public void String_StringIntOrPoint_Fat()
        {
            Test(() =>
            {
                var union = F.StringIntOrPoint.Create("one");
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void String_StringIntOrPoint_Boxed()
        {
            Test(() =>
            {
                var union = B.StringIntOrPoint.Create("one");
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void String_StringIntOrPoint_Hybrid()
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
        public void Int_OneOf_Boxed()
        {
            Test(() =>
            {
                var union = EB.OneOf<string, int, Point>.Create(1);
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Int_OneOf_Fat()
        {
            Test(() =>
            {
                var union = EF.OneOf<string, int, Point>.Create(1);
                if (union.TryGet<string>(out var value))
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
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Int_StringIntOrPoint_Fat()
        {
            Test(() =>
            {
                var union = F.StringIntOrPoint.Create(1);
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Int_StringIntOrPoint_Boxed()
        {
            Test(() =>
            {
                var union = B.StringIntOrPoint.Create(1);
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Int_StringIntOrPoint_Hybrid()
        {
            Test(() =>
            {
                var union = H.StringIntOrPoint.Create(1);
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }


        [Benchmark]
        public void Point_OneOf_Boxed()
        {
            Test(() =>
            {
                var union = EB.OneOf<string, int, Point>.Create(new Point(1,1));
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Point_OneOf_Fat()
        {
            Test(() =>
            {
                var union = EF.OneOf<string, int, Point>.Create(new Point(1, 1));
                if (union.TryGet<string>(out var value))
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
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Point_StringIntOrPoint_Fat()
        {
            Test(() =>
            {
                var union = F.StringIntOrPoint.Create(new Point(1,1));
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }

        [Benchmark]
        public void Point_StringIntOrPoint_Boxed()
        {
            Test(() =>
            {
                var union = B.StringIntOrPoint.Create(new Point(1, 1));
                if (union.TryGet<string>(out var value))
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
                if (union.TryGet<string>(out var value))
                {
                }
            });
        }
    }
}
