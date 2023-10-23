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
    [Benchmark]
    public void Int_OneOf_Boxed()
    {
        var union = EB.OneOf<int, string, Point>.Create(1);
    }

    [Benchmark]
    public void Int_OneOf_Fat()
    {
        var union = EF.OneOf<int, string, Point>.Create(1);
    }

    [Benchmark]
    public void Int_OneOf_Hybrid()
    {
        var union = EH.OneOf<int, string, Point>.Create(1);
    }

    [Benchmark]
    public void Int_StringIntOrPoint_Fat()
    {
        var unin = F.StringIntOrPoint.Create(1);
    }

    [Benchmark]
    public void Int_StringIntOrPoint_Boxed()
    {
        var unin = B.StringIntOrPoint.Create(1);
    }

    [Benchmark]
    public void Int_StringIntOrPoint_Hybrid()
    {
        var unin = H.StringIntOrPoint.Create(1);
    }

#if false
    [Benchmark]
    public void String_OneOf_Boxed()
    {
        var union = EB.OneOf<string, int, Point>.Create("one");
    }

    [Benchmark]
    public void String_OneOf_Fat()
    {
        var union = EF.OneOf<string, int, Point>.Create("one");
    }

    [Benchmark]
    public void String_OneOf_Hybrid()
    {
        var union = EH.OneOf<string, int, Point>.Create("one");
    }

    [Benchmark]
    public void Point_OneOf_Boxed()
    {
        var union = EB.OneOf<Point, int, string>.Create(point);
    }

    [Benchmark]
    public void Point_OneOf_Fat()
    {
        var union = EF.OneOf<Point, int, string>.Create(point);
    }

    [Benchmark]
    public void Point_OneOf_Hybrid()
    {
        var union = EH.OneOf<Point, int, string>.Create(point);
    }
#endif
}


public class StringIntOrPoint_IsType
{
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

    [Benchmark]
    public void Int_OneOf_Boxed()
    {
        OneOf_Boxed_Int.IsType<int>();
    }

    [Benchmark]
    public void Int_OneOf_Fat()
    {
        OneOf_Fat_Int.IsType<int>();
    }

    [Benchmark]
    public void Int_OneOf_Hybrid()
    {
        OneOf_Hybrid_Int.IsType<int>();
    }

    [Benchmark]
    public void Int_StringIntOrPoint_Fat()
    {
        StringIntOrPoint_Fat_Int.IsType<int>();
    }

    [Benchmark]
    public void Int_StringIntOrPoint_Boxed()
    {
        StringIntOrPoint_Boxed_Int.IsType<int>();
    }

    [Benchmark]
    public void Int_StringIntOrPoint_Hybrid()
    {
        StringIntOrPoint_Hybrid_Int.IsType<int>();
    }
}
