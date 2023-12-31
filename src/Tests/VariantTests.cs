using Dumbo;
using System.Text;

namespace Tests;

[TestClass]
public class VariantTests
{
    [TestMethod]
    public void TestCreateAndAccess()
    {
        TestCreateAndAccess<bool>(true);
        TestCreateAndAccess<bool>(false);
        TestCreateAndAccess<bool?>(true);
        TestCreateAndAccess<bool?>(false);
        TestCreateAndAccess<bool?>(null);

        TestCreateAndAccess<sbyte>(1);
        TestCreateAndAccess<sbyte>(0);
        TestCreateAndAccess<sbyte>(sbyte.MinValue);
        TestCreateAndAccess<sbyte>(sbyte.MaxValue);
        TestCreateAndAccess<sbyte?>(1);
        TestCreateAndAccess<sbyte?>(null);

        TestCreateAndAccess<short>(1);
        TestCreateAndAccess<short>(0);
        TestCreateAndAccess<short>(short.MinValue);
        TestCreateAndAccess<short>(short.MaxValue);
        TestCreateAndAccess<short?>(1);
        TestCreateAndAccess<short?>(null);

        TestCreateAndAccess<int>(1);
        TestCreateAndAccess<int>(0);
        TestCreateAndAccess<int>(int.MinValue);
        TestCreateAndAccess<int>(int.MaxValue);
        TestCreateAndAccess<int?>(1);
        TestCreateAndAccess<int?>(null);

        TestCreateAndAccess<long>(1);
        TestCreateAndAccess<long>(0);
        TestCreateAndAccess<long>(long.MinValue);
        TestCreateAndAccess<long>(long.MaxValue);
        TestCreateAndAccess<long?>(1);
        TestCreateAndAccess<long?>(null);

        TestCreateAndAccess<byte>(1);
        TestCreateAndAccess<byte>(byte.MinValue);
        TestCreateAndAccess<byte>(byte.MaxValue);
        TestCreateAndAccess<byte?>(1);
        TestCreateAndAccess<byte?>(null);

        TestCreateAndAccess<ushort>(1);
        TestCreateAndAccess<ushort>(ushort.MinValue);
        TestCreateAndAccess<ushort>(ushort.MaxValue);
        TestCreateAndAccess<ushort?>(1);
        TestCreateAndAccess<ushort?>(null);

        TestCreateAndAccess<uint>(1);
        TestCreateAndAccess<uint>(uint.MinValue);
        TestCreateAndAccess<uint>(uint.MaxValue);
        TestCreateAndAccess<uint?>(1);
        TestCreateAndAccess<uint?>(null);

        TestCreateAndAccess<ulong>(1);
        TestCreateAndAccess<ulong>(ulong.MinValue);
        TestCreateAndAccess<ulong>(ulong.MaxValue);
        TestCreateAndAccess<ulong?>(1);
        TestCreateAndAccess<ulong?>(null);

        TestCreateAndAccess<float>(1.0f);
        TestCreateAndAccess<float>(0.0f);
        TestCreateAndAccess<float>(float.MinValue);
        TestCreateAndAccess<float>(float.MaxValue);
        TestCreateAndAccess<float>(float.Epsilon);
        TestCreateAndAccess<float>(float.PositiveInfinity);
        TestCreateAndAccess<float>(float.NegativeInfinity);
        TestCreateAndAccess<float>(float.NaN);
        TestCreateAndAccess<float?>(1.0f);
        TestCreateAndAccess<float?>(null);

        TestCreateAndAccess<double>(1.0);
        TestCreateAndAccess<double>(0.0);
        TestCreateAndAccess<double>(double.MinValue);
        TestCreateAndAccess<double>(double.MaxValue);
        TestCreateAndAccess<double>(double.Epsilon);
        TestCreateAndAccess<double>(double.PositiveInfinity);
        TestCreateAndAccess<double>(double.NegativeInfinity);
        TestCreateAndAccess<double>(double.NaN);
        TestCreateAndAccess<double?>(1.0);
        TestCreateAndAccess<double?>(null);

        TestCreateAndAccess<Decimal64>(Decimal64.Create(1.0m));
        TestCreateAndAccess<Decimal64>(Decimal64.Create(0.0m));
        TestCreateAndAccess<Decimal64>(Decimal64.MinValue);
        TestCreateAndAccess<Decimal64>(Decimal64.MaxValue);
        TestCreateAndAccess<Decimal64?>(Decimal64.Create(1.0m));
        TestCreateAndAccess<Decimal64?>(null);

        TestCreateAndAccess<decimal>(1.0m);
        TestCreateAndAccess<decimal>(0.0m);
        TestCreateAndAccess<decimal>(decimal.MinValue, isBoxed: true);
        TestCreateAndAccess<decimal>(decimal.MaxValue, isBoxed: true);
        TestCreateAndAccess<decimal?>(1.0m);
        TestCreateAndAccess<decimal?>(null);

        TestCreateAndAccess<char>('1');
        TestCreateAndAccess<char>('\0');
        TestCreateAndAccess<char>(char.MinValue);
        TestCreateAndAccess<char>(char.MaxValue);
        TestCreateAndAccess<char?>('1');
        TestCreateAndAccess<char?>(null);

        TestCreateAndAccess<Rune>(new Rune('1'));
        TestCreateAndAccess<Rune>(new Rune('\0'));
        TestCreateAndAccess<Rune?>(new Rune('1'));
        TestCreateAndAccess<Rune?>(null);

        TestCreateAndAccess<DateOnly>(new DateOnly(2002, 4, 15));
        TestCreateAndAccess<DateOnly>(DateOnly.MinValue);
        TestCreateAndAccess<DateOnly>(DateOnly.MaxValue);
        TestCreateAndAccess<DateOnly?>(new DateOnly(2002, 4, 15));
        TestCreateAndAccess<DateOnly?>(null);

        TestCreateAndAccess<TimeOnly>(new TimeOnly(10, 53));
        TestCreateAndAccess<TimeOnly>(TimeOnly.MinValue);
        TestCreateAndAccess<TimeOnly>(TimeOnly.MaxValue);
        TestCreateAndAccess<TimeOnly?>(new TimeOnly(10, 53));
        TestCreateAndAccess<TimeOnly?>(null);

        TestCreateAndAccess<DateTime>(DateTime.Now);
        TestCreateAndAccess<DateTime>(DateTime.MinValue);
        TestCreateAndAccess<DateTime>(DateTime.MaxValue);
        TestCreateAndAccess<DateTime?>(DateTime.Now);
        TestCreateAndAccess<DateTime?>(null);

        TestCreateAndAccess<TimeSpan>(TimeSpan.FromMinutes(53));
        TestCreateAndAccess<TimeSpan>(TimeSpan.MinValue);
        TestCreateAndAccess<TimeSpan>(TimeSpan.MaxValue);
        TestCreateAndAccess<TimeSpan?>(TimeSpan.FromMinutes(53));
        TestCreateAndAccess<TimeSpan?>(null);

        TestCreateAndAccess<string>("string");
        TestCreateAndAccess<string>("");
        TestCreateAndAccess<string?>("string");
        TestCreateAndAccess<string?>(null);

        TestCreateAndAccess<Guid>(Guid.NewGuid(), isBoxed: true);
        TestCreateAndAccess<Guid?>(Guid.NewGuid(), isBoxed: true);
        TestCreateAndAccess<Guid?>(null);

        TestCreateAndAccess<ReferenceType>(new ReferenceType("one", 1));
        TestCreateAndAccess<ReferenceType?>(new ReferenceType("one", 1));
        TestCreateAndAccess<ReferenceType?>(null);

        TestCreateAndAccess<WrapperStruct>(new WrapperStruct("one"));
        TestCreateAndAccess<WrapperStruct?>(new WrapperStruct("one"));
        TestCreateAndAccess<WrapperStruct?>(null);

        TestCreateAndAccess<SmallStructMixed>(new SmallStructMixed("one", 1), isBoxed: true);
        TestCreateAndAccess<SmallStructMixed?>(new SmallStructMixed("one", 1), isBoxed: true);
        TestCreateAndAccess<SmallStructMixed?>(null);

        TestCreateAndAccess<LargeStructMixed>(new LargeStructMixed("one", 1, "two", 2), isBoxed: true);
        TestCreateAndAccess<LargeStructMixed?>(new LargeStructMixed("one", 1, "two", 2), isBoxed: true);
        TestCreateAndAccess<LargeStructMixed?>(null);

        TestCreateAndAccess<SmallStructNoRefs>(new SmallStructNoRefs(1, 2));
        TestCreateAndAccess<SmallStructNoRefs?>(new SmallStructNoRefs(1, 2));
        TestCreateAndAccess<SmallStructNoRefs?>(null);

        TestCreateAndAccess<LargeStructNoRefs>(new LargeStructNoRefs(1, 2, 3, 4), isBoxed: true);
        TestCreateAndAccess<LargeStructNoRefs?>(new LargeStructNoRefs(1, 2, 3, 4), isBoxed: true);
        TestCreateAndAccess<LargeStructNoRefs?>(null);

        // enums
        TestCreateAndAccess(I8Enum.A);
        TestCreateAndAccess(I16Enum.A);
        TestCreateAndAccess(I32Enum.A);
        TestCreateAndAccess(I64Enum.A);
        TestCreateAndAccess(UI8Enum.A);
        TestCreateAndAccess(UI16Enum.A);
        TestCreateAndAccess(UI32Enum.A);
        TestCreateAndAccess(UI64Enum.A);
    }

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

