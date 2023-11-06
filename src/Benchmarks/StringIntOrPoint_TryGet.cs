using BenchmarkDotNet.Attributes;
using System.Drawing;
using B = Dumbo.TypeUnions.Boxed;
using F = Dumbo.TypeUnions.Fat;
using H = Dumbo.TypeUnions.Hybrid;
using EB = Dumbo.TypeUnions.Existing.Boxed;
using EF = Dumbo.TypeUnions.Existing.Fat;
using EH = Dumbo.TypeUnions.Existing.Hybrid;

namespace Benchmarks;

public class StringIntOrPoint_TryGet
{
    private static readonly EB.OneOf<int, string, Point> OneOf_Boxed_String =
        EB.OneOf<int, string, Point>.Create("one");

    private static readonly EF.OneOf<int, string, Point> OneOf_Fat_String =
        EF.OneOf<int, string, Point>.Create("one");

    private static readonly EH.OneOf<int, string, Point> OneOf_Hybrid_String =
        EH.OneOf<int, string, Point>.Create("one");

    private static readonly B.StringIntOrPoint StringIntOrPoint_Boxed_String =
        B.StringIntOrPoint.Create("one");

    private static readonly H.StringIntOrPoint StringIntOrPoint_Hybrid_String =
        H.StringIntOrPoint.Create("one");

    private static readonly F.StringIntOrPoint StringIntOrPoint_Fat_String =
        F.StringIntOrPoint.Create("one");


    private static readonly EB.OneOf<int, string, Point> OneOf_Boxed_Int =
        EB.OneOf<int, string, Point>.Create(1);

    private static readonly EF.OneOf<int, string, Point> OneOf_Fat_Int =
        EF.OneOf<int, string, Point>.Create(1);

    private static readonly EH.OneOf<int, string, Point> OneOf_Hybrid_Int =
        EH.OneOf<int, string, Point>.Create(1);

    private static readonly B.StringIntOrPoint StringIntOrPoint_Boxed_Int =
        B.StringIntOrPoint.Create(1);

    private static readonly H.StringIntOrPoint StringIntOrPoint_Hybrid_Int =
        H.StringIntOrPoint.Create(1);

    private static readonly F.StringIntOrPoint StringIntOrPoint_Fat_Int =
        F.StringIntOrPoint.Create(1);


    private static readonly EB.OneOf<int, string, Point> OneOf_Boxed_Point =
        EB.OneOf<int, string, Point>.Create(new Point(1, 1));

    private static readonly EF.OneOf<int, string, Point> OneOf_Fat_Point =
        EF.OneOf<int, string, Point>.Create(new Point(1, 1));

    private static readonly EH.OneOf<int, string, Point> OneOf_Hybrid_Point =
        EH.OneOf<int, string, Point>.Create(new Point(1, 1));

    private static readonly B.StringIntOrPoint StringIntOrPoint_Boxed_Point =
        B.StringIntOrPoint.Create(new Point(1, 1));

    private static readonly H.StringIntOrPoint StringIntOrPoint_Hybrid_Point =
        H.StringIntOrPoint.Create(new Point(1, 1));

    private static readonly F.StringIntOrPoint StringIntOrPoint_Fat_Point =
        F.StringIntOrPoint.Create(new Point(1, 1));


    [Params(1000)]
    public int Iterations { get; set; }

    [Benchmark]
    public void String_OneOf_Boxed()
    {
        for (int i = 0; i < Iterations; i++)
        {
            OneOf_Boxed_String.TryGet(out string value);
        }
    }

    [Benchmark]
    public void String_OneOf_Fat()
    {
        for (int i = 0; i < Iterations; i++)
        {
            OneOf_Fat_String.TryGet(out string value);
        }
    }

    [Benchmark]
    public void String_OneOf_Hybrid()
    {
        for (int i = 0; i < Iterations; i++)
        {
            OneOf_Hybrid_String.TryGet(out string value);
        }
    }

    [Benchmark]
    public void String_StringIntOrPoint_Fat()
    {
        for (int i = 0; i < Iterations; i++)
        {
            StringIntOrPoint_Fat_String.TryGet(out string value);
        }
    }

    [Benchmark]
    public void String_StringIntOrPoint_Boxed()
    {
        for (int i = 0; i < Iterations; i++)
        {
            StringIntOrPoint_Boxed_String.TryGet(out string value);
        }
    }

    [Benchmark]
    public void String_StringIntOrPoint_Hybrid()
    {
        for (int i = 0; i < Iterations; i++)
        {
            StringIntOrPoint_Hybrid_String.TryGet(out string value);
        }
    }


    [Benchmark]
    public void Int_OneOf_Boxed()
    {
        for (int i = 0; i < Iterations; i++)
        {
            OneOf_Boxed_Int.TryGet(out int value);
        }
    }

    [Benchmark]
    public void Int_OneOf_Fat()
    {
        for (int i = 0; i < Iterations; i++)
        {
            OneOf_Fat_Int.TryGet(out int value);
        }
    }

    [Benchmark]
    public void Int_OneOf_Hybrid()
    {
        for (int i = 0; i < Iterations; i++)
        {
            OneOf_Hybrid_Int.TryGet(out int value);
        }
    }

    [Benchmark]
    public void Int_StringIntOrPoint_Fat()
    {
        for (int i = 0; i < Iterations; i++)
        {
            StringIntOrPoint_Fat_Int.TryGet(out int value);
        }
    }

    [Benchmark]
    public void Int_StringIntOrPoint_Boxed()
    {
        for (int i = 0; i < Iterations; i++)
        {
            StringIntOrPoint_Boxed_Int.TryGet(out int value);
        }
    }

    [Benchmark]
    public void Int_StringIntOrPoint_Hybrid()
    {
        for (int i = 0; i < Iterations; i++)
        {
            StringIntOrPoint_Hybrid_Int.TryGet(out int value);
        }
    }


    [Benchmark]
    public void Point_OneOf_Boxed()
    {
        for (int i = 0; i < Iterations; i++)
        {
            OneOf_Boxed_Point.TryGet(out Point value);
        }
    }

    [Benchmark]
    public void Point_OneOf_Fat()
    {
        for (int i = 0; i < Iterations; i++)
        {
            OneOf_Fat_Point.TryGet(out Point value);
        }
    }

    [Benchmark]
    public void Point_OneOf_Hybrid()
    {
        for (int i = 0; i < Iterations; i++)
        {
            OneOf_Hybrid_Point.TryGet(out Point value);
        }
    }

    [Benchmark]
    public void Point_StringIntOrPoint_Fat()
    {
        for (int i = 0; i < Iterations; i++)
        {
            StringIntOrPoint_Fat_Point.TryGet(out Point value);
        }
    }

    [Benchmark]
    public void Point_StringIntOrPoint_Boxed()
    {
        for (int i = 0; i < Iterations; i++)
        {
            StringIntOrPoint_Boxed_Point.TryGet(out Point value);
        }
    }

    [Benchmark]
    public void Point_StringIntOrPoint_Hybrid()
    {
        for (int i = 0; i < Iterations; i++)
        {
            StringIntOrPoint_Hybrid_Point.TryGet(out Point value);
        }
    }
}
