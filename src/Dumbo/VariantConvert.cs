using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Dumbo
{
    public static class VariantConvert
    {
        /// <summary>
        /// Returns the variant converted to the specified type, or throws <see cref="InvalidCastException"/> if it cannot be converted.
        /// </summary>
        public static T ConvertTo<T>(this in Variant variant) =>
            TryConvertTo<T>(variant, out var value)
                ? value
                : throw new InvalidCastException();

        /// <summary>
        /// Returns true if the variant can be converted to the specified type.
        /// </summary>
        public static bool TryConvertTo<T>(this in Variant variant, out T value)
        {
            var ttype = typeof(T);

            if (ttype.IsEnum)
            {
                return TryConvertToEnum(in variant, out value);
            }

            switch (Type.GetTypeCode(ttype))
            {
                case TypeCode.Boolean:
                    if (TryConvertToBool(in variant, out var boolValue)
                        && boolValue is T tboolValue)
                    {
                        value = tboolValue;
                        return true;
                    }
                    break;
                case TypeCode.SByte:
                    if (TryConvertToSByte(in variant, out var sbyteValue)
                        && sbyteValue is T tsbyteValue)
                    {
                        value = tsbyteValue;
                        return true;
                    }
                    break;
                case TypeCode.Byte:
                    if (TryConvertToByte(in variant, out var byteValue)
                        && byteValue is T tbyteValue)
                    {
                        value = tbyteValue;
                        return true;
                    }
                    break;
                case TypeCode.Int16:
                    if (TryConvertToInt16(in variant, out var shortValue)
                        && shortValue is T tshortValue)
                    {
                        value = tshortValue;
                        return true;
                    }
                    break;
                case TypeCode.UInt16:
                    if (TryConvertToUInt16(in variant, out var ushortValue)
                        && ushortValue is T tushortValue)
                    {
                        value = tushortValue;
                        return true;
                    }
                    break;
                case TypeCode.Int32:
                    if (TryConvertToInt32(in variant, out var intValue)
                        && intValue is T tintValue)
                    {
                        value = tintValue;
                        return true;
                    }
                    break;
                case TypeCode.UInt32:
                    if (TryConvertToUInt32(in variant, out var uintValue)
                        && uintValue is T tuintValue)
                    {
                        value = tuintValue;
                        return true;
                    }
                    break;
                case TypeCode.Int64:
                    if (TryConvertToInt64(in variant, out var longValue)
                        && longValue is T tlongValue)
                    {
                        value = tlongValue;
                        return true;
                    }
                    break;
                case TypeCode.UInt64:
                    if (TryConvertToUInt64(in variant, out var ulongValue)
                        && ulongValue is T tulongValue)
                    {
                        value = tulongValue;
                        return true;
                    }
                    break;
                case TypeCode.Single:
                    if (TryConvertToSingle(in variant, out var floatValue)
                        && floatValue is T tfloatValue)
                    {
                        value = tfloatValue;
                        return true;
                    }
                    break;
                case TypeCode.Double:
                    if (TryConvertToDouble(in variant, out var doubleValue)
                        && doubleValue is T tdoubleValue)
                    {
                        value = tdoubleValue;
                        return true;
                    }
                    break;
                case TypeCode.Decimal:
                    if (TryConvertToDecimal(in variant, out var decimalValue)
                        && decimalValue is T tdecimalValue)
                    {
                        value = tdecimalValue;
                        return true;
                    }
                    break;
                case TypeCode.Char:
                    if (TryConvertToChar(in variant, out var charValue)
                        && charValue is T tcharValue)
                    {
                        value = tcharValue;
                        return true;
                    }
                    break;
                case TypeCode.String:
                    if (variant.ToString() is T tstringValue)
                    {
                        value = tstringValue;
                        return true;
                    }
                    break;
                case TypeCode.Object:
                    if (ttype == typeof(Variant)
                        && variant is T tvariant)
                    {
                        value = tvariant;
                        return true;
                    }
                    else if (ttype == typeof(Decimal64)
                        && TryConvertToDecimal(in variant, out decimalValue)
                        && Decimal64.TryCreate(decimalValue, out var dec64Value)
                        && dec64Value is T tdec64Value)
                    {
                        value = tdec64Value;
                        return true;
                    }
                    else if (ttype == typeof(Rune)
                        && TryConvertToRune(in variant, out var runeValue)
                        && runeValue is T truneValue)
                    {
                        value = truneValue;
                        return true;
                    }
                    else if (ttype == typeof(DateTime)
                        && TryConvertToDateTime(in variant, out var dtValue)
                        && dtValue is T tdtValue)
                    {
                        value = tdtValue;
                        return true;
                    }
                    else if (ttype == typeof(TimeSpan)
                        && TryConvertToTimeSpan(in variant, out var tsValue)
                        && tsValue is T ttsValue)
                    {
                        value = ttsValue;
                        return true;
                    }
                    else if (ttype == typeof(DateOnly)
                        && TryConvertToDateOnly(in variant, out var doValue)
                        && doValue is T tdoValue)
                    {
                        value = tdoValue;
                        return true;
                    }
                    else if (ttype == typeof(TimeOnly)
                        && TryConvertToTimeOnly(in variant, out var toValue)
                        && toValue is T ttoValue)
                    {
                        value = ttoValue;
                        return true;
                    }
                    else if (variant.Type == typeof(string)
                        && TypeParser<T>.TryGetParser(out var parser))
                    {
                        return parser.TryParse(variant.ToString(), out value);
                    }
                    break;
            }

            return variant.TryGet(out value);
        }

        private static bool TryConvertToBool(in Variant variant, out bool value)
        {
            if (TryConvertToInt64(in variant, out var longValue))
            {
                value = longValue != 0;
                return true;
            }

            value = default;
            return false;
        }

        private static bool TryConvertToInt64(in Variant variant, out long value)
        {
            var vtype = variant.Type;

            if (vtype.IsEnum)
            {
                var reader = EnumConverter.GetConverter(vtype);
                value = reader.ConvertToLong(in variant);
                return true;
            }

            switch (Type.GetTypeCode(vtype))
            {
                case TypeCode.Boolean:
                    value = variant.Get<bool>() ? 1 : 0;
                    return true;
                case TypeCode.SByte:
                    value = variant.Get<sbyte>();
                    return true;
                case TypeCode.Byte:
                    value = variant.Get<byte>();
                    return true;
                case TypeCode.Int16:
                    value = variant.Get<short>();
                    return true;
                case TypeCode.UInt16:
                    value = variant.Get<ushort>();
                    return true;
                case TypeCode.Int32:
                    value = variant.Get<int>();
                    return true;
                case TypeCode.UInt32:
                    value = variant.Get<uint>();
                    return true;
                case TypeCode.Int64:
                    value = variant.Get<long>();
                    return true;
                case TypeCode.UInt64:
                    var ulongValue = variant.Get<ulong>();
                    if (ulongValue <= long.MaxValue)
                    {
                        value = (long)ulongValue;
                        return true;
                    }
                    break;
                case TypeCode.Char:
                    value = variant.Get<char>();
                    return true;
                case TypeCode.Single:
                    var floatValue = variant.Get<float>();
                    if (floatValue >= long.MinValue && floatValue <= long.MaxValue)
                    {
                        value = (long)floatValue;
                        return true;
                    }
                    break;
                case TypeCode.Double:
                    var doubleValue = variant.Get<double>();
                    if (doubleValue >= long.MinValue && doubleValue <= long.MaxValue)
                    {
                        value = (long)doubleValue;
                        return true;
                    }
                    break;
                case TypeCode.Decimal:
                    var decValue = variant.Get<decimal>();
                    if (decValue >= long.MinValue && decValue <= long.MaxValue)
                    {
                        value = (long)decValue;
                        return true;
                    }
                    break;
                case TypeCode.String:
                    var strValue = variant.Get<string>();
                    return Int64.TryParse(strValue, out value);
                case TypeCode.Object:
                    if (vtype == typeof(Decimal64))
                    {
                        decValue = variant.Get<Decimal64>().ToDecimal();
                        if (decValue >= long.MinValue && decValue <= long.MaxValue)
                        {
                            value = (long)decValue;
                            return true;
                        }
                    }
                    else if (vtype == typeof(Rune))
                    {
                        value = variant.Get<Rune>().Value;
                        return true;
                    }
                    break;
            }
            value = default;
            return false;
        }

        private static bool TryConvertToDecimal(in Variant variant, out decimal value)
        {
            var vtype = variant.Type;
            switch (Type.GetTypeCode(vtype))
            {
                case TypeCode.Boolean:
                    value = variant.Get<bool>() ? 1 : 0;
                    return true;
                case TypeCode.SByte:
                    value = variant.Get<sbyte>();
                    return true;
                case TypeCode.Byte:
                    value = variant.Get<byte>();
                    return true;
                case TypeCode.Int16:
                    value = variant.Get<short>();
                    return true;
                case TypeCode.UInt16:
                    value = variant.Get<ushort>();
                    return true;
                case TypeCode.Int32:
                    value = variant.Get<int>();
                    return true;
                case TypeCode.UInt32:
                    value = variant.Get<uint>();
                    return true;
                case TypeCode.Int64:
                    value = variant.Get<long>();
                    return true;
                case TypeCode.UInt64:
                    var ulongValue = variant.Get<ulong>();
                    if (ulongValue <= long.MaxValue)
                    {
                        value = (long)ulongValue;
                        return true;
                    }
                    break;
                case TypeCode.Single:
                    var floatValue = variant.Get<float>();
                    if (floatValue >= (float)decimal.MinValue && floatValue <= (float)decimal.MaxValue)
                    {
                        value = (decimal)floatValue;
                        return true;
                    }
                    break;
                case TypeCode.Double:
                    var doubleValue = variant.Get<double>();
                    if (doubleValue >= (double)decimal.MinValue && doubleValue <= (double)decimal.MaxValue)
                    {
                        value = (decimal)doubleValue;
                        return true;
                    }
                    break;
                case TypeCode.Decimal:
                    value = variant.Get<Decimal>();
                    return true;
                case TypeCode.String:
                    return decimal.TryParse(variant.ToString(), out value);
                case TypeCode.Object:
                    if (vtype == typeof(Decimal64))
                    {
                        value = variant.Get<Decimal64>().ToDecimal();
                        return true;
                    }
                    break;
                default:
                    if (TryConvertToInt64(in variant, out var longValue))
                    {
                        value = longValue;
                        return true;
                    }
                    break;
            }

            value = default;
            return false;
        }

        private static bool TryConvertToDouble(in Variant variant, out double value)
        {
            var vtype = variant.Type;
            switch (Type.GetTypeCode(vtype))
            {
                case TypeCode.Boolean:
                    value = variant.Get<bool>() ? 1 : 0;
                    return true;
                case TypeCode.SByte:
                    value = variant.Get<sbyte>();
                    return true;
                case TypeCode.Byte:
                    value = variant.Get<byte>();
                    return true;
                case TypeCode.Int16:
                    value = variant.Get<short>();
                    return true;
                case TypeCode.UInt16:
                    value = variant.Get<ushort>();
                    return true;
                case TypeCode.Int32:
                    value = variant.Get<int>();
                    return true;
                case TypeCode.UInt32:
                    value = variant.Get<uint>();
                    return true;
                case TypeCode.Int64:
                    value = variant.Get<long>();
                    return true;
                case TypeCode.UInt64:
                    var ulongValue = variant.Get<ulong>();
                    if (ulongValue <= long.MaxValue)
                    {
                        value = (long)ulongValue;
                        return true;
                    }
                    break;
                case TypeCode.Single:
                    value = variant.Get<float>();
                    return true;
                case TypeCode.Double:
                    value = variant.Get<double>();
                    return true;
                case TypeCode.String:
                    return double.TryParse(variant.ToString(), out value);
                default:
                    if (TryConvertToDecimal(in variant, out var decValue))
                    {
                        value = (double)decValue;
                        return true;
                    }
                    break;
            }

            value = default;
            return false;
        }

        private static bool TryConvertToSByte(in Variant variant, out sbyte value)
        {
            if (TryConvertToInt64(in variant, out var longValue)
                && longValue >= sbyte.MinValue && longValue <= sbyte.MaxValue)
            {
                value = (sbyte)longValue;
                return true;
            }

            value = default;
            return false;
        }

        private static bool TryConvertToByte(in Variant variant, out byte value)
        {
            if (TryConvertToInt64(in variant, out var longValue)
                && longValue >= byte.MinValue && longValue <= byte.MaxValue)
            {
                value = (byte)longValue;
                return true;
            }

            value = default;
            return false;
        }

        private static bool TryConvertToInt16(in Variant variant, out short value)
        {
            if (TryConvertToInt64(in variant, out var longValue)
                && longValue >= short.MinValue && longValue <= short.MaxValue)
            {
                value = (short)longValue;
                return true;
            }

            value = default;
            return false;
        }

        private static bool TryConvertToUInt16(in Variant variant, out ushort value)
        {
            if (TryConvertToInt64(in variant, out var longValue)
                && longValue >= ushort.MinValue && longValue <= ushort.MaxValue)
            {
                value = (ushort)longValue;
                return true;
            }

            value = default;
            return false;
        }

        private static bool TryConvertToInt32(in Variant variant, out int value)
        {
            if (TryConvertToInt64(in variant, out var longValue)
                && longValue >= int.MinValue && longValue <= int.MaxValue)
            {
                value = (int)longValue;
                return true;
            }

            value = default;
            return false;
        }

        private static bool TryConvertToUInt32(in Variant variant, out uint value)
        {
            if (TryConvertToInt64(in variant, out var longValue)
                && longValue >= uint.MinValue && longValue <= uint.MaxValue)
            {
                value = (uint)longValue;
                return true;
            }

            value = default;
            return false;
        }

        private static bool TryConvertToUInt64(in Variant variant, out ulong value)
        {
            var vtype = variant.Type;
            switch (Type.GetTypeCode(vtype))
            {
                case TypeCode.UInt64:
                    value = variant.Get<UInt64>();
                    return true;
                case TypeCode.String:
                    return ulong.TryParse(variant.ToString(), out value);
                default:
                    if (TryConvertToDecimal(in variant, out var decimalValue)
                        && decimalValue >= ulong.MinValue && decimalValue <= ulong.MaxValue)
                    {
                        value = (ulong)decimalValue;
                        return true;
                    }
                    break;
            }

            value = default;
            return false;
        }

        private static bool TryConvertToSingle(in Variant variant, out float value)
        {
            if (TryConvertToDouble(in variant, out var doubleValue)
                && doubleValue >= float.MinValue && doubleValue <= float.MaxValue)
            {
                value = (float)doubleValue;
                return true;
            }

            value = default;
            return false;
        }

        private static bool TryConvertToChar(in Variant variant, out char value)
        {
            if (TryConvertToInt64(in variant, out var longValue)
                && longValue >= char.MinValue && longValue <= char.MaxValue)
            {
                value = (char)longValue;
                return true;
            }
            else if (variant.Type == typeof(string))
            {
                var strValue = variant.ToString();
                if (strValue.Length == 1)
                {
                    value = strValue[0];
                    return true;
                }
            }

            value = default;
            return false;
        }

        private static bool TryConvertToRune(in Variant variant, out Rune value)
        {
            if (TryConvertToInt32(in variant, out var intValue)
                && Rune.IsValid(intValue))
            {
                value = new Rune(intValue);
                return true;
            }
            else if (variant.Type == typeof(string))
            {
                var strValue = variant.ToString();
                if (Rune.DecodeFromUtf16(strValue, out value, out var count) == System.Buffers.OperationStatus.Done
                    && count == strValue.Length)
                {
                    return true;
                }
            }

            value = default;
            return false;
        }

        private static bool TryConvertToDateTime(in Variant variant, out DateTime value)
        {
            if (variant.TryGet(out value))
            {
                return true;
            }
            else if (variant.TryGet<TimeSpan>(out var tsValue))
            {
                value = new DateTime(tsValue.Ticks);
                return true;
            }
            else if (variant.TryGet<DateOnly>(out var doValue))
            {
                value = doValue.ToDateTime(default);
                return true;
            }
            else if (variant.TryGet<TimeOnly>(out var toValue))
            {
                value = new DateTime(toValue.Ticks);
                return true;
            }
            else if (variant.TryGet<string>(out var strValue))
            {
                return DateTime.TryParse(strValue, out value);
            }

            value = default!;
            return false;
        }

        private static bool TryConvertToTimeSpan(in Variant variant, out TimeSpan value)
        {
            if (variant.TryGet(out value))
            {
                return true;
            }
            else if (variant.TryGet<DateTime>(out var dtValue))
            {
                value = new TimeSpan(dtValue.Ticks);
                return true;
            }
            else if (variant.TryGet<DateOnly>(out var doValue))
            {
                value = TimeSpan.MinValue;
                return true;
            }
            else if (variant.TryGet<TimeOnly>(out var toValue))
            {
                value = new TimeSpan(toValue.Ticks);
                return true;
            }
            else if (variant.TryGet<string>(out var strValue))
            {
                return TimeSpan.TryParse(strValue, out value);
            }

            value = default!;
            return false;
        }

        private static bool TryConvertToDateOnly(in Variant variant, out DateOnly value)
        {
            if (variant.TryGet(out value))
            {
                return true;
            }
            else if (variant.TryGet<DateTime>(out var dtValue))
            {
                value = DateOnly.FromDateTime(dtValue);
                return true;
            }
            else if (variant.TryGet<TimeSpan>(out var tsValue))
            {
                value = DateOnly.FromDayNumber(tsValue.Days);
                return true;
            }
            else if (variant.TryGet<TimeOnly>(out var toValue))
            {
                value = DateOnly.MinValue;
                return true;
            }
            else if (variant.TryGet<string>(out var strValue))
            {
                return DateOnly.TryParse(strValue, out value);
            }

            value = default!;
            return false;
        }

        private static bool TryConvertToTimeOnly(in Variant variant, out TimeOnly value)
        {
            if (variant.TryGet(out value))
            {
                return true;
            }
            else if (variant.TryGet<DateTime>(out var dtValue))
            {
                value = TimeOnly.FromDateTime(dtValue);
                return true;
            }
            else if (variant.TryGet<TimeSpan>(out var tsValue))
            {
                value = TimeOnly.FromTimeSpan(tsValue);
                return true;
            }
            else if (variant.TryGet<DateOnly>(out var toValue))
            {
                value = TimeOnly.MinValue;
                return true;
            }
            else if (variant.TryGet<string>(out var strValue))
            {
                return TimeOnly.TryParse(strValue, out value);
            }

            value = default!;
            return false;
        }

        private static bool TryConvertToEnum<TEnum>(in Variant variant, out TEnum value)
        {
            if (variant.Type == typeof(TEnum))
            {
                value = variant.Get<TEnum>();
                return true;
            }
            else if (variant.Type.IsEnum
                || variant.Type == typeof(string))
            {
                return EnumConverter.GetConverter<TEnum>().TryParse(variant.ToString(), out value);
            }
            else if (TryConvertToInt64(in variant, out var longValue))
            {
                return EnumConverter<TEnum>.TryConvertLongToEnum(longValue, out value);
            }
            else if (TryConvertToUInt64(in variant, out var ulongValue))
            {
                return EnumConverter<TEnum>.TryConvertLongToEnum(unchecked((long)ulongValue), out value);
            }

            value = default!;
            return false;
        }

        private abstract class EnumConverter
        {
            public abstract long ConvertToLong(in Variant variant);

            public static EnumConverter GetConverter(Type enumType)
            {
                if (!s_converterMap.TryGetValue(enumType, out var converter))
                {
                    var converterType = typeof(EnumConverterImpl<>).MakeGenericType(enumType);
                    var newConverter = (EnumConverter)Activator.CreateInstance(converterType)!;
                    converter = ImmutableInterlocked.GetOrAdd(ref s_converterMap, enumType, newConverter);
                }

                return converter;
            }

            public static EnumConverter<TEnum> GetConverter<TEnum>() =>
                (EnumConverter<TEnum>)GetConverter(typeof(TEnum));

            private static ImmutableDictionary<Type, EnumConverter> s_converterMap
                = ImmutableDictionary<Type, EnumConverter>.Empty;
        }

        private abstract class EnumConverter<TEnum> : EnumConverter
        {
            public override long ConvertToLong(in Variant variant) =>
                ConvertEnumToLong(variant.Get<TEnum>());

            public abstract bool TryParse(string text, out TEnum value);

            private static long ConvertEnumToLong(TEnum value)
            {
                var enumType = typeof(TEnum);
                var underlyngType = enumType.GetEnumUnderlyingType();
                return Type.GetTypeCode(underlyngType) switch
                {
                    TypeCode.SByte => Unsafe.As<TEnum, sbyte>(ref value),
                    TypeCode.Int16 => Unsafe.As<TEnum, short>(ref value),
                    TypeCode.Int32 => Unsafe.As<TEnum, int>(ref value),
                    TypeCode.Int64 => Unsafe.As<TEnum, long>(ref value),
                    TypeCode.Byte => Unsafe.As<TEnum, byte>(ref value),
                    TypeCode.UInt16 => Unsafe.As<TEnum, ushort>(ref value),
                    TypeCode.UInt32 => Unsafe.As<TEnum, uint>(ref value),
                    TypeCode.UInt64 => unchecked((long)Unsafe.As<TEnum, ulong>(ref value)),
                    _ => 0
                };
            }

            public static bool TryConvertLongToEnum(long value, [NotNullWhen(true)] out TEnum enumValue)
            {
                var enumType = typeof(TEnum);
                if (enumType.IsEnum)
                {
                    var underlyingType = enumType.GetEnumUnderlyingType();
                    switch (Type.GetTypeCode(underlyingType))
                    {
                        case TypeCode.SByte when value >= sbyte.MinValue && value <= sbyte.MaxValue:
                            var i8val = (sbyte)value;
                            enumValue = Unsafe.As<sbyte, TEnum>(ref i8val)!;
                            return true;
                        case TypeCode.Int16 when value >= short.MinValue && value <= short.MaxValue:
                            var i16val = (short)value;
                            enumValue = Unsafe.As<short, TEnum>(ref i16val)!;
                            return true;
                        case TypeCode.Int32 when value >= int.MinValue && value <= short.MaxValue:
                            var i32val = (int)value;
                            enumValue = Unsafe.As<int, TEnum>(ref i32val)!;
                            return true;
                        case TypeCode.Int64:
                            enumValue = Unsafe.As<long, TEnum>(ref value)!;
                            return true;
                        case TypeCode.Byte when value >= byte.MinValue && value <= byte.MaxValue:
                            var ui8val = (byte)value;
                            enumValue = Unsafe.As<byte, TEnum>(ref ui8val)!;
                            return true;
                        case TypeCode.UInt16 when value >= ushort.MinValue && value <= ushort.MaxValue:
                            var ui16val = (ushort)value;
                            enumValue = Unsafe.As<ushort, TEnum>(ref ui16val)!;
                            return true;
                        case TypeCode.UInt32 when value >= uint.MinValue && value <= uint.MaxValue:
                            var ui32val = (uint)value;
                            enumValue = Unsafe.As<uint, TEnum>(ref ui32val)!;
                            return true;
                        case TypeCode.UInt64:
                            var ui64val = unchecked((ulong)value);
                            enumValue = Unsafe.As<ulong, TEnum>(ref ui64val)!;
                            return true;
                    };
                }

                enumValue = default!;
                return false;
            }
        }

        private sealed class EnumConverterImpl<TEnum> : EnumConverter<TEnum>
            where TEnum : struct, System.Enum
        {
            public override bool TryParse(string text, out TEnum value) =>
                Enum.TryParse<TEnum>(text, ignoreCase: true, out value);
        }

        private abstract class TypeParser<T>
        {
            private static readonly Type? _parserType;
            private static TypeParser<T>? _instance;

            static TypeParser()
            {
                if (typeof(T).IsAssignableTo(typeof(IParsable<>).MakeGenericType(typeof(T))))
                {
                    _parserType = typeof(StringParsableParser<>).MakeGenericType(typeof(T));
                }
                else if (typeof(T).IsAssignableTo(typeof(ISpanParsable<>).MakeGenericType(typeof(T))))
                {
                    _parserType = typeof(SpanParsableParser<>).MakeGenericType(typeof(T));
                }
            }

            public static bool TryGetParser([NotNullWhen(true)] out TypeParser<T>? parser)
            {
                if (_instance == null && _parserType != null)
                {
                    var newParser = (TypeParser<T>)Activator.CreateInstance(_parserType)!;
                    Interlocked.CompareExchange(ref _instance, newParser, null);
                }

                parser = _instance;
                return parser is not null;
            }

            public abstract bool TryParse(string text, out T value);
        }

        private sealed class StringParsableParser<T> : TypeParser<T>
            where T : IParsable<T>
        {
            public override bool TryParse(string text, out T value) =>
                T.TryParse(text, null, out value!);
        }

        private sealed class SpanParsableParser<T> : TypeParser<T>
            where T : ISpanParsable<T>
        {
            public override bool TryParse(string text, out T value) =>
                T.TryParse(text, null, out value!);
        }
    }
}