    private void TestCreateAndAccess<T>(T value, bool isBoxed = false)
    {
        var v = Variant.Create(value);

        if (value == null)
        {
            Assert.AreEqual(typeof(object), v.Type, "Type");
            Assert.IsTrue(v.IsNull, "IsNull");
            Assert.AreEqual(isBoxed, v.IsBoxed, "IsBoxed");
            Assert.IsFalse(v.IsType<T>(), "IsType");
            Assert.IsFalse(v.TryGet<T>(out _), "TryGet");
            Assert.ThrowsException<InvalidCastException>(() => v.Get<T>());
            Assert.AreEqual(default, v.AsType<T>());
        }
        else
        {
            var nonNullT = GetNonNullableType(typeof(T));
            Assert.AreEqual(nonNullT, v.Type, "Type");
            Assert.IsFalse(v.IsNull, "IsNull");
            Assert.AreEqual(isBoxed, v.IsBoxed, "IsBoxed");
            Assert.IsTrue(v.IsType<T>(), "IsType");
            Assert.IsTrue(v.TryGet<T>(out var actualValue), "TryGet");
            Assert.AreEqual(value, v.Get<T>());
            Assert.AreEqual(value, v.AsType<T>());
        }
    }

    /// <summary>
    /// Returns true if the type is Nullable&lt;T&gt;
    /// </summary>
    private static bool IsNullableType(Type type) =>
        type.IsGenericType 
        && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    /// <summary>
    /// If the type is Nullable&lt;T&gt;, returns the type T,
    /// otherwise returns the type.
    /// </summary>
    private static Type GetNonNullableType(Type type) =>
        IsNullableType(type)
            ? type.GetGenericArguments()[0]
            : type;

