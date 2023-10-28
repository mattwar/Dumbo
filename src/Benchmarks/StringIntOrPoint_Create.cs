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
    public void String_OneOf_Boxed()
    {
        var union = EB.OneOf<int, string, Point>.Create("one");
    }

    [Benchmark]
    public void String_OneOf_Fat()
    {
        var union = EF.OneOf<int, string, Point>.Create("one");
    }

    [Benchmark]
    public void String_OneOf_Hybrid()
    {
        var union = EH.OneOf<int, string, Point>.Create("one");
    }

    [Benchmark]
    public void String_StringIntOrPoint_Fat()
    {
        var union = F.StringIntOrPoint.Create("one");
    }

    [Benchmark]
    public void String_StringIntOrPoint_Boxed()
    {
        var union = B.StringIntOrPoint.Create("one");
    }

    [Benchmark]
    public void String_StringIntOrPoint_Hybrid()
    {
        var union = H.StringIntOrPoint.Create("one");
    }


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
        var union = F.StringIntOrPoint.Create(1);
    }

    [Benchmark]
    public void Int_StringIntOrPoint_Boxed()
    {
        var union = B.StringIntOrPoint.Create(1);
    }

    [Benchmark]
    public void Int_StringIntOrPoint_Hybrid()
    {
        var union = H.StringIntOrPoint.Create(1);
    }


    [Benchmark]
    public void Point_OneOf_Boxed()
    {
        var union = EB.OneOf<int, string, Point>.Create(new Point(1, 1));
    }

    [Benchmark]
    public void Point_OneOf_Fat()
    {
        var union = EF.OneOf<int, string, Point>.Create(new Point(1, 1));
    }

    [Benchmark]
    public void Point_OneOf_Hybrid()
    {
        var union = EH.OneOf<int, string, Point>.Create(new Point(1, 1));
    }

    [Benchmark]
    public void Point_StringIntOrPoint_Fat()
    {
        var union = F.StringIntOrPoint.Create(new Point(1, 1));
    }

    [Benchmark]
    public void Point_StringIntOrPoint_Boxed()
    {
        var union = B.StringIntOrPoint.Create(new Point(1, 1));
    }

    [Benchmark]
    public void Point_StringIntOrPoint_Hybrid()
    {
        var union = H.StringIntOrPoint.Create(new Point(1, 1));
    }
}
