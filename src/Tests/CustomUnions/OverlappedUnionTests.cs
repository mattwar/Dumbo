using Dumbo;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tests.CustomUnions
{
    [TestClass]
    public class OverlappedUnionTests
    {
        public record struct Point(float x, float y);
        public record struct Circle(Point center, float radius, string name);
        public record struct Square(Point corner, float width, float height, string name);

        /// <summary>
        /// Custom union type wrapper, generates overlapping data
        /// </summary>
        public readonly struct Shape : ITypeUnion
        {
            public enum ShapeKind { Point = 1, Circle, Square };

            private struct CircleVals
            {
                public Point center;
                public float radius;
            }

            private struct CircleRefs
            {
                public string name;
            }

            private struct SquareVals
            {
                public Point corner;
                public float width;
                public float height;
            }

            private struct SquareRefs
            {
                public string name;
            }

            [StructLayout(LayoutKind.Explicit)]
            private struct OverlappedVals
            {
                [FieldOffset(0)]
                public Point point;

                [FieldOffset(0)]
                public CircleVals circle;

                [FieldOffset(0)]
                public SquareVals square;
            }

            [StructLayout(LayoutKind.Explicit)]
            private struct OverlappedRefs
            {
                [FieldOffset(0)]
                public CircleRefs circle;

                [FieldOffset(0)]
                public SquareRefs square;
            }

            private readonly ShapeKind _kind;
            private readonly OverlappedRefs _refs;
            private readonly OverlappedVals _vals;

            private Shape(ShapeKind kind, OverlappedRefs refs, OverlappedVals vals)
            {
                _kind = kind;
                _refs = refs;
                _vals = vals;
            }

            public static Shape Create(Point point) =>
                new Shape(
                    ShapeKind.Point,
                    default,
                    new OverlappedVals { point = point }
                    );

            public static Shape Create(Circle circle) =>
                new Shape(
                    ShapeKind.Circle,
                    new OverlappedRefs { circle = new CircleRefs { name = circle.name } },
                    new OverlappedVals { circle = new CircleVals { center = circle.center, radius = circle.radius } }
                    );

            public static Shape Create(Square square) =>
                new Shape(
                    ShapeKind.Square,
                    new OverlappedRefs { square = new SquareRefs { name = square.name } },
                    new OverlappedVals { square = new SquareVals { corner = square.corner, width = square.width, height = square.height } }
                    );

            public static Shape Point(float x, float y) => Create(new Point(x, y));
            public static Shape Circle(Point center, float radius, string name) => Create(new Circle(center, radius, name));
            public static Shape Square(Point corner, float width, float height, string name) => Create(new Square(corner, width, height, name));

            public ShapeKind Kind => _kind;
            public bool IsPoint => _kind == ShapeKind.Point;
            public bool IsCircle => _kind == ShapeKind.Circle;
            public bool IsSquare => _kind == ShapeKind.Square;

            public Point GetPoint() => _kind == ShapeKind.Point ? _vals.point : throw new InvalidCastException();
            public Circle GetCircle() => _kind == ShapeKind.Circle ? new Circle(_vals.circle.center, _vals.circle.radius, _refs.circle.name) : throw new InvalidCastException();
            public Square GetSquare() => _kind == ShapeKind.Square ? new Square(_vals.square.corner, _vals.square.width, _vals.square.height, _refs.square.name) : throw new InvalidCastException();

            public bool TryGetPoint(out Point point) => TryGet(out point);
            public bool TryGetCircle(out Circle circle) => TryGet(out circle);
            public bool TryGetSquare(out Square square) => TryGet(out square);

            public bool TryGet<T>([NotNullWhen(true)] out T value)
            {
                switch (_kind)
                {
                    case ShapeKind.Point when GetPoint() is T point:
                        value = point;
                        return true;
                    case ShapeKind.Circle when GetCircle() is T circle:
                        value = circle;
                        return true;
                    case ShapeKind.Square when GetSquare() is T square:
                        value = square;
                        return true;
                    default:
                        value = default!;
                        return false;
                }
            }

            public T Get<T>() => TryGet(out T value) ? value : throw new InvalidCastException();

            bool ITypeUnion.IsType<T>() => TryGet<T>(out _);

            public override string ToString()
            {
                switch (_kind)
                {
                    case ShapeKind.Point: return GetPoint().ToString();
                    case ShapeKind.Circle: return GetCircle().ToString();
                    case ShapeKind.Square: return GetSquare().ToString();
                    default: return "";
                }
            }
        }

    }
}