    [TestMethod]
    public void TestIsType()
    {
        // non-nullable T
        TestIsType<sbyte>(1);
        TestIsType<short>(1);
        TestIsType<int>(1);
        TestIsType<long>(1);
        TestIsType<byte>(1);
        TestIsType<ushort>(1);
        TestIsType<uint>(1);
        TestIsType<ulong>(1);
        TestIsType<float>(1.0f);
        TestIsType<double>(1.0);
        TestIsType<Decimal64>(1);
        TestIsType<decimal>(1.0m);
        TestIsType<char>('1');
        TestIsType<Rune>(new Rune('1'));
        TestIsType<DateOnly>(new DateOnly(1, 2, 3));
        TestIsType<TimeOnly>(new TimeOnly(1, 2));
        TestIsType<DateTime>(DateTime.Now);
        TestIsType<TimeSpan>(TimeSpan.FromMinutes(1));
        TestIsType<Guid>(Guid.NewGuid());
        TestIsType<string>("string");
        TestIsType<ReferenceType>(new ReferenceType("one", 1));
        TestIsType<WrapperStruct>(new WrapperStruct("one"));
        TestIsType<SmallStructMixed>(new SmallStructMixed("one", 1));
        TestIsType<SmallStructNoRefs>(new SmallStructNoRefs(1, 2));
        TestIsType<LargeStructMixed>(new LargeStructMixed("one", 1, "two", 2));
        TestIsType<LargeStructNoRefs>(new LargeStructNoRefs(1, 2, 3, 4));

        // nullabe T with non-null value
        TestIsType<sbyte?>(1);
        TestIsType<short?>(1);
        TestIsType<int?>(1);
        TestIsType<long?>(1);
        TestIsType<byte?>(1);
        TestIsType<ushort?>(1);
        TestIsType<uint?>(1);
        TestIsType<ulong?>(1);
        TestIsType<float?>(1.0f);
        TestIsType<double?>(1.0);
        TestIsType<Decimal64?>(1);
        TestIsType<decimal?>(1.0m);
        TestIsType<char?>('1');
        TestIsType<Rune?>(new Rune('1'));
        TestIsType<DateOnly?>(new DateOnly(1, 2, 3));
        TestIsType<TimeOnly?>(new TimeOnly(1, 2));
        TestIsType<DateTime?>(DateTime.Now);
        TestIsType<TimeSpan?>(TimeSpan.FromMinutes(1));
        TestIsType<Guid?>(Guid.NewGuid());
        TestIsType<string?>("string");
        TestIsType<ReferenceType?>(new ReferenceType("one", 1));
        TestIsType<WrapperStruct?>(new WrapperStruct("one"));
        TestIsType<SmallStructMixed?>(new SmallStructMixed("one", 1));
        TestIsType<SmallStructNoRefs?>(new SmallStructNoRefs(1, 2));
        TestIsType<LargeStructMixed?>(new LargeStructMixed("one", 1, "two", 2));
        TestIsType<LargeStructNoRefs?>(new LargeStructNoRefs(1, 2, 3, 4));
    }

