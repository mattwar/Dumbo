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

        TestCreateAndAccess<Decimal64>(Decimal64.Convert(1.0m));
        TestCreateAndAccess<Decimal64>(Decimal64.Convert(0.0m));
        TestCreateAndAccess<Decimal64>(Decimal64.MinValue);
        TestCreateAndAccess<Decimal64>(Decimal64.MaxValue);
        TestCreateAndAccess<Decimal64?>(Decimal64.Convert(1.0m));
        TestCreateAndAccess<Decimal64?>(null);

        TestCreateAndAccess<decimal>(1.0m);
        TestCreateAndAccess<decimal>(0.0m);
        TestCreateAndAccess<decimal>(decimal.MinValue);
        TestCreateAndAccess<decimal>(decimal.MaxValue);
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

        TestCreateAndAccess<Guid>(Guid.NewGuid());
        TestCreateAndAccess<Guid?>(Guid.NewGuid());
        TestCreateAndAccess<Guid?>(null);

        TestCreateAndAccess<SomeRecord>(new SomeRecord("one", 1));
        TestCreateAndAccess<SomeRecord?>(new SomeRecord("one", 1));
        TestCreateAndAccess<SomeRecord?>(null);

        TestCreateAndAccess<SomeRecordStruct>(new SomeRecordStruct("one", 1));
        TestCreateAndAccess<SomeRecordStruct?>(new SomeRecordStruct("one", 1));
        TestCreateAndAccess<SomeRecordStruct?>(null);
    }

    private record SomeRecord(string a, int b);
    private record struct SomeRecordStruct(string a, int b);

    private enum I8Enum : sbyte { A = 1, B, C };
    private enum I16Enum : short { A = 1, B, C };
    private enum I32Enum : int { A = 1, B, C };
    private enum I64Enum : long { A = 1, B, C };
    private enum UI8Enum : byte { A = 1, B, C };
    private enum UI16Enum : ushort { A = 1, B, C };
    private enum UI32Enum : uint { A = 1, B, C };
    private enum UI64Enum : ulong { A = 1, B, C };

    [TestMethod]
    public void TestEnums()
    {
        TestCreateAndAccess(I8Enum.A);
        TestCreateAndAccess(I16Enum.A);
        TestCreateAndAccess(I32Enum.A);
        TestCreateAndAccess(I64Enum.A);
        TestCreateAndAccess(UI8Enum.A);
        TestCreateAndAccess(UI16Enum.A);
        TestCreateAndAccess(UI32Enum.A);
        TestCreateAndAccess(UI64Enum.A);
    }

    private void TestCreateAndAccess<T>(T value)
    {
        Console.WriteLine($"TestCreateAndAccess: {typeof(T).Name}: {value}");

        Console.WriteLine("\tCreate");
        var v = Variant.Create(value);

        if (value == null)
        {
            Console.WriteLine("\tType");
            Assert.IsNull(v.Type, "Type");
            Console.WriteLine("\tIsNull (true)");
            Assert.IsTrue(v.IsNull, "IsNull");
            Console.WriteLine("\tIsType");
            Assert.IsFalse(v.IsType<T>(), "IsType");
            Console.WriteLine("\tTryGet");
            Assert.IsFalse(v.TryGet<T>(out _), "TryGet");
            Console.WriteLine("\tGet");
            Assert.ThrowsException<InvalidCastException>(() => v.Get<T>());
            Console.WriteLine("\tAsType");
            Assert.AreEqual(default, v.AsType<T>());
        }
        else
        {
            var nonNullT = GetNonNullableType(typeof(T));
            Console.WriteLine("\tType");
            Assert.AreEqual(nonNullT, v.Type, "Type");
            Console.WriteLine("IsNull (false)");
            Assert.IsFalse(v.IsNull, "IsNull");
            Console.WriteLine("\tIsType");
            Assert.IsTrue(v.IsType<T>(), "IsType");
            Console.WriteLine("\tTryGet");
            Assert.IsTrue(v.TryGet<T>(out var actualValue), "TryGet");
            Assert.AreEqual(value, actualValue, "value");
            Console.WriteLine("\tGet");
            Assert.AreEqual(value, v.Get<T>());
            Console.WriteLine("\tAsType");
            Assert.AreEqual(value, v.AsType<T>());
        }
    }

    private static Type GetNonNullableType(Type type)
    {
        if (type.IsValueType
            && type.IsGenericType
            && type.GetGenericTypeDefinition() is Type genericTypeDef
            && genericTypeDef == typeof(Nullable<>))
        {
            return type.GetGenericArguments()[0];
        }

        return type;
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
        TestIsType<SomeRecordStruct>(new SomeRecordStruct("one", 1));
        TestIsType<string>("string");
        TestIsType<SomeRecord>(new SomeRecord("one", 1));

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
        TestIsType<SomeRecordStruct?>(new SomeRecordStruct("one", 1));
        TestIsType<string?>("string");
        TestIsType<SomeRecord?>(new SomeRecord("one", 1));
    }

    private void TestIsType<T>(T value)
    {
        var v = Variant.Create(value);
        Assert.IsTrue(v.IsType<T>(), "IsType");
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
        TestAsTypeNullStruct<SomeRecordStruct>();

        TestAsTypeNullRef<string>();
        TestAsTypeNullRef<SomeRecord>();
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