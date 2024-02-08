using Dumbo;
using System.Text;

namespace Tests;

[TestClass]
public class VariantTests
{
    [TestMethod]
    public void TestGenericCreateAndAccess()
    {
        TestGenericCreateAndAccess<bool>(true);
        TestGenericCreateAndAccess<bool>(false);
        TestGenericCreateAndAccess<bool?>(true);
        TestGenericCreateAndAccess<bool?>(false);
        TestGenericCreateAndAccess<bool?>(null);

        TestGenericCreateAndAccess<sbyte>(1);
        TestGenericCreateAndAccess<sbyte>(0);
        TestGenericCreateAndAccess<sbyte>(sbyte.MinValue);
        TestGenericCreateAndAccess<sbyte>(sbyte.MaxValue);
        TestGenericCreateAndAccess<sbyte?>(1);
        TestGenericCreateAndAccess<sbyte?>(null);

        TestGenericCreateAndAccess<short>(1);
        TestGenericCreateAndAccess<short>(0);
        TestGenericCreateAndAccess<short>(short.MinValue);
        TestGenericCreateAndAccess<short>(short.MaxValue);
        TestGenericCreateAndAccess<short?>(1);
        TestGenericCreateAndAccess<short?>(null);

        TestGenericCreateAndAccess<int>(1);
        TestGenericCreateAndAccess<int>(0);
        TestGenericCreateAndAccess<int>(int.MinValue);
        TestGenericCreateAndAccess<int>(int.MaxValue);
        TestGenericCreateAndAccess<int?>(1);
        TestGenericCreateAndAccess<int?>(null);

        TestGenericCreateAndAccess<long>(1);
        TestGenericCreateAndAccess<long>(0);
        TestGenericCreateAndAccess<long>(long.MinValue);
        TestGenericCreateAndAccess<long>(long.MaxValue);
        TestGenericCreateAndAccess<long?>(1);
        TestGenericCreateAndAccess<long?>(null);

        TestGenericCreateAndAccess<byte>(1);
        TestGenericCreateAndAccess<byte>(byte.MinValue);
        TestGenericCreateAndAccess<byte>(byte.MaxValue);
        TestGenericCreateAndAccess<byte?>(1);
        TestGenericCreateAndAccess<byte?>(null);

        TestGenericCreateAndAccess<ushort>(1);
        TestGenericCreateAndAccess<ushort>(ushort.MinValue);
        TestGenericCreateAndAccess<ushort>(ushort.MaxValue);
        TestGenericCreateAndAccess<ushort?>(1);
        TestGenericCreateAndAccess<ushort?>(null);

        TestGenericCreateAndAccess<uint>(1);
        TestGenericCreateAndAccess<uint>(uint.MinValue);
        TestGenericCreateAndAccess<uint>(uint.MaxValue);
        TestGenericCreateAndAccess<uint?>(1);
        TestGenericCreateAndAccess<uint?>(null);

        TestGenericCreateAndAccess<ulong>(1);
        TestGenericCreateAndAccess<ulong>(ulong.MinValue);
        TestGenericCreateAndAccess<ulong>(ulong.MaxValue);
        TestGenericCreateAndAccess<ulong?>(1);
        TestGenericCreateAndAccess<ulong?>(null);

        TestGenericCreateAndAccess<float>(1.0f);
        TestGenericCreateAndAccess<float>(0.0f);
        TestGenericCreateAndAccess<float>(float.MinValue);
        TestGenericCreateAndAccess<float>(float.MaxValue);
        TestGenericCreateAndAccess<float>(float.Epsilon);
        TestGenericCreateAndAccess<float>(float.PositiveInfinity);
        TestGenericCreateAndAccess<float>(float.NegativeInfinity);
        TestGenericCreateAndAccess<float>(float.NaN);
        TestGenericCreateAndAccess<float?>(1.0f);
        TestGenericCreateAndAccess<float?>(null);

        TestGenericCreateAndAccess<double>(1.0);
        TestGenericCreateAndAccess<double>(0.0);
        TestGenericCreateAndAccess<double>(double.MinValue);
        TestGenericCreateAndAccess<double>(double.MaxValue);
        TestGenericCreateAndAccess<double>(double.Epsilon);
        TestGenericCreateAndAccess<double>(double.PositiveInfinity);
        TestGenericCreateAndAccess<double>(double.NegativeInfinity);
        TestGenericCreateAndAccess<double>(double.NaN);
        TestGenericCreateAndAccess<double?>(1.0);
        TestGenericCreateAndAccess<double?>(null);

        TestGenericCreateAndAccess<Decimal64>(Decimal64.Create(1.0m));
        TestGenericCreateAndAccess<Decimal64>(Decimal64.Create(0.0m));
        TestGenericCreateAndAccess<Decimal64>(Decimal64.MinValue);
        TestGenericCreateAndAccess<Decimal64>(Decimal64.MaxValue);
        TestGenericCreateAndAccess<Decimal64?>(Decimal64.Create(1.0m));
        TestGenericCreateAndAccess<Decimal64?>(null);

        TestGenericCreateAndAccess<decimal>(1.0m);
        TestGenericCreateAndAccess<decimal>(0.0m);
        TestGenericCreateAndAccess<decimal>(decimal.MinValue, isBoxed: true);
        TestGenericCreateAndAccess<decimal>(decimal.MaxValue, isBoxed: true);
        TestGenericCreateAndAccess<decimal?>(1.0m);
        TestGenericCreateAndAccess<decimal?>(null);

        TestGenericCreateAndAccess<char>('1');
        TestGenericCreateAndAccess<char>('\0');
        TestGenericCreateAndAccess<char>(char.MinValue);
        TestGenericCreateAndAccess<char>(char.MaxValue);
        TestGenericCreateAndAccess<char?>('1');
        TestGenericCreateAndAccess<char?>(null);

        TestGenericCreateAndAccess<Rune>(new Rune('1'));
        TestGenericCreateAndAccess<Rune>(new Rune('\0'));
        TestGenericCreateAndAccess<Rune?>(new Rune('1'));
        TestGenericCreateAndAccess<Rune?>(null);

        TestGenericCreateAndAccess<DateOnly>(new DateOnly(2002, 4, 15));
        TestGenericCreateAndAccess<DateOnly>(DateOnly.MinValue);
        TestGenericCreateAndAccess<DateOnly>(DateOnly.MaxValue);
        TestGenericCreateAndAccess<DateOnly?>(new DateOnly(2002, 4, 15));
        TestGenericCreateAndAccess<DateOnly?>(null);

        TestGenericCreateAndAccess<TimeOnly>(new TimeOnly(10, 53));
        TestGenericCreateAndAccess<TimeOnly>(TimeOnly.MinValue);
        TestGenericCreateAndAccess<TimeOnly>(TimeOnly.MaxValue);
        TestGenericCreateAndAccess<TimeOnly?>(new TimeOnly(10, 53));
        TestGenericCreateAndAccess<TimeOnly?>(null);

        TestGenericCreateAndAccess<DateTime>(DateTime.Now);
        TestGenericCreateAndAccess<DateTime>(DateTime.MinValue);
        TestGenericCreateAndAccess<DateTime>(DateTime.MaxValue);
        TestGenericCreateAndAccess<DateTime?>(DateTime.Now);
        TestGenericCreateAndAccess<DateTime?>(null);

        TestGenericCreateAndAccess<TimeSpan>(TimeSpan.FromMinutes(53));
        TestGenericCreateAndAccess<TimeSpan>(TimeSpan.MinValue);
        TestGenericCreateAndAccess<TimeSpan>(TimeSpan.MaxValue);
        TestGenericCreateAndAccess<TimeSpan?>(TimeSpan.FromMinutes(53));
        TestGenericCreateAndAccess<TimeSpan?>(null);

        TestGenericCreateAndAccess<string>("string");
        TestGenericCreateAndAccess<string>("");
        TestGenericCreateAndAccess<string?>("string");
        TestGenericCreateAndAccess<string?>(null);

        TestGenericCreateAndAccess<Guid>(Guid.NewGuid(), isBoxed: true);
        TestGenericCreateAndAccess<Guid?>(Guid.NewGuid(), isBoxed: true);
        TestGenericCreateAndAccess<Guid?>(null);

        TestGenericCreateAndAccess<ReferenceType>(new ReferenceType("one", 1));
        TestGenericCreateAndAccess<ReferenceType?>(new ReferenceType("one", 1));
        TestGenericCreateAndAccess<ReferenceType?>(null);

        TestGenericCreateAndAccess<WrapperStruct>(new WrapperStruct("one"));
        TestGenericCreateAndAccess<WrapperStruct?>(new WrapperStruct("one"));
        TestGenericCreateAndAccess<WrapperStruct?>(null);

        TestGenericCreateAndAccess<SmallStructMixed>(new SmallStructMixed("one", 1), isBoxed: true);
        TestGenericCreateAndAccess<SmallStructMixed?>(new SmallStructMixed("one", 1), isBoxed: true);
        TestGenericCreateAndAccess<SmallStructMixed?>(null);

        TestGenericCreateAndAccess<LargeStructMixed>(new LargeStructMixed("one", 1, "two", 2), isBoxed: true);
        TestGenericCreateAndAccess<LargeStructMixed?>(new LargeStructMixed("one", 1, "two", 2), isBoxed: true);
        TestGenericCreateAndAccess<LargeStructMixed?>(null);

        TestGenericCreateAndAccess<SmallStructNoRefs>(new SmallStructNoRefs(1, 2));
        TestGenericCreateAndAccess<SmallStructNoRefs?>(new SmallStructNoRefs(1, 2));
        TestGenericCreateAndAccess<SmallStructNoRefs?>(null);

        TestGenericCreateAndAccess<LargeStructNoRefs>(new LargeStructNoRefs(1, 2, 3, 4), isBoxed: true);
        TestGenericCreateAndAccess<LargeStructNoRefs?>(new LargeStructNoRefs(1, 2, 3, 4), isBoxed: true);
        TestGenericCreateAndAccess<LargeStructNoRefs?>(null);

        // enums
        TestGenericCreateAndAccess(I8Enum.A);
        TestGenericCreateAndAccess(I16Enum.A);
        TestGenericCreateAndAccess(I32Enum.A);
        TestGenericCreateAndAccess(I64Enum.A);
        TestGenericCreateAndAccess(UI8Enum.A);
        TestGenericCreateAndAccess(UI16Enum.A);
        TestGenericCreateAndAccess(UI32Enum.A);
        TestGenericCreateAndAccess(UI64Enum.A);
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

    private void TestGenericCreateAndAccess<T>(T value, bool isBoxed = false)
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
    public void TestNonGenericAPI()
    {
        Variant v = Variant.Create(10);
        var ival = v.GetInt32();
        Assert.AreEqual(10, ival);

        var ival2 = v.Get<int>();
        Assert.AreEqual(ival, ival2);

        var v2 = Variant.Create<int>(10);
        var ival3 = v2.GetInt32();
        Assert.AreEqual(ival, ival3);
    }

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