    private void TestIsType<T>(T value)
    {
        var v = Variant.Create(value);
        Assert.IsTrue(v.IsType<T>(), "IsType");
        Assert.IsTrue(v.IsType<object>(), "IsType<object>");
    }

    [TestMethod]
    public void TestAsTypeNull()
    {
        TestAsTypeNullStruct<sbyte>();
        TestAsTypeNullStruct<short>();
        TestAsTypeNullStruct<int>();
        TestAsTypeNullStruct<long>();
        TestAsTypeNullStruct<byte>();
        TestAsTypeNullStruct<ushort>();
        TestAsTypeNullStruct<uint>();
        TestAsTypeNullStruct<ulong>();
        TestAsTypeNullStruct<float>();
        TestAsTypeNullStruct<double>();
        TestAsTypeNullStruct<Decimal64>();
        TestAsTypeNullStruct<decimal>();
        TestAsTypeNullStruct<char>();
        TestAsTypeNullStruct<Rune>();
        TestAsTypeNullStruct<DateOnly>();
        TestAsTypeNullStruct<TimeOnly>();
        TestAsTypeNullStruct<DateTime>();
        TestAsTypeNullStruct<TimeSpan>();
        TestAsTypeNullStruct<Guid>();
        TestAsTypeNullStruct<WrapperStruct>();
        TestAsTypeNullStruct<SmallStructMixed>();
        TestAsTypeNullStruct<LargeStructMixed>();
        TestAsTypeNullStruct<SmallStructNoRefs>();
        TestAsTypeNullStruct<LargeStructNoRefs>();

        TestAsTypeNullRef<string>();
        TestAsTypeNullRef<ReferenceType>();
    }

    private void TestAsTypeNullStruct<T>()
        where T : struct
    {
        Assert.AreEqual((T?)null, Variant.Null.AsType<T?>());
    }

    private void TestAsTypeNullRef<T>()
        where T : class
    {
        Assert.AreEqual((T?)null, Variant.Null.AsType<T?>());
    }
}