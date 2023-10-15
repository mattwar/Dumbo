using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;

namespace Dumbo
{
    /// <summary>
    /// A type union for an instance of any type that avoids boxing of most common primitive values.
    /// </summary>
    [DebuggerDisplay("{DebugText}")]
    public readonly struct Variant : IEquatable<Variant>, ITypeUnion
    {
        private string DebugText => ToString();

        private readonly object? _refValue;
        private readonly Overlapped _structValue;

        private Variant(object? refValue, Overlapped structValue)
        {
            _refValue = refValue;
            _structValue = structValue;
        }

        private readonly VariantKind Kind
        {
            get
            {
                if (_refValue is OverlappedInfo info)
                    return info.Kind;
                else if (_refValue is string)
                    return VariantKind.String;
                else if (_refValue == null)
                    return VariantKind.Null;
                else
                    return VariantKind.Object;
            }
        }

        public readonly bool IsNull => Kind == VariantKind.Null;

        public readonly Type? Type =>
            _refValue is OverlappedInfo info
                ? info.Type
                : _refValue?.GetType();

        public static readonly Variant Null = new Variant(null, default);

        public static Variant Create(bool value) => new Variant(OverlappedInfo.Bool, new Overlapped { Bool = value });
        public static Variant Create(sbyte value) => new Variant(OverlappedInfo.Int8, new Overlapped { Int8 = value });
        public static Variant Create(short value) => new Variant(OverlappedInfo.Int16, new Overlapped { Int16 = value });
        public static Variant Create(int value) => new Variant(OverlappedInfo.Int32, new Overlapped { Int32 = value });
        public static Variant Create(long value) => new Variant(OverlappedInfo.Int64, new Overlapped { Int64 = value });
        public static Variant Create(byte value) => new Variant(OverlappedInfo.UInt8, new Overlapped { UInt8 = value });
        public static Variant Create(ushort value) => new Variant(OverlappedInfo.UInt16, new Overlapped { UInt16 = value });
        public static Variant Create(uint value) => new Variant(OverlappedInfo.UInt32, new Overlapped { UInt32 = value });
        public static Variant Create(ulong value) => new Variant(OverlappedInfo.UInt64, new Overlapped { UInt64 = value });
        public static Variant Create(float value) => new Variant(OverlappedInfo.Float32, new Overlapped { Float32 = value });
        public static Variant Create(double value) => new Variant(OverlappedInfo.Float64, new Overlapped { Float64 = value });
        public static Variant Create(char value) => new Variant(OverlappedInfo.Char16, new Overlapped { Char16 = value });
        public static Variant Create(Rune value) => new Variant(OverlappedInfo.Char32, new Overlapped { Int32 = value.Value });
        public static Variant Create(Decimal64 value) => new Variant(OverlappedInfo.Decimal64, new Overlapped { Int64 = value.GetBits() });
        public static Variant Create(DateOnly value) => new Variant(OverlappedInfo.DateOnly, new Overlapped { Int32 = value.DayNumber });
        public static Variant Create(TimeOnly value) => new Variant(OverlappedInfo.TimeOnly, new Overlapped { Int64 = value.Ticks });
        public static Variant Create(DateTime value) => new Variant(OverlappedInfo.DateTime, new Overlapped { Int64 = value.Ticks });
        public static Variant Create(TimeSpan value) => new Variant(OverlappedInfo.TimeSpan, new Overlapped { Int64 = value.Ticks });

        public static Variant Create(decimal value)
        {
            // if decimal fits into Decimal64 then store it as such
            if (Decimal64.TryConvert(value, out var dec64))
            {
                return new Variant(OverlappedInfo.Decimal128, new Overlapped { Int64 = dec64.GetBits() });
            }
            else
            {
                // otherwise box as object
                return new Variant(value, default);
            }
        }

        public static Variant Create(object? value) => 
            Create<object>(value!);

        public static Variant Create<T>(T? value)
        {
            if (typeof(T).IsEnum && value != null)
            {
                var info = GetEnumInfo<T>();
                var val = ConvertEnumToLong(value);
                return new Variant(info, new Overlapped { Int64 = val });
            }

            return value switch
            {
                bool v => Create(v),
                sbyte v => Create(v),
                short v => Create(v),
                int v => Create(v),
                long v => Create(v),
                byte v => Create(v),
                ushort v => Create(v),
                uint v => Create(v),
                ulong v => Create(v),
                float v => Create(v),
                double v => Create(v),
                char v => Create(v),
                Rune v => Create(v),
                Decimal64 v => Create(v),
                decimal v => Create(v),
                DateOnly v => Create(v),
                TimeOnly v => Create(v),
                DateTime v => Create(v),
                TimeSpan v => Create(v),
                Variant v => v,
                ITypeUnion tu => 
                      tu.TryGet<Variant>(out var v) ? v
                    : tu.TryGet<object>(out var ov) ? Create<object>(ov)
                    : Null,
                object obj => new Variant(obj, new Overlapped { UInt64 = 0L }),
                null => Null
            };
        }

        private bool BoolValue => Kind == VariantKind.Bool ? _structValue.Bool : false;
        private sbyte Int8Value => Kind == VariantKind.Int8 ? _structValue.Int8 : (sbyte)0;
        private short Int16Value => Kind == VariantKind.Int16 ? _structValue.Int16 : (short)0;
        private int Int32Value => Kind == VariantKind.Int32 ? _structValue.Int32 : 0;
        private long Int64Value => Kind == VariantKind.Int64 ? _structValue.Int64 : 0L;
        private byte UInt8Value => Kind == VariantKind.UInt8 ? _structValue.UInt8 : (byte)0;
        private ushort UInt16Value => Kind == VariantKind.UInt16 ? _structValue.UInt16 : (ushort)0;
        private uint UInt32Value => Kind == VariantKind.UInt32 ? _structValue.UInt32 : 0;
        private ulong UInt64Value => Kind == VariantKind.UInt64 ? _structValue.UInt64 : 0UL;
        private float Float32Value => Kind == VariantKind.Float32 ? _structValue.Float32 : 0.0f;
        private double Float64Value => Kind == VariantKind.Float64 ? _structValue.Float64 : 0.0;
        private char Char16Value => Kind == VariantKind.Char16 ? _structValue.Char16 : '\0';
        private Rune Char32Value => Kind == VariantKind.Char32 ? new Rune(_structValue.Int32) : default;
        private String? StringValue => Kind == VariantKind.String ? _refValue as string : null;
        private Decimal64 Decimal64Value => Kind == VariantKind.Decimal64 ? Decimal64.FromBits(_structValue.Int64) : Decimal64.Zero;
        private decimal Decimal128Value => 
            Kind == VariantKind.Decimal128 ? Decimal64.FromBits(_structValue.Int64).ToDecimal()
            : Kind == VariantKind.Object && _refValue is decimal decimalValue ? decimalValue
            : 0m;
        private DateOnly DateOnlyValue => Kind == VariantKind.DateOnly ? DateOnly.FromDayNumber(_structValue.Int32) : default;
        private TimeOnly TimeOnlyValue => Kind == VariantKind.TimeOnly ? new TimeOnly(_structValue.Int64) : default;
        private DateTime DateTimeValue => Kind == VariantKind.DateTime ? new DateTime(_structValue.Int64) : default;
        private TimeSpan TimeSpanValue => Kind == VariantKind.TimeSpan ? new TimeSpan(_structValue.Int64) : default;
        private long EnumLongValue => Kind == VariantKind.Enum ? _structValue.Int64 : default;
        private object? ObjectValue => Kind == VariantKind.Object ? _refValue : null;
              
        public readonly bool IsType<T>()
        {
            if (_refValue is EnumInfo einfo)
                return einfo.IsType<T>(EnumLongValue);
            else if (_refValue is OverlappedInfo info)
                return info.Kind == GetKind<T>();
            else
                return _refValue is T;
        }

        public readonly bool TryGet<T>([NotNullWhen(true)] out T value)
        {
            if (typeof(T) == typeof(object)
                && ToObject() is T objVal)
            {
                value = objVal;
                return true;
            }
            else if (typeof(T) == typeof(Variant)
                && this is T varVal)
            {
                value = varVal;
                return true;
            }

            switch (Kind)
            {
                case VariantKind.Bool: 
                    if (BoolValue is T boolValue)
                    {
                        value = boolValue;
                        return true;
                    }
                    break;
                case VariantKind.Int8:
                    if (Int8Value is T sbyteValue)
                    {
                        value = sbyteValue;
                        return true;
                    }
                    break;
                case VariantKind.Int16:
                    if (Int16Value is T shortValue)
                    {
                        value = shortValue;
                        return true;
                    }
                    break;
                case VariantKind.Int32:
                    if (Int32Value is T intValue)
                    {
                        value = intValue;
                        return true;
                    }
                    break;
                case VariantKind.Int64:
                    if (Int64Value is T longValue)
                    {
                        value = longValue;
                        return true;
                    }
                    break;
                case VariantKind.UInt8:
                    if (UInt8Value is T byteValue)
                    {
                        value = byteValue;
                        return true;
                    }
                    break;
                case VariantKind.UInt16:
                    if (UInt16Value is T ushortValue)
                    {
                        value = ushortValue;
                        return true;
                    }
                    break;
                case VariantKind.UInt32:
                    if (UInt32Value is T uintValue)
                    {
                        value = uintValue;
                        return true;
                    }
                    break;
                case VariantKind.UInt64:
                    if (UInt64Value is T ulongValue)
                    {
                        value = ulongValue;
                        return true;
                    }
                    break;
                case VariantKind.Float32:
                    if (Float32Value is T floatValue)
                    {
                        value = floatValue;
                        return true;
                    }
                    break;
                case VariantKind.Float64:
                    if (Float64Value is T doubleValue)
                    {
                        value = doubleValue;
                        return true;
                    }
                    break;
                case VariantKind.Char16:
                    if (Char16Value is T charValue)
                    {
                        value = charValue;
                        return true;
                    }
                    break;
                case VariantKind.Char32:
                    if (Char32Value is T runeValue)
                    {
                        value = runeValue;
                        return true;
                    }
                    break;
                case VariantKind.Decimal64:
                    if (Decimal64Value is T dec64Value)
                    {
                        value = dec64Value;
                        return true;
                    }
                    break;
                case VariantKind.Decimal128:
                    if (Decimal128Value is T decimalValue)
                    {
                        value = decimalValue;
                        return true;
                    }
                    break;
                case VariantKind.DateOnly:
                    if (DateOnlyValue is T dateValue)
                    {
                        value = dateValue;
                        return true;
                    }
                    break;
                case VariantKind.TimeOnly:
                    if (TimeOnlyValue is T timeValue)
                    {
                        value = timeValue;
                        return true;
                    }
                    break;
                case VariantKind.DateTime:
                    if (DateTimeValue is T dateTimeValue)
                    {
                        value = dateTimeValue;
                        return true;
                    }
                    break;
                case VariantKind.TimeSpan:
                    if (TimeSpanValue is T timeSpanValue)
                    {
                        value = timeSpanValue;
                        return true;
                    }
                    break;
                case VariantKind.Enum:
                    var info = (EnumInfo)_refValue!;
                    if (info.IsType<T>(EnumLongValue))
                    {
                        return TryConvertLongToEnum(_structValue.Int64, out value);
                    }
                    break;
                case VariantKind.String:
                    if (StringValue is T stringValue)
                    {
                        value = stringValue;
                        return true;
                    }
                    break;
                case VariantKind.Object:
                    if (ObjectValue is T objectValue)
                    {
                        value = objectValue;
                        return true;
                    }
                    break;
                case VariantKind.Null:
                    value = default!;
                    return false;
                default:
                    throw new InvalidOperationException("Unhandled VariantKind");
            };

            value = default!;
            return false;
        }

        public readonly T AsType<T>() => TryGet<T>(out var value) ? value : default!;
        public readonly T Get<T>() => TryGet<T>(out var value) ? value : throw new InvalidCastException($"Cannot cast to type: {typeof(T).Name}");

        public object? ToObject() =>
            Kind switch
            {
                VariantKind.Bool => BoolValue,
                VariantKind.Int8 => Int8Value,
                VariantKind.Int16 => Int16Value,
                VariantKind.Int32 => Int32Value,
                VariantKind.Int64 => Int64Value,
                VariantKind.UInt8 => UInt8Value,
                VariantKind.UInt16 => UInt16Value,
                VariantKind.UInt32 => UInt32Value,
                VariantKind.UInt64 => UInt64Value,
                VariantKind.Float32 => Float32Value,
                VariantKind.Float64 => Float64Value,
                VariantKind.Char16 => Char16Value,
                VariantKind.Char32 => Char32Value,
                VariantKind.Decimal64 => Decimal64Value,
                VariantKind.Decimal128 => Decimal128Value,
                VariantKind.DateOnly => DateOnlyValue,
                VariantKind.DateTime => DateTimeValue,
                VariantKind.TimeSpan => DateTimeValue,
                VariantKind.Enum => _refValue is EnumInfo info ? info.ConvertToObject(EnumLongValue) : null,
                VariantKind.String => StringValue,
                VariantKind.Object => ObjectValue,
                _ => null
            };

        public readonly override string ToString() =>
            Kind switch
            {
                VariantKind.Bool => BoolValue.ToString(),
                VariantKind.Int8 => Int8Value.ToString(),
                VariantKind.Int16 => Int16Value.ToString(),
                VariantKind.Int32 => Int32Value.ToString(),
                VariantKind.Int64 => Int64Value.ToString(),
                VariantKind.UInt8 => UInt8Value.ToString(),
                VariantKind.UInt16 => UInt16Value.ToString(),
                VariantKind.UInt32 => UInt32Value.ToString(),
                VariantKind.UInt64 => UInt64Value.ToString(),
                VariantKind.Float32 => Float32Value.ToString(),
                VariantKind.Float64 => Float64Value.ToString(),
                VariantKind.Char16 => Char16Value.ToString(),
                VariantKind.Char32 => Char32Value.ToString(),
                VariantKind.Decimal64 => Decimal64Value.ToString(),
                VariantKind.Decimal128 => Decimal128Value.ToString(),
                VariantKind.DateOnly => DateOnlyValue.ToString(),
                VariantKind.DateTime => DateTimeValue.ToString(),
                VariantKind.TimeSpan => DateTimeValue.ToString(),
                VariantKind.Enum => _refValue is EnumInfo info ? info.ConvertToEnumString(EnumLongValue) : "",
                VariantKind.String => StringValue ?? "",
                VariantKind.Object => ObjectValue?.ToString() ?? "",
                _ => ""
            };

        public readonly bool Equals(Variant other) =>
            Kind == other.Kind &&
            Kind switch
            {
                VariantKind.Bool => BoolValue == other.BoolValue,
                VariantKind.Int8 => Int8Value == other.Int8Value,
                VariantKind.Int16 => Int16Value == other.Int16Value,
                VariantKind.Int32 => Int32Value == other.Int32Value,
                VariantKind.Int64 => Int64Value == other.Int64Value,
                VariantKind.UInt8 => UInt8Value == other.UInt8Value,
                VariantKind.UInt16 => UInt16Value == other.UInt16Value,
                VariantKind.UInt32 => UInt32Value == other.UInt32Value,
                VariantKind.UInt64 => UInt64Value == other.UInt64Value,
                VariantKind.Float32 => Float32Value == other.Float32Value,
                VariantKind.Float64 => Float64Value == other.Float64Value,
                VariantKind.Char16 => Char16Value == other.Char16Value,
                VariantKind.Char32 => Char32Value == other.Char32Value,
                VariantKind.Decimal64 => Decimal64Value == other.Decimal64Value,
                VariantKind.Decimal128 => Decimal128Value == other.Decimal128Value,
                VariantKind.DateOnly => DateOnlyValue == other.DateOnlyValue,
                VariantKind.TimeOnly => TimeOnlyValue == other.TimeOnlyValue,
                VariantKind.DateTime => DateTimeValue == other.DateTimeValue,
                VariantKind.TimeSpan => TimeSpanValue == other.TimeSpanValue,
                VariantKind.Enum => EnumLongValue == other.EnumLongValue,
                VariantKind.Object => (ObjectValue == other.ObjectValue) || (ObjectValue?.Equals(other) ?? false),
                _ => false
            };

        public readonly bool Equals<T>(T other) where T : IEquatable<T>
        {
            if (other is Variant vother)
            {
                return Equals(vother);
            }
            else if (TryGet<T>(out var tval))
            {
                return tval.Equals(other);
            }

            return false;
        }

        public readonly override bool Equals([NotNullWhen(true)] object? obj) =>
            obj is Variant other && Equals(other);

        public readonly override int GetHashCode() =>
            Kind switch
            {
                VariantKind.Bool => BoolValue ? 1 : 0,
                VariantKind.Int8 => Int8Value.GetHashCode(),
                VariantKind.Int16 => Int16Value.GetHashCode(),
                VariantKind.Int32 => Int32Value.GetHashCode(),
                VariantKind.Int64 => Int64Value.GetHashCode(),
                VariantKind.UInt8 => UInt8Value.GetHashCode(),
                VariantKind.UInt16 => UInt16Value.GetHashCode(),
                VariantKind.UInt32 => UInt32Value.GetHashCode(),
                VariantKind.UInt64 => UInt64Value.GetHashCode(),
                VariantKind.Float32 => Float32Value.GetHashCode(),
                VariantKind.Float64 => Float64Value.GetHashCode(),
                VariantKind.Char16 => Char16Value.GetHashCode(),
                VariantKind.Char32 => Char32Value.GetHashCode(),
                VariantKind.Decimal64 => Decimal64Value.GetHashCode(),
                VariantKind.Decimal128 => Decimal128Value.GetHashCode(),
                VariantKind.DateOnly => DateOnlyValue.GetHashCode(),
                VariantKind.TimeOnly => TimeOnlyValue.GetHashCode(),
                VariantKind.DateTime => DateTimeValue.GetHashCode(),
                VariantKind.TimeSpan => TimeSpanValue.GetHashCode(),
                VariantKind.Enum => EnumLongValue.GetHashCode(),
                VariantKind.Object => ObjectValue?.GetHashCode() ?? 0,
                _ => 0
            };

        public static bool operator ==(Variant a, Variant b) => a.Equals(b);
        public static bool operator !=(Variant a, Variant b) => !a.Equals(b);

        public static implicit operator Variant(bool value) => Create(value);
        public static implicit operator Variant(sbyte value) => Create(value);
        public static implicit operator Variant(short value) => Create(value);
        public static implicit operator Variant(int value) => Create(value);
        public static implicit operator Variant(long value) => Create(value);
        public static implicit operator Variant(byte value) => Create(value);
        public static implicit operator Variant(ushort value) => Create(value);
        public static implicit operator Variant(uint value) => Create(value);
        public static implicit operator Variant(ulong value) => Create(value);
        public static implicit operator Variant(float value) => Create(value);
        public static implicit operator Variant(double value) => Create(value);
        public static implicit operator Variant(Decimal64 value) => Create(value);
        public static implicit operator Variant(decimal value) => Create(value);
        public static implicit operator Variant(char value) => Create(value);
        public static implicit operator Variant(Rune value) => Create(value);
        public static implicit operator Variant(string value) => Create(value);
        public static implicit operator Variant(DateOnly value) => Create(value);
        public static implicit operator Variant(TimeOnly value) => Create(value);
        public static implicit operator Variant(DateTime value) => Create(value);
        public static implicit operator Variant(DateTimeOffset value) => Create(value);
        public static implicit operator Variant(TimeSpan value) => Create(value);
        public static implicit operator Variant(Guid value) => Create(value);

        public static implicit operator Variant(bool? value) => Create(value);
        public static implicit operator Variant(sbyte? value) => Create(value);
        public static implicit operator Variant(short? value) => Create(value);
        public static implicit operator Variant(int? value) => Create(value);
        public static implicit operator Variant(long? value) => Create(value);
        public static implicit operator Variant(byte? value) => Create(value);
        public static implicit operator Variant(ushort? value) => Create(value);
        public static implicit operator Variant(uint? value) => Create(value);
        public static implicit operator Variant(ulong? value) => Create(value);
        public static implicit operator Variant(float? value) => Create(value);
        public static implicit operator Variant(double? value) => Create(value);
        public static implicit operator Variant(Decimal64? value) => Create(value);
        public static implicit operator Variant(decimal? value) => Create(value);
        public static implicit operator Variant(char? value) => Create(value);
        public static implicit operator Variant(Rune? value) => Create(value);
        public static implicit operator Variant(DateOnly? value) => Create(value);
        public static implicit operator Variant(TimeOnly? value) => Create(value);
        public static implicit operator Variant(DateTime? value) => Create(value);
        public static implicit operator Variant(DateTimeOffset? value) => Create(value);
        public static implicit operator Variant(TimeSpan? value) => Create(value);
        public static implicit operator Variant(Guid? value) => Create(value);

        public static explicit operator bool(Variant value) => value.ConvertTo<bool>();
        public static implicit operator sbyte(Variant value) => value.ConvertTo<sbyte>();
        public static implicit operator short(Variant value) => value.ConvertTo<short>();
        public static implicit operator int(Variant value) => value.ConvertTo<int>();
        public static implicit operator long(Variant value) => value.ConvertTo<long>();
        public static implicit operator byte(Variant value) => value.ConvertTo<byte>();
        public static implicit operator ushort(Variant value) => value.ConvertTo<ushort>();
        public static implicit operator uint(Variant value) => value.ConvertTo<uint>();
        public static implicit operator ulong(Variant value) => value.ConvertTo<ulong>();
        public static implicit operator float(Variant value) => value.ConvertTo<float>();
        public static implicit operator double(Variant value) => value.ConvertTo<double>();
        public static implicit operator Decimal64(Variant value) => value.ConvertTo<Decimal64>();
        public static implicit operator decimal(Variant value) => value.ConvertTo<decimal>();
        public static implicit operator char(Variant value) => value.ConvertTo<char>();
        public static implicit operator Rune(Variant value) => value.ConvertTo<Rune>();
        public static implicit operator String(Variant value) => value.ConvertTo<string>();
        public static implicit operator DateOnly(Variant value) => value.ConvertTo<DateOnly>();
        public static implicit operator TimeOnly(Variant value) => value.ConvertTo<TimeOnly>();
        public static implicit operator DateTime(Variant value) => value.ConvertTo<DateTime>();
        public static implicit operator TimeSpan(Variant value) => value.ConvertTo<TimeSpan>();
        public static implicit operator Guid(Variant value) => value.ConvertTo<Guid>();

        public static explicit operator bool?(Variant value) => value.ConvertToOrDefault<bool?>();
        public static implicit operator sbyte?(Variant value) => value.ConvertToOrDefault<sbyte?>();
        public static implicit operator short?(Variant value) => value.ConvertToOrDefault<short?>();
        public static implicit operator int?(Variant value) => value.ConvertToOrDefault<int?>();
        public static implicit operator long?(Variant value) => value.ConvertToOrDefault<long?>();
        public static implicit operator byte?(Variant value) => value.ConvertToOrDefault<byte?>();
        public static implicit operator ushort?(Variant value) => value.ConvertToOrDefault<ushort?>();
        public static implicit operator uint?(Variant value) => value.ConvertToOrDefault<uint?>();
        public static implicit operator ulong?(Variant value) => value.ConvertToOrDefault<ulong?>();
        public static implicit operator float?(Variant value) => value.ConvertToOrDefault<float?>();
        public static implicit operator double?(Variant value) => value.ConvertToOrDefault<double?>();
        public static implicit operator Decimal64?(Variant value) => value.ConvertToOrDefault<Decimal64?>();
        public static implicit operator decimal?(Variant value) => value.ConvertToOrDefault<decimal?>();
        public static implicit operator char?(Variant value) => value.ConvertToOrDefault<char?>();
        public static implicit operator Rune?(Variant value) => value.ConvertToOrDefault<Rune?>();
        public static implicit operator DateOnly?(Variant value) => value.ConvertToOrDefault<DateOnly?>();
        public static implicit operator TimeOnly?(Variant value) => value.ConvertToOrDefault<TimeOnly?>();
        public static implicit operator DateTime?(Variant value) => value.ConvertToOrDefault<DateTime?>();
        public static implicit operator TimeSpan?(Variant value) => value.ConvertToOrDefault<TimeSpan?>();
        public static implicit operator Guid?(Variant value) => value.ConvertToOrDefault<Guid?>();

        private bool TryConvertToInt64(out long converted)
        {
            switch (Kind)
            {
                case VariantKind.Bool:
                    converted = BoolValue ? 1 : 0;
                    return true;
                case VariantKind.Int8:
                    converted = Int8Value;
                    return true;
                case VariantKind.Int16:
                    converted = Int16Value;
                    return true;
                case VariantKind.Int32:
                    converted = Int32Value;
                    return true;
                case VariantKind.Int64:
                    converted = Int64Value;
                    return true;
                case VariantKind.UInt8:
                    converted = UInt8Value;
                    return true;
                case VariantKind.UInt16:
                    converted = UInt16Value;
                    return true;
                case VariantKind.UInt32:
                    converted = UInt32Value;
                    return true;
                case VariantKind.UInt64:
                    if (UInt64Value <= Int64.MaxValue)
                    {
                        converted = (long)UInt64Value;
                        return true;
                    }
                    break;
                case VariantKind.Float32:
                    if (Float32Value >= Int64.MinValue && Float32Value <= Int64.MaxValue)
                    {
                        converted = (long)Float32Value;
                        return true;
                    }
                    break;
                case VariantKind.Float64:
                    if (Float64Value >= Int64.MinValue && Float64Value <= Int64.MaxValue)
                    {
                        converted = (long)Float64Value;
                        return true;
                    }
                    break;
                case VariantKind.Char16:
                    converted = Char16Value;
                    return true;
                case VariantKind.Char32:
                    converted = Char32Value.Value;
                    return true;
                case VariantKind.Decimal64:
                    converted = (long)Decimal64Value;
                    return true;
                case VariantKind.Decimal128:
                    var dec128 = Decimal128Value;
                    if (dec128 >= Int64.MinValue && dec128 <= Int64.MaxValue)
                    {
                        converted = (long)dec128;
                        return true;
                    }
                    break;
                case VariantKind.Enum:
                    converted = EnumLongValue;
                    return true;
                case VariantKind.String:
                    return Int64.TryParse(StringValue, out converted);
            };

            converted = default!;
            return false;
        }

        private bool TryConvertToDecimal(out decimal converted)
        {
            switch (Kind)
            {
                case VariantKind.Bool:
                    converted = BoolValue ? 1 : 0;
                    return true;
                case VariantKind.Int8:
                    converted = Int8Value;
                    return true;
                case VariantKind.Int16:
                    converted = Int16Value;
                    return true;
                case VariantKind.Int32:
                    converted = Int32Value;
                    return true;
                case VariantKind.Int64:
                    converted = Int64Value;
                    return true;
                case VariantKind.UInt8:
                    converted = UInt8Value;
                    return true;
                case VariantKind.UInt16:
                    converted = UInt16Value;
                    return true;
                case VariantKind.UInt32:
                    converted = UInt32Value;
                    return true;
                case VariantKind.UInt64:
                    converted = UInt64Value;
                    return true;
                case VariantKind.Float32:
                    if (Float32Value >= (double)decimal.MinValue && Float32Value <= (double)decimal.MaxValue)
                    {
                        converted = (decimal)Float32Value;
                        return true;
                    }
                    break;
                case VariantKind.Float64:
                    if (Float64Value >= (double)decimal.MinValue && Float64Value <= (double)decimal.MaxValue)
                    {
                        converted = (decimal)Float64Value;
                        return true;
                    }
                    break;
                case VariantKind.Char16:
                    converted = Char16Value;
                    return true;
                case VariantKind.Char32:
                    converted = Char32Value.Value;
                    return true;
                case VariantKind.Decimal64:
                    converted = Decimal64Value;
                    return true;
                case VariantKind.Decimal128:
                    converted = Decimal128Value;
                    return true;
                case VariantKind.Enum:
                    converted = EnumLongValue;
                    return true;
                case VariantKind.String:
                    return decimal.TryParse(StringValue, out converted);
            };

            converted = default!;
            return false;
        }

        private bool TryConvertToBool(out bool value)
        {
            if (Kind == VariantKind.Bool)
            {
                value = BoolValue;
                return true;
            }
            else if (Kind == VariantKind.String && bool.TryParse(StringValue, out var bval))
            {
                value = bval;
                return true;
            }
            else if (TryConvertToInt64(out var i64) && (i64 == 1 || i64 == 0))
            {
                value = i64 == 1;
                return true;
            }

            value = default;
            return false;
        }

        private bool TryConvertToInt8(out sbyte value)
        {
            if (TryConvertToInt64(out var i64) && i64 >= sbyte.MinValue && i64 <= sbyte.MaxValue)
            {
                value = (sbyte)i64;
                return true;
            }
            value = default;
            return false;
        }

        private bool TryConvertToInt16(out short value)
        {
            if (TryConvertToInt64(out var i64) && i64 >= short.MinValue && i64 <= short.MaxValue)
            {
                value = (short)i64;
                return true;
            }
            value = default;
            return false;
        }

        private bool TryConvertToInt32(out int value)
        {
            if (TryConvertToInt64(out var i64) && i64 >= int.MinValue && i64 <= int.MaxValue)
            {
                value = (short)i64;
                return true;
            }
            value = default;
            return false;
        }

        private bool TryConvertToUInt8(out short value)
        {
            if (TryConvertToInt64(out var i64) && i64 >= byte.MinValue && i64 <= byte.MaxValue)
            {
                value = (byte)i64;
                return true;
            }
            value = default;
            return false;
        }

        private bool TryConvertToUInt16(out ushort value)
        {
            if (TryConvertToInt64(out var i64) && i64 >= ushort.MinValue && i64 <= ushort.MaxValue)
            {
                value = (ushort)i64;
                return true;
            }
            value = default;
            return false;
        }

        private bool TryConvertToUInt32(out uint value)
        {
            if (TryConvertToInt64(out var i64) && i64 >= uint.MinValue && i64 <= uint.MaxValue)
            {
                value = (uint)i64;
                return true;
            }
            value = default;
            return false;
        }

        private bool TryConvertToUInt64(out ulong value)
        {
            if (TryConvertToDecimal(out var dec) && dec >= ulong.MinValue && dec <= ulong.MaxValue)
            {
                value = (ulong)dec;
                return true;
            }
            value = default;
            return false;
        }

        private bool TryConvertToFloat32(out float value)
        {
            if (Kind == VariantKind.Float32)
            {
                value = Float32Value;
                return true;
            }
            else if (Kind == VariantKind.Float64 && Float64Value >= float.MinValue && Float64Value <= float.MaxValue)
            {
                value = (float)Float64Value;
                return true;
            }
            else if (TryConvertToDecimal(out var dec))
            {
                value = (float)dec;
                return true;
            }
            value = default;
            return false;
        }

        private bool TryConvertToFloat64(out double value)
        {
            if (Kind == VariantKind.Float32)
            {
                value = Float32Value;
                return true;
            }
            else if (Kind == VariantKind.Float64)
            {
                value = Float64Value;
                return true;
            }
            else if (TryConvertToDecimal(out var dec))
            {
                value = (float)dec;
                return true;
            }
            value = default;
            return false;
        }

        private bool TryConvertToEnum<TEnum>([NotNullWhen(true)] out TEnum value)
        {
            if (TryConvertToInt64(out var i64val)
                && TryConvertLongToEnum(i64val, out value))
            {
                return true;
            }

            value = default!;
            return false;
        }

        private bool TryConvertToDecimal64(out Decimal64 value)
        {
            if (Kind == VariantKind.Decimal64)
            {
                value = Decimal64Value;
                return true;
            }
            else if (TryConvertToDecimal(out var dec))
            {
                return Decimal64.TryConvert(dec, out value);
            }
            value = default;
            return false;
        }

        private bool TryConvertToDateOnly(out DateOnly value)
        {
            if (Kind == VariantKind.DateOnly)
            {
                value = DateOnlyValue;
                return true;
            }
            else if (Kind == VariantKind.DateTime)
            {
                value = DateOnly.FromDateTime(DateTimeValue);
                return true;
            }
            else if (Kind == VariantKind.TimeSpan)
            {
                value = DateOnly.FromDateTime(new DateTime(TimeSpanValue.Ticks));
                return true;
            }
            else if (Kind == VariantKind.TimeOnly)
            {
                value = default;
                return true;
            }
            else if (Kind == VariantKind.String && DateOnly.TryParse(StringValue, out var dateVal))
            {
                value = dateVal;
                return true;
            }

            value = default;
            return false;
        }

        private bool TryConvertToTimeOnly(out TimeOnly value)
        {
            if (Kind == VariantKind.TimeOnly)
            {
                value = TimeOnlyValue;
                return true;
            }
            else if (Kind == VariantKind.TimeSpan)
            {
                value = TimeOnly.FromTimeSpan(TimeSpanValue);
                return true;
            }
            else if (Kind == VariantKind.DateTime)
            {
                value = TimeOnly.FromDateTime(DateTimeValue);
                return true;
            }
            else if (Kind == VariantKind.DateOnly)
            {
                value = default;
                return true;
            }
            else if (Kind == VariantKind.String && TimeOnly.TryParse(StringValue, out var timeVal))
            {
                value = timeVal;
                return true;
            }

            value = default;
            return false;
        }

        private bool TryConvertToDateTime(out DateTime value)
        {
            if (Kind == VariantKind.DateTime)
            {
                value = DateTimeValue;
                return true;
            }
            else if (Kind == VariantKind.DateOnly)
            {
                value = DateOnlyValue.ToDateTime(default);
                return true;
            }
            else if (Kind == VariantKind.TimeOnly)
            {
                value = new DateTime(TimeOnlyValue.Ticks);
                return true;
            }
            else if (Kind == VariantKind.TimeSpan)
            {
                value = new DateTime(TimeSpanValue.Ticks);
                return true;
            }
            else if (Kind == VariantKind.String && DateTime.TryParse(StringValue, out var dateVal))
            {
                value = dateVal;
                return true;
            }

            value = default;
            return false;
        }

        private bool TryConvertToTimeSpan(out TimeSpan value)
        {
            if (Kind == VariantKind.TimeSpan)
            {
                value = TimeSpanValue;
                return true;
            }
            else if (Kind == VariantKind.DateTime)
            {
                value = TimeSpan.FromTicks(DateTimeValue.Ticks);
                return true;
            }
            else if (Kind == VariantKind.DateOnly)
            {
                value = TimeSpan.FromDays(DateOnlyValue.DayNumber);
                return true;
            }
            else if (Kind == VariantKind.TimeOnly)
            {
                value = TimeSpan.FromTicks(TimeOnlyValue.Ticks);
                return true;
            }
            else if (Kind == VariantKind.String && TimeSpan.TryParse(StringValue, out var timeVal))
            {
                value = timeVal;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attempts to convert the value into the specified type.
        /// Returns true if the value is successfully converted.
        /// </summary>
        public bool TryConvertTo<T>([NotNullWhen(true)] out T converted)
        {
            var kind = GetKind<T>();

            switch (kind)
            {
                case VariantKind.Bool when TryConvertToBool(out var bval) && bval is T tval:
                    converted = tval;
                    return true;
                case VariantKind.Int8 when TryConvertToInt8(out var i8val) && i8val is T tval:
                    converted = tval;
                    return true;
                case VariantKind.Int16 when TryConvertToInt16(out var i16val) && i16val is T tval:
                    converted = tval;
                    return true;
                case VariantKind.Int32 when TryConvertToInt32(out var i32val) && i32val is T tval:
                    converted = tval;
                    return true;
                case VariantKind.Int64 when TryConvertToInt64(out var i64val) && i64val is T tval:
                    converted = tval;
                    return true;
                case VariantKind.UInt8 when TryConvertToUInt8(out var u8val) && u8val is T tval:
                    converted = tval;
                    return true;
                case VariantKind.UInt16 when TryConvertToUInt16(out var u16val) && u16val is T tval:
                    converted = tval;
                    return true;
                case VariantKind.UInt32 when TryConvertToUInt32(out var u32val) && u32val is T tval:
                    converted = tval;
                    return true;
                case VariantKind.UInt64 when TryConvertToUInt64(out var u64val) && u64val is T tval:
                    converted = tval;
                    return true;
                case VariantKind.Float32 when TryConvertToFloat32(out var f32val) && f32val is T tval:
                    converted = tval;
                    return true;
                case VariantKind.Float64 when TryConvertToFloat64(out var f64val) && f64val is T tval:
                    converted = tval;
                    return true;
                case VariantKind.Decimal64 when TryConvertToDecimal64(out var d64val) && d64val is T tval:
                    converted = tval;
                    return true;
                case VariantKind.Decimal128 when TryConvertToDecimal(out var dval) && dval is T tval:
                    converted = tval;
                    return true;
                case VariantKind.DateOnly when TryConvertToDateOnly(out var doval) && doval is T tval:
                    converted = tval;
                    return true;
                case VariantKind.TimeOnly when TryConvertToTimeOnly(out var toval) && toval is T tval:
                    converted = tval;
                    return true;
                case VariantKind.DateTime when TryConvertToDateTime(out var dtval) && dtval is T tval:
                    converted = tval;
                    return true;
                case VariantKind.TimeSpan when TryConvertToTimeSpan(out var tsval) && tsval is T tval:
                    converted = tval;
                    return true;
                case VariantKind.Enum:
                    return TryConvertToEnum(out converted);
                case VariantKind.Object when ObjectValue is T tval:
                    converted = tval;
                    return true;                
                default:
                    if (typeof(T) == typeof(object)
                        && ToObject() is T oval)
                    {
                        converted = oval;
                        return true;
                    }
                    break;
            };

            converted = default!;
            return false;
        }

        public T ConvertTo<T>() =>
            TryConvertTo<T>(out var tval) ? tval : throw new InvalidCastException();

        public T ConvertToOrDefault<T>() =>
            TryConvertTo<T>(out var tval) ? tval : default!;

        /// <summary>
        /// Attempts to convert the value into the specified parsable type.
        /// Returns true if the value is successfully converted.
        /// </summary>
        public bool TryConvertToParsable<T>([NotNullWhen(true)] out T converted)
            where T : IParsable<T>
        {
            if (TryConvertTo(out converted))
            {
                return true;
            }
            else if (Kind == VariantKind.String && T.TryParse(StringValue, null, out converted!))
            {
                return true;
            }

            converted = default!;
            return false;
        }

        public T ConvertToParsable<T>() where T : IParsable<T> =>
            TryConvertToParsable<T>(out var tval) ? tval : throw new InvalidCastException();

        public T ConvertToParsableOrDefault<T>() where T : IParsable<T> =>
            TryConvertToParsable<T>(out var tval) ? tval : default!;

        private static VariantKind GetKind(Type type)
        {
            if (s_typeToKindMap.TryGetValue(type, out var kind))
                return kind;

            return VariantKind.Object;
        }

        private static VariantKind GetKind<TType>()
        {
            var type = typeof(TType);

            if (s_typeToKindMap.TryGetValue(type, out var kind))
                return kind;

            if (type.IsEnum)
                return GetEnumInfo<TType>().Kind;

            return VariantKind.Object;
        }

        private static readonly Dictionary<Type, VariantKind> s_typeToKindMap = new Dictionary<Type, VariantKind>
        {
            { typeof(bool), VariantKind.Bool },
            { typeof(sbyte), VariantKind.Int8 },
            { typeof(short), VariantKind.Int16 },
            { typeof(int), VariantKind.Int32 },
            { typeof(long), VariantKind.Int64 },
            { typeof(byte), VariantKind.UInt8 },
            { typeof(ushort), VariantKind.UInt16 },
            { typeof(uint), VariantKind.UInt32 },
            { typeof(ulong), VariantKind.UInt64 },
            { typeof(Decimal64), VariantKind.Decimal64 },
            { typeof(decimal), VariantKind.Decimal128 },
            { typeof(float), VariantKind.Float32 },
            { typeof(double), VariantKind.Float64 },
            { typeof(char), VariantKind.Char16 },
            { typeof(Rune), VariantKind.Char32 },
            { typeof(DateOnly), VariantKind.DateOnly },
            { typeof(TimeOnly), VariantKind.TimeOnly },
            { typeof(DateTime), VariantKind.DateTime },
            { typeof(TimeSpan), VariantKind.TimeSpan },
            { typeof(string), VariantKind.String },

            { typeof(bool?), VariantKind.Bool },
            { typeof(sbyte?), VariantKind.Int8 },
            { typeof(short?), VariantKind.Int16 },
            { typeof(int?), VariantKind.Int32 },
            { typeof(long?), VariantKind.Int64 },
            { typeof(byte?), VariantKind.UInt8 },
            { typeof(ushort?), VariantKind.UInt16 },
            { typeof(uint?), VariantKind.UInt32 },
            { typeof(ulong?), VariantKind.UInt64 },
            { typeof(Decimal64?), VariantKind.Decimal64 },
            { typeof(decimal?), VariantKind.Decimal128 },
            { typeof(float?), VariantKind.Float32 },
            { typeof(double?), VariantKind.Float64 },
            { typeof(char?), VariantKind.Char16 },
            { typeof(Rune?), VariantKind.Char32 },
            { typeof(DateOnly?), VariantKind.DateOnly },
            { typeof(TimeOnly?), VariantKind.TimeOnly },
            { typeof(DateTime?), VariantKind.DateTime },
            { typeof(TimeSpan?), VariantKind.TimeSpan },
        };

        private enum VariantKind
        {
            Bool,
            Int8,
            Int16,
            Int32,
            Int64,
            UInt8,
            UInt16,
            UInt32,
            UInt64,
            Float32,
            Float64,
            Char16,
            Char32,
            Decimal64,
            Decimal128,
            DateOnly,
            TimeOnly,
            DateTime,
            TimeSpan,
            String,
            Object,
            Enum,
            Null
        }

        [StructLayout(LayoutKind.Explicit)]
        struct Overlapped
        {
            [FieldOffset(0)]
            public bool Bool;

            [FieldOffset(0)]
            public sbyte Int8;

            [FieldOffset(0)]
            public byte UInt8;

            [FieldOffset(0)]
            public short Int16;

            [FieldOffset(0)]
            public ushort UInt16;

            [FieldOffset(0)]
            public int Int32;

            [FieldOffset(0)]
            public uint UInt32;

            [FieldOffset(0)]
            public long Int64;

            [FieldOffset(0)]
            public ulong UInt64;

            [FieldOffset(0)]
            public float Float32;

            [FieldOffset(0)]
            public double Float64;

            [FieldOffset(0)]
            public char Char16;
        }

        private class OverlappedInfo
        {
            public VariantKind Kind { get; }
            public Type Type { get; }

            public OverlappedInfo(VariantKind kind, Type type)
            {
                this.Kind = kind;
                this.Type = type;
            }

            public static OverlappedInfo Bool = new OverlappedInfo(VariantKind.Bool, typeof(bool));
            public static OverlappedInfo Int8 = new OverlappedInfo(VariantKind.Int8, typeof(sbyte));
            public static OverlappedInfo Int16 = new OverlappedInfo(VariantKind.Int16, typeof(short));
            public static OverlappedInfo Int32 = new OverlappedInfo(VariantKind.Int32, typeof(int));
            public static OverlappedInfo Int64 = new OverlappedInfo(VariantKind.Int64, typeof(long));
            public static OverlappedInfo UInt8 = new OverlappedInfo(VariantKind.UInt8, typeof(byte));
            public static OverlappedInfo UInt16 = new OverlappedInfo(VariantKind.UInt16, typeof(ushort));
            public static OverlappedInfo UInt32 = new OverlappedInfo(VariantKind.UInt32, typeof(uint));
            public static OverlappedInfo UInt64 = new OverlappedInfo(VariantKind.UInt64, typeof(ulong));
            public static OverlappedInfo Float32 = new OverlappedInfo(VariantKind.Float32, typeof(float));
            public static OverlappedInfo Float64 = new OverlappedInfo(VariantKind.Float64, typeof(double));
            public static OverlappedInfo Char16 = new OverlappedInfo(VariantKind.Char16, typeof(char));
            public static OverlappedInfo Char32 = new OverlappedInfo(VariantKind.Char32, typeof(Rune));
            public static OverlappedInfo Decimal64 = new OverlappedInfo(VariantKind.Decimal64, typeof(Decimal64));
            public static OverlappedInfo Decimal128 = new OverlappedInfo(VariantKind.Decimal128, typeof(decimal));
            public static OverlappedInfo DateOnly = new OverlappedInfo(VariantKind.DateOnly, typeof(DateOnly));
            public static OverlappedInfo TimeOnly = new OverlappedInfo(VariantKind.TimeOnly, typeof(TimeOnly));
            public static OverlappedInfo DateTime = new OverlappedInfo(VariantKind.DateTime, typeof(DateTime));
            public static OverlappedInfo TimeSpan = new OverlappedInfo(VariantKind.TimeSpan, typeof(TimeSpan));
        }

        private abstract class EnumInfo : OverlappedInfo
        {
            public EnumInfo(Type type)
                : base(VariantKind.Enum, type)
            {
            }

            public abstract bool IsType<TType>(long enumLongValue);
            public abstract object ConvertToObject(long enumLongValue);
            public abstract string ConvertToEnumString(long enumLongValue);
        }

        private class EnumInfo<TEnum> : EnumInfo
        {
            public EnumInfo()
                : base(typeof(TEnum))
            {
            }

            public override bool IsType<TType>(long enumLongValue) =>
                TryConvertLongToEnum<TEnum>(enumLongValue, out var enumValue) 
                && enumValue is TType;

            public override object ConvertToObject(long enumLongValue) =>
                TryConvertLongToEnum<TEnum>(enumLongValue, out var enumValue)
                    ? enumValue
                    : default!;

            public override string ConvertToEnumString(long enumLongValue) =>
                TryConvertLongToEnum<TEnum>(enumLongValue, out var enumValue) 
                    ? enumValue.ToString()!
                    : "";
        }

        private static ImmutableDictionary<Type, OverlappedInfo> s_typeToInfoMap =
            ImmutableDictionary<Type, OverlappedInfo>.Empty;

        private static OverlappedInfo GetEnumInfo<TEnum>()
        {
            var map = s_typeToInfoMap;
            var enumType = typeof(TEnum);
            if (!map.TryGetValue(enumType, out var info))
            {
                info = ImmutableInterlocked.GetOrAdd(ref s_typeToInfoMap, enumType, new EnumInfo<TEnum>());
            }
            return info;
        }

        private static long ConvertEnumToLong<TEnum>(TEnum value)
        {
            var enumType = typeof(TEnum);
            var kind = GetKind(enumType.GetEnumUnderlyingType());
            return kind switch
            {
                VariantKind.Int8 => Unsafe.As<TEnum, sbyte>(ref value),
                VariantKind.Int16 => Unsafe.As<TEnum, short>(ref value),
                VariantKind.Int32 => Unsafe.As<TEnum, int>(ref value),
                VariantKind.Int64 => Unsafe.As<TEnum, long>(ref value),
                VariantKind.UInt8 => Unsafe.As<TEnum, byte>(ref value),
                VariantKind.UInt16 => Unsafe.As<TEnum, ushort>(ref value),
                VariantKind.UInt32 => Unsafe.As<TEnum, uint>(ref value),
                VariantKind.UInt64 => unchecked((long)Unsafe.As<TEnum, ulong>(ref value)),
                _ => 0
            };
        }

        private static bool TryConvertLongToEnum<TEnum>(long value, [NotNullWhen(true)] out TEnum enumValue)
        {
            var enumType = typeof(TEnum);
            if (enumType.IsEnum)
            {
                var kind = GetKind(enumType.GetEnumUnderlyingType());
                switch (kind)
                {
                    case VariantKind.Int8 when value >= sbyte.MinValue && value <= sbyte.MaxValue:
                        var i8val = (sbyte)value;
                        enumValue = Unsafe.As<sbyte, TEnum>(ref i8val)!;
                        return true;
                    case VariantKind.Int16 when value >= short.MinValue && value <= short.MaxValue:
                        var i16val = (short)value;
                        enumValue = Unsafe.As<short, TEnum>(ref i16val)!;
                        return true;
                    case VariantKind.Int32 when value >= int.MinValue && value <= short.MaxValue:
                        var i32val = (int)value;
                        enumValue = Unsafe.As<int, TEnum>(ref i32val)!;
                        return true;
                    case VariantKind.Int64:
                        enumValue = Unsafe.As<long, TEnum>(ref value)!;
                        return true;
                    case VariantKind.UInt8 when value >= byte.MinValue && value <= byte.MaxValue:
                        var ui8val = (byte)value;
                        enumValue = Unsafe.As<byte, TEnum>(ref ui8val)!;
                        return true;
                    case VariantKind.UInt16 when value >= ushort.MinValue && value <= ushort.MaxValue:
                        var ui16val = (ushort)value;
                        enumValue = Unsafe.As<ushort, TEnum>(ref ui16val)!;
                        return true;
                    case VariantKind.UInt32 when value >= uint.MinValue && value <= uint.MaxValue:
                        var ui32val = (uint)value;
                        enumValue = Unsafe.As<uint, TEnum>(ref ui32val)!;
                        return true;
                    case VariantKind.UInt64:
                        var ui64val = unchecked((ulong)value);
                        enumValue = Unsafe.As<ulong, TEnum>(ref ui64val)!;
                        return true;
                };
            }

            enumValue = default!;
            return false;
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
    }
}