using Dumbo;
using System.Text;

namespace Tests
{
    [TestClass]
    public class VariantTests
    {
        [TestMethod]
        public void TestCreateAndAccess()
        {
            TestCreateAndAccess<bool>(true, Variant.VariantKind.Bool);
            TestCreateAndAccess<bool>(false, Variant.VariantKind.Bool);
            TestCreateAndAccess<bool?>(true, Variant.VariantKind.Bool);
            TestCreateAndAccess<bool?>(false, Variant.VariantKind.Bool);
            TestCreateAndAccess<bool?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<sbyte>(1, Variant.VariantKind.Int8);
            TestCreateAndAccess<sbyte>(0, Variant.VariantKind.Int8);
            TestCreateAndAccess<sbyte>(sbyte.MinValue, Variant.VariantKind.Int8);
            TestCreateAndAccess<sbyte>(sbyte.MaxValue, Variant.VariantKind.Int8);
            TestCreateAndAccess<sbyte?>(1, Variant.VariantKind.Int8);
            TestCreateAndAccess<sbyte?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<short>(1, Variant.VariantKind.Int16);
            TestCreateAndAccess<short>(0, Variant.VariantKind.Int16);
            TestCreateAndAccess<short>(short.MinValue, Variant.VariantKind.Int16);
            TestCreateAndAccess<short>(short.MaxValue, Variant.VariantKind.Int16);
            TestCreateAndAccess<short?>(1, Variant.VariantKind.Int16);
            TestCreateAndAccess<short?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<int>(1, Variant.VariantKind.Int32);
            TestCreateAndAccess<int>(0, Variant.VariantKind.Int32);
            TestCreateAndAccess<int>(int.MinValue, Variant.VariantKind.Int32);
            TestCreateAndAccess<int>(int.MaxValue, Variant.VariantKind.Int32);
            TestCreateAndAccess<int?>(1, Variant.VariantKind.Int32);
            TestCreateAndAccess<int?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<long>(1, Variant.VariantKind.Int64);
            TestCreateAndAccess<long>(0, Variant.VariantKind.Int64);
            TestCreateAndAccess<long>(long.MinValue, Variant.VariantKind.Int64);
            TestCreateAndAccess<long>(long.MaxValue, Variant.VariantKind.Int64);
            TestCreateAndAccess<long?>(1, Variant.VariantKind.Int64);
            TestCreateAndAccess<long?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<byte>(1, Variant.VariantKind.UInt8);
            TestCreateAndAccess<byte>(byte.MinValue, Variant.VariantKind.UInt8);
            TestCreateAndAccess<byte>(byte.MaxValue, Variant.VariantKind.UInt8);
            TestCreateAndAccess<byte?>(1, Variant.VariantKind.UInt8);
            TestCreateAndAccess<byte?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<ushort>(1, Variant.VariantKind.UInt16);
            TestCreateAndAccess<ushort>(ushort.MinValue, Variant.VariantKind.UInt16);
            TestCreateAndAccess<ushort>(ushort.MaxValue, Variant.VariantKind.UInt16);
            TestCreateAndAccess<ushort?>(1, Variant.VariantKind.UInt16);
            TestCreateAndAccess<ushort?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<uint>(1, Variant.VariantKind.UInt32);
            TestCreateAndAccess<uint>(uint.MinValue, Variant.VariantKind.UInt32);
            TestCreateAndAccess<uint>(uint.MaxValue, Variant.VariantKind.UInt32);
            TestCreateAndAccess<uint?>(1, Variant.VariantKind.UInt32);
            TestCreateAndAccess<uint?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<ulong>(1, Variant.VariantKind.UInt64);
            TestCreateAndAccess<ulong>(ulong.MinValue, Variant.VariantKind.UInt64);
            TestCreateAndAccess<ulong>(ulong.MaxValue, Variant.VariantKind.UInt64);
            TestCreateAndAccess<ulong?>(1, Variant.VariantKind.UInt64);
            TestCreateAndAccess<ulong?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<float>(1.0f, Variant.VariantKind.Float32);
            TestCreateAndAccess<float>(0.0f, Variant.VariantKind.Float32);
            TestCreateAndAccess<float>(float.MinValue, Variant.VariantKind.Float32);
            TestCreateAndAccess<float>(float.MaxValue, Variant.VariantKind.Float32);
            TestCreateAndAccess<float>(float.Epsilon, Variant.VariantKind.Float32);
            TestCreateAndAccess<float>(float.PositiveInfinity, Variant.VariantKind.Float32);
            TestCreateAndAccess<float>(float.NegativeInfinity, Variant.VariantKind.Float32);
            TestCreateAndAccess<float>(float.NaN, Variant.VariantKind.Float32);
            TestCreateAndAccess<float?>(1.0f, Variant.VariantKind.Float32);
            TestCreateAndAccess<float?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<double>(1.0, Variant.VariantKind.Float64);
            TestCreateAndAccess<double>(0.0, Variant.VariantKind.Float64);
            TestCreateAndAccess<double>(double.MinValue, Variant.VariantKind.Float64);
            TestCreateAndAccess<double>(double.MaxValue, Variant.VariantKind.Float64);
            TestCreateAndAccess<double>(double.Epsilon, Variant.VariantKind.Float64);
            TestCreateAndAccess<double>(double.PositiveInfinity, Variant.VariantKind.Float64);
            TestCreateAndAccess<double>(double.NegativeInfinity, Variant.VariantKind.Float64);
            TestCreateAndAccess<double>(double.NaN, Variant.VariantKind.Float64);
            TestCreateAndAccess<double?>(1.0, Variant.VariantKind.Float64);
            TestCreateAndAccess<double?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<Decimal64>(Decimal64.Convert(1.0m), Variant.VariantKind.Decimal64);
            TestCreateAndAccess<Decimal64>(Decimal64.Convert(0.0m), Variant.VariantKind.Decimal64);
            TestCreateAndAccess<Decimal64>(Decimal64.MinValue, Variant.VariantKind.Decimal64);
            TestCreateAndAccess<Decimal64>(Decimal64.MaxValue, Variant.VariantKind.Decimal64);
            TestCreateAndAccess<Decimal64?>(Decimal64.Convert(1.0m), Variant.VariantKind.Decimal64);
            TestCreateAndAccess<Decimal64?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<decimal>(1.0m, Variant.VariantKind.Decimal128);
            TestCreateAndAccess<decimal>(0.0m, Variant.VariantKind.Decimal128);
            TestCreateAndAccess<decimal>(decimal.MinValue, Variant.VariantKind.Object);
            TestCreateAndAccess<decimal>(decimal.MaxValue, Variant.VariantKind.Object);
            TestCreateAndAccess<decimal?>(1.0m, Variant.VariantKind.Decimal128);
            TestCreateAndAccess<decimal?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<char>('1', Variant.VariantKind.Char16);
            TestCreateAndAccess<char>('\0', Variant.VariantKind.Char16);
            TestCreateAndAccess<char>(char.MinValue, Variant.VariantKind.Char16);
            TestCreateAndAccess<char>(char.MaxValue, Variant.VariantKind.Char16);
            TestCreateAndAccess<char?>('1', Variant.VariantKind.Char16);
            TestCreateAndAccess<char?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<Rune>(new Rune('1'), Variant.VariantKind.Char32);
            TestCreateAndAccess<Rune>(new Rune('\0'), Variant.VariantKind.Char32);
            TestCreateAndAccess<Rune?>(new Rune('1'), Variant.VariantKind.Char32);
            TestCreateAndAccess<Rune?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<DateOnly>(new DateOnly(2002, 4, 15), Variant.VariantKind.DateOnly);
            TestCreateAndAccess<DateOnly>(DateOnly.MinValue, Variant.VariantKind.DateOnly);
            TestCreateAndAccess<DateOnly>(DateOnly.MaxValue, Variant.VariantKind.DateOnly);
            TestCreateAndAccess<DateOnly?>(new DateOnly(2002, 4, 15), Variant.VariantKind.DateOnly);
            TestCreateAndAccess<DateOnly?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<TimeOnly>(new TimeOnly(10, 53), Variant.VariantKind.TimeOnly);
            TestCreateAndAccess<TimeOnly>(TimeOnly.MinValue, Variant.VariantKind.TimeOnly);
            TestCreateAndAccess<TimeOnly>(TimeOnly.MaxValue, Variant.VariantKind.TimeOnly);
            TestCreateAndAccess<TimeOnly?>(new TimeOnly(10, 53), Variant.VariantKind.TimeOnly);
            TestCreateAndAccess<TimeOnly?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<DateTime>(DateTime.Now, Variant.VariantKind.DateTime);
            TestCreateAndAccess<DateTime>(DateTime.MinValue, Variant.VariantKind.DateTime);
            TestCreateAndAccess<DateTime>(DateTime.MaxValue, Variant.VariantKind.DateTime);
            TestCreateAndAccess<DateTime?>(DateTime.Now, Variant.VariantKind.DateTime);
            TestCreateAndAccess<DateTime?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<TimeSpan>(TimeSpan.FromMinutes(53), Variant.VariantKind.TimeSpan);
            TestCreateAndAccess<TimeSpan>(TimeSpan.MinValue, Variant.VariantKind.TimeSpan);
            TestCreateAndAccess<TimeSpan>(TimeSpan.MaxValue, Variant.VariantKind.TimeSpan);
            TestCreateAndAccess<TimeSpan?>(TimeSpan.FromMinutes(53), Variant.VariantKind.TimeSpan);
            TestCreateAndAccess<TimeSpan?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<string>("string", Variant.VariantKind.String);
            TestCreateAndAccess<string>("", Variant.VariantKind.String);
            TestCreateAndAccess<string?>("string", Variant.VariantKind.String);
            TestCreateAndAccess<string?>(null, Variant.VariantKind.Null);

            TestCreateAndAccess<Guid>(Guid.NewGuid(), Variant.VariantKind.Object);
            TestCreateAndAccess<Guid?>(Guid.NewGuid(), Variant.VariantKind.Object);
            TestCreateAndAccess<Guid?>(null, Variant.VariantKind.Null);
        }

        private void TestCreateAndAccess<T>(T value, Variant.VariantKind expectedKind)
        {
            var v = Variant.Create<T>(value);

            Assert.AreEqual(expectedKind, v.Kind);

            if (value == null)
            {
                Assert.IsTrue(v.IsNull, "IsNull");
                Assert.IsFalse(v.IsType<T>(), "IsType");
                Assert.IsFalse(v.TryGet<T>(out _), "TryGet");
                Assert.ThrowsException<InvalidCastException>(() => v.Get<T>());
                Assert.AreEqual(default, v.AsType<T>());
            }
            else
            {
                Assert.IsFalse(v.IsNull, "IsNull");
                Assert.IsTrue(v.IsType<T>(), "IsType");
                Assert.IsTrue(v.TryGet<T>(out var actualValue), "TryGet");
                Assert.AreEqual(value, actualValue, "value");
                Assert.AreEqual(value, v.Get<T>());
                Assert.AreEqual(value, v.AsType<T>());
            }
        }
    }
}