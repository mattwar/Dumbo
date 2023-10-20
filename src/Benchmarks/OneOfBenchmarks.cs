using BenchmarkDotNet.Attributes;
using System.Drawing;
using B = Dumbo.TypeUnions.Existing.Boxed;
using F = Dumbo.TypeUnions.Existing.Fat;
using H = Dumbo.TypeUnions.Existing.Hybrid;

namespace Benchmarks;

[MemoryDiagnoser]
public class OneOfIntStringPointBenchmarks
{
    private readonly Point point = new Point(1, 1);

    [Benchmark]
    public void IntBoxed()
    {
        var union = B.OneOf<int, string, Point>.Create(1);
    }

    [Benchmark]
    public void IntFat()
    {
        var union = F.OneOf<int, string, Point>.Create(1);
    }

    [Benchmark]
    public void IntHybrid()
    {
        var union = H.OneOf<int, string, Point>.Create(1);
    }

    [Benchmark]
    public void StringBoxed()
    {
        var union = B.OneOf<string, int, Point>.Create("one");
    }

    [Benchmark]
    public void StringFat()
    {
        var union = F.OneOf<string, int, Point>.Create("one");
    }

    [Benchmark]
    public void StringHybrid()
    {
        var union = H.OneOf<string, int, Point>.Create("one");
    }

    [Benchmark]
    public void PointBoxed()
    {
        var union = B.OneOf<Point, int, string>.Create(point);
    }

    [Benchmark]
    public void PointFat()
    {
        var union = F.OneOf<Point, int, string>.Create(point);
    }

    [Benchmark]
    public void PointHybrid()
    {
        var union = H.OneOf<Point, int, string>.Create(point);
    }
}
