using BenchmarkDotNet.Attributes;
using System.Drawing;
using B = Dumbo.TypeUnions.Boxed;
using F = Dumbo.TypeUnions.Fat;
using H = Dumbo.TypeUnions.Hybrid;
using EB = Dumbo.TypeUnions.Existing.Boxed;
using EF = Dumbo.TypeUnions.Existing.Fat;
using EH = Dumbo.TypeUnions.Existing.Hybrid;

namespace Benchmarks;

// [MemoryDiagnoser]
public class StringIntOrPoint_Create
{
    [Params(1000)]
    public int Iterations { get; set; }

    [Benchmark]
    public void String_OneOf_Boxed()
    {
        EB.OneOf<int, string, Point> union;

        for (int i = 0; i < Iterations; i++)
        {
            union = EB.OneOf<int, string, Point>.Create("one");
        }
    }

    [Benchmark]
    public void String_OneOf_Fat()
    {
        EF.OneOf<int, string, Point> union;

        for (int i = 0; i < Iterations; i++)
        {
            union = EF.OneOf<int, string, Point>.Create("one");
        }
    }

    [Benchmark]
    public void String_OneOf_Hybrid()
    {
        EH.OneOf<int, string, Point> union;

        for (int i = 0; i < Iterations; i++)
        {
            union = EH.OneOf<int, string, Point>.Create("one");
        }
    }

    [Benchmark]
    public void String_StringIntOrPoint_Fat()
    {
        F.StringIntOrPoint union;

        for (int i = 0; i < Iterations; i++)
        {
            union = F.StringIntOrPoint.Create("one");
        }
    }

    [Benchmark]
    public void String_StringIntOrPoint_Boxed()
    {
        B.StringIntOrPoint union;

        for (int i = 0; i < Iterations; i++)
        {
            union = B.StringIntOrPoint.Create("one");
        }
    }

    [Benchmark]
    public void String_StringIntOrPoint_Hybrid()
    {
        H.StringIntOrPoint union;

        for (int i = 0; i < Iterations; i++)
        {
            union = H.StringIntOrPoint.Create("one");
        }
    }


    [Benchmark]
    public void Int_OneOf_Boxed()
    {
        EB.OneOf<int, string, Point> union;

        for (int i = 0; i < Iterations; i++)
        {
            union = EB.OneOf<int, string, Point>.Create(1);
        }
    }

    [Benchmark]
    public void Int_OneOf_Fat()
    {
        EF.OneOf<int, string, Point> union;

        for (int i = 0; i < Iterations; i++)
        {
            union = EF.OneOf<int, string, Point>.Create(1);
        }
    }

    [Benchmark]
    public void Int_OneOf_Hybrid()
    {
        EH.OneOf<int, string, Point> union;

        for (int i = 0; i < Iterations; i++)
        {
            union = EH.OneOf<int, string, Point>.Create(1);
        }
    }

    [Benchmark]
    public void Int_StringIntOrPoint_Fat()
    {
        F.StringIntOrPoint union;

        for (int i = 0; i < Iterations; i++)
        {
            union = F.StringIntOrPoint.Create(1);
        }
    }

    [Benchmark]
    public void Int_StringIntOrPoint_Boxed()
    {
        B.StringIntOrPoint union;

        for (int i = 0; i < Iterations; i++)
        {
            union = B.StringIntOrPoint.Create(1);
        }
    }

    [Benchmark]
    public void Int_StringIntOrPoint_Hybrid()
    {
        H.StringIntOrPoint union;

        for (int i = 0; i < Iterations; i++)
        {
            union = H.StringIntOrPoint.Create(1);
        }
    }


    [Benchmark]
    public void Point_OneOf_Boxed()
    {
        EB.OneOf<int, string, Point> union;

        for (int i = 0; i < Iterations; i++)
        {
            union = EB.OneOf<int, string, Point>.Create(new Point(1, 1));
        }
    }

    [Benchmark]
    public void Point_OneOf_Fat()
    {
        EF.OneOf<int, string, Point> union;

        for (int i = 0; i < Iterations; i++)
        {
            union = EF.OneOf<int, string, Point>.Create(new Point(1, 1));
        }
    }

    [Benchmark]
    public void Point_OneOf_Hybrid()
    {
        EH.OneOf<int, string, Point> union;

        for (int i = 0; i < Iterations; i++)
        {
            union = EH.OneOf<int, string, Point>.Create(new Point(1, 1));
        }
    }

    [Benchmark]
    public void Point_StringIntOrPoint_Fat()
    {
        F.StringIntOrPoint union;

        for (int i = 0; i < Iterations; i++)
        {
            union = F.StringIntOrPoint.Create(new Point(1, 1));
        }
    }

    [Benchmark]
    public void Point_StringIntOrPoint_Boxed()
    {
        B.StringIntOrPoint union;

        for (int i = 0; i < Iterations; i++)
        {
            union = B.StringIntOrPoint.Create(new Point(1, 1));
        }
    }

    [Benchmark]
    public void Point_StringIntOrPoint_Hybrid()
    {
        H.StringIntOrPoint union;

        for (int i = 0; i < Iterations; i++)
        {
            union = H.StringIntOrPoint.Create(new Point(1, 1));
        }
    }
}
