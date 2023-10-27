#if !VARIANT2
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;

namespace Dumbo;

/// <summary>
/// A type union for an instance of any type that avoids boxing of most common primitive values.
/// </summary>
[DebuggerDisplay("{DebugText}")]
public readonly struct Variant : ITypeUnion<Variant> //, IEquatable<Variant>
{
    private string DebugText => ToString();

    private readonly object? _refValue;
    private readonly Overlapped _structValue;

    private Variant(object? refValue, Overlapped structValue)
    {
        _refValue = refValue;
        _structValue = structValue;
    }

    public static readonly Variant Null =
        new Variant(null, default);

    public static Variant Create(bool value) => 
        new Variant(VariantInfo.Bool, new Overlapped { Bool = value });

    public static Variant Create(sbyte value) =>
        new Variant(VariantInfo.Int8, new Overlapped { Int8 = value });

    public static Variant Create(short value) =>
        new Variant(VariantInfo.Int16, new Overlapped { Int16 = value });

    public static Variant Create(int value) => 
        new Variant(VariantInfo.Int32, new Overlapped { Int32 = value });

    public static Variant Create(long value) =>
        new Variant(VariantInfo.Int64, new Overlapped { Int64 = value });

    public static Variant Create(byte value) =>
        new Variant(VariantInfo.UInt8, new Overlapped { UInt8 = value });

    public static Variant Create(ushort value) =>
        new Variant(VariantInfo.UInt16, new Overlapped { UInt16 = value });

    public static Variant Create(uint value) =>
        new Variant(VariantInfo.UInt32, new Overlapped { UInt32 = value });

    public static Variant Create(ulong value) =>
        new Variant(VariantInfo.UInt64, new Overlapped { UInt64 = value });

    public static Variant Create(float value) =>
        new Variant(VariantInfo.Float32, new Overlapped { Float32 = value });

    public static Variant Create(double value) =>
        new Variant(VariantInfo.Float64, new Overlapped { Float64 = value });

    public static Variant Create(char value) =>
        new Variant(VariantInfo.Char16, new Overlapped { Char16 = value });

    public static Variant Create(Rune value) =>
        new Variant(VariantInfo.Char32, new Overlapped { Int32 = value.Value });

    public static Variant Create(Decimal64 value) =>
        new Variant(VariantInfo.Decimal64, new Overlapped { Int64 = value.GetBits() });

    public static Variant Create(DateOnly value) =>
        new Variant(VariantInfo.DateOnly, new Overlapped { Int32 = value.DayNumber });

    public static Variant Create(TimeOnly value) =>
        new Variant(VariantInfo.TimeOnly, new Overlapped { Int64 = value.Ticks });

    public static Variant Create(DateTime value) =>
        new Variant(VariantInfo.DateTime, new Overlapped { Int64 = value.Ticks });

    public static Variant Create(TimeSpan value) => 
        new Variant(VariantInfo.TimeSpan, new Overlapped { Int64 = value.Ticks });

    public static Variant Create(string value) =>
        new Variant(value, new Overlapped { Int64 = 0 });

    public static Variant Create(decimal value)
    {
        // if decimal fits into Decimal64 then store it as such
        if (Decimal64.TryConvert(value, out var dec64))
        {
            return new Variant(VariantInfo.Decimal128, new Overlapped { Int64 = dec64.GetBits() });
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
        var type = typeof(T);

        if (type.IsValueType)
        {
            switch (GetKind(type))
            {
                case VariantKind.Bool:
                    if (value is bool boolVal)
                        return Create(boolVal);
                    break;
                case VariantKind.Int8:
                    if (value is sbyte sbyteVal)
                        return Create(sbyteVal);
                    break;
                case VariantKind.UInt8:
                    if (value is byte byteVal)
                        return Create(byteVal);
                    break;
                case VariantKind.Int16:
                    if (value is short shortVal)
                        return Create(shortVal);
                    break;
                case VariantKind.UInt16:
                    if (value is ushort ushortVal)
                        return Create(ushortVal);
                    break;
                case VariantKind.Int32:
                    if (value is int intVal)
                        return Create(intVal);
                    break;
                case VariantKind.UInt32:
                    if (value is uint uintVal)
                        return Create(uintVal);
                    break;
                case VariantKind.Char16:
                    if (value is char charVal)
                        return Create(charVal);
                    break;
                case VariantKind.Char32:
                    if (value is Rune runeVal)
                        return Create(runeVal);
                    break;
                case VariantKind.Float32:
                    if (value is float floatVal)
                        return Create(floatVal);
                    break;
                case VariantKind.Float64:
                    if (value is double doubleVal)
                        return Create(doubleVal);
                    break;
                case VariantKind.Decimal64:
                    if (value is Decimal64 dec64Val)
                        return Create(dec64Val);
                    break;
                case VariantKind.Decimal128:
                    if (value is decimal decimalVal)
                        return Create(decimalVal);
                    break;
                case VariantKind.DateOnly:
                    if (value is DateOnly dateVal)
                        return Create(dateVal);
                    break;
                case VariantKind.TimeOnly:
                    if (value is TimeOnly timeVal)
                        return Create(timeVal);
                    break;
                case VariantKind.DateTime:
                    if (value is DateTime dtVal)
                        return Create(dtVal);
                    break;
                case VariantKind.TimeSpan:
                    if (value is TimeSpan tsVal)
                        return Create(tsVal);
                    break;
                case VariantKind.Enum:
                    var info = VariantInfo.GetEnumInfo<T>();
                    var val = ConvertEnumToLong(value);
                    return new Variant(info, new Overlapped { Int64 = val });
            }

            switch (value)
            {
                case Variant vVal:
                    return vVal;
                case ITypeUnion tu:
                    if (tu.TryGet<Variant>(out var tuv))
                        return tuv;
                    else if (tu.TryGet<object>(out var ov))
                        return Create<object>(ov);
                    else
                        return Null;
            };
        }

        return new Variant(value, new Overlapped { UInt64 = 0L });
    }

    public static bool TryCreate<T>(T value, [NotNullWhen(true)] out Variant result)
    {
        result = Create(value);
        return true;
    }

    public bool IsNull => 
        _refValue == null;

    public Type? Type =>
        _refValue is VariantInfo info
            ? info.Type
            : _refValue?.GetType();

    public bool IsType<T>() =>
        _refValue is VariantInfo info
            ? info.IsType<T>(in this)
            : _refValue is T;

    public bool TryGet<T>([NotNullWhen(true)] out T value)
    {
        if (_refValue is VariantInfo info)
        {
            return info.TryGet<T>(in this, out value);
        }
        else if (_refValue is T tval)
        {
            value = tval;
            return true;
        }
        value = default!;
        return false;
    }

    public T AsType<T>() => 
        TryGet<T>(out var value) ? value : default!;

    public T Get<T>() =>
        TryGet<T>(out var value) ? value : throw new InvalidCastException($"Cannot cast to type: {typeof(T).Name}");

    public object? ToObject() =>
        _refValue is VariantInfo info
            ? info.ToObject(in this)
            : _refValue;

    public override string ToString() =>
        _refValue is VariantInfo info
            ? info.ToString(in this)
            : _refValue?.ToString() ?? "";

    public bool TryConvertToBool(out bool value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToBool(in this, out value))
        {
            return true;
        }
        else if (_refValue is string str)
        {
            return bool.TryParse(str, out value);
        }

        value = default;
        return false;
    }

    public bool ToBool() =>
        TryConvertToBool(out var value)
            ? value
            : throw new InvalidCastException();

    public bool TryConvertToInt8(out sbyte value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToInt8(in this, out value))
        {
            return true;
        }
        else if (_refValue is string str)
        {
            return sbyte.TryParse(str, out value);
        }

        value = default;
        return false;
    }

    public sbyte ToInt8() =>
        TryConvertToInt8(out var value)
            ? value
            : throw new InvalidCastException();

    public bool TryConvertToInt16(out short value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToInt16(in this, out value))
        {
            return true;
        }
        else if (_refValue is string str)
        {
            return short.TryParse(str, out value);
        }

        value = default;
        return false;
    }

    public short ToInt16() =>
        TryConvertToInt16(out var value)
            ? value
            : throw new InvalidCastException();

    public bool TryConvertToInt32(out int value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToInt32(in this, out value))
        {
            return true;
        }
        else if (_refValue is string str)
        {
            return int.TryParse(str, out value);
        }

        value = default;
        return false;
    }

    public int ToInt32() =>
        TryConvertToInt32(out var value)
            ? value
            : throw new InvalidCastException();

    public bool TryConvertToInt64(out long value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToInt64(in this, out value))
        {
            return true;
        }
        else if (_refValue is string str)
        {
            return long.TryParse(str, out value);
        }

        value = default;
        return false;
    }

    public long ToInt64() =>
        TryConvertToInt64(out var value)
            ? value
            : throw new InvalidCastException();

    public bool TryConvertToByte(out byte value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToUInt8(in this, out value))
        {
            return true;
        }
        else if (_refValue is string str)
        {
            return byte.TryParse(str, out value);
        }

        value = default;
        return false;
    }

    public byte ToByte() =>
        TryConvertToByte(out var value)
            ? value
            : throw new InvalidCastException();

    public bool TryConvertToUInt16(out ushort value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToUInt16(in this, out value))
        {
            return true;
        }
        else if (_refValue is string str)
        {
            return ushort.TryParse(str, out value);
        }

        value = default;
        return false;
    }

    public ushort ToUInt16() =>
        TryConvertToUInt16(out var value)
            ? value
            : throw new InvalidCastException();

    public bool TryConvertToUInt32(out uint value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToUInt32(in this, out value))
        {
            return true;
        }
        else if (_refValue is string str)
        {
            return uint.TryParse(str, out value);
        }

        value = default;
        return false;
    }

    public uint ToUInt32() =>
        TryConvertToUInt32(out var value)
            ? value
            : throw new InvalidCastException();

    public bool TryConvertToUInt64(out ulong value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToUInt64(in this, out value))
        {
            return true;
        }
        else if (_refValue is string str)
        {
            return ulong.TryParse(str, out value);
        }

        value = default;
        return false;
    }

    public ulong ToUInt64() =>
        TryConvertToUInt64(out var value)
            ? value
            : throw new InvalidCastException();

    public bool TryConvertToDecimal64(out Decimal64 value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToDecimal64(in this, out value))
        {
            return true;
        }
        else if (_refValue is string str)
        {
            return Decimal64.TryParse(str, out value);
        }

        value = default;
        return false;
    }

    public Decimal64 ToDecimal64() =>
        TryConvertToDecimal64(out var value)
            ? value
            : throw new InvalidCastException();

    public bool TryConvertToDecimal(out decimal value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToDecimal(in this, out value))
        {
            return true;
        }
        else if (_refValue is decimal dval)
        {
            value = dval;
            return true;
        }
        else if (_refValue is string str)
        {
            return decimal.TryParse(str, out value);
        }

        value = default;
        return false;
    }

    public decimal ToDecimal() =>
        TryConvertToDecimal(out var value)
            ? value
            : throw new InvalidCastException();

    public bool TryConvertToSingle(out float value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToFloat32(in this, out value))
        {
            return true;
        }
        else if (_refValue is string str)
        {
            return float.TryParse(str, out value);
        }

        value = default;
        return false;
    }

    public float ToSingle() =>
        TryConvertToSingle(out var value)
            ? value
            : throw new InvalidCastException();


    public bool TryConvertToDouble(out double value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToFloat64(in this, out value))
        {
            return true;
        }
        else if (_refValue is string str)
        {
            return double.TryParse(str, out value);
        }

        value = default;
        return false;
    }

    public double ToDouble() =>
        TryConvertToDouble(out var value)
            ? value
            : throw new InvalidCastException();

    public bool TryConvertToChar(out char value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToChar16(in this, out value))
        {
            return true;
        }
        else if (_refValue is string str)
        {
            return char.TryParse(str, out value);
        }

        value = default;
        return false;
    }

    public char ToChar() =>
        TryConvertToChar(out var value)
            ? value
            : throw new InvalidCastException();

    public bool TryConvertToRune(out Rune value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToChar32(in this, out value))
        {
            return true;
        }

        value = default;
        return false;
    }

    public Rune ToRune() =>
        TryConvertToRune(out var value)
            ? value
            : throw new InvalidCastException();

    public bool TryConvertToDateOnly(out DateOnly value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToDateOnly(in this, out value))
        {
            return true;
        }
        else if (_refValue is string str)
        {
            return DateOnly.TryParse(str, out value);
        }    

        value = default;
        return false;
    }

    public DateOnly ToDateOnly() =>
        TryConvertToDateOnly(out var value)
            ? value
            : throw new InvalidCastException();

    public bool TryConvertToTimeOnly(out TimeOnly value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToTimeOnly(in this, out value))
        {
            return true;
        }
        else if (_refValue is string str)
        {
            return TimeOnly.TryParse(str, out value);
        }

        value = default;
        return false;
    }

    public TimeOnly ToTimeOnly() =>
        TryConvertToTimeOnly(out var value)
            ? value
            : throw new InvalidCastException();

    public bool TryConvertToDateTime(out DateTime value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToDateTime(in this, out value))
        {
            return true;
        }
        else if (_refValue is string str)
        {
            return DateTime.TryParse(str, out value);
        }

        value = default;
        return false;
    }

    public DateTime ToDateTime() =>
        TryConvertToDateTime(out var value)
            ? value
            : throw new InvalidCastException();

    public bool TryConvertToTimeSpan(out TimeSpan value)
    {
        if (_refValue is VariantInfo info
            && info.TryConvertToTimeSpan(in this, out value))
        {
            return true;
        }
        else if (_refValue is string str)
        {
            return TimeSpan.TryParse(str, out value);
        }

        value = default;
        return false;
    }

    public TimeSpan ToTimeSpan() =>
        TryConvertToTimeSpan(out var value)
            ? value
            : throw new InvalidCastException();

    public bool TryConvertToGuid(out Guid value)
    {
        if (_refValue is Guid guid)
        {
            value = guid;
            return true;
        }
        else if (_refValue is string str)
        {
            return Guid.TryParse(str, out value);
        }

        value = default;
        return false;       
    }

    public Guid ToGuid() =>
        TryConvertToGuid(out var value)
            ? value
            : throw new InvalidCastException();

    /// <summary>
    /// Attempts to convert the value into the specified type.
    /// Returns true if the value is successfully converted.
    /// </summary>
    public bool TryConvertTo<T>([NotNullWhen(true)] out T converted)
    {
        var kind = GetKind(typeof(T));

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
            case VariantKind.UInt8 when TryConvertToByte(out var u8val) && u8val is T tval:
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
            case VariantKind.Float32 when TryConvertToSingle(out var f32val) && f32val is T tval:
                converted = tval;
                return true;
            case VariantKind.Float64 when TryConvertToDouble(out var f64val) && f64val is T tval:
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
            case VariantKind.Enum when TryConvertToInt64(out var eval) && TryConvertLongToEnum(eval, out T tval):
                converted = tval;
                return true;
            case VariantKind.String when ToString() is T tval:
                converted = tval;
                return true;
            case VariantKind.Object when _refValue is T tval:
                converted = tval;
                return true;                
            default:
                if (typeof(T) == typeof(object)
                    && ToObject() is T oval)
                {
                    converted = oval;
                    return true;
                }
                else if (typeof(T) == typeof(Guid)
                    && ToGuid() is T guid)
                {
                    converted = guid;
                    return true;
                }
                break;
        };

        converted = default!;
        return false;
    }

    public T ConvertTo<T>() =>
        TryConvertTo<T>(out var value) 
            ? value 
            : throw new InvalidCastException();

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
        else if (_refValue is string str)
        {
            return T.TryParse(str, null, out converted!);
        }

        converted = default!;
        return false;
    }

    public T ConvertToParsable<T>() where T : IParsable<T> =>
        TryConvertToParsable<T>(out var value) 
            ? value 
            : throw new InvalidCastException();

    public bool Equals(Variant other) =>
        _refValue is VariantInfo info
            ? info.Equals(in this, in other)
            : object.Equals(_refValue, other._refValue);

    public bool Equals<T>(T? other)
    {
        if (other is Variant vother)
        {
            return Equals(vother);
        }
        else if (TryGet<T>(out var tval))
        {
            return tval.Equals(other);
        }
        else if (other is ITypeUnion union && union.TryGet<object>(out var obj))
        {
            return object.Equals(ToObject(), obj);
        }

        return false;
    }

    public override bool Equals([NotNullWhen(true)] object? other) =>
        Equals<object?>(other);

    public override int GetHashCode() =>
        _refValue is VariantInfo info
            ? info.GetHashCode(in this)
            : _refValue?.GetHashCode() ?? 0;

    // operators
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

    public static explicit operator bool(Variant value) => value.ToBool();
    public static implicit operator sbyte(Variant value) => value.ToInt8();
    public static implicit operator short(Variant value) => value.ToInt16();
    public static implicit operator int(Variant value) => value.ToInt32();
    public static implicit operator long(Variant value) => value.ToInt64();
    public static implicit operator byte(Variant value) => value.ToByte();
    public static implicit operator ushort(Variant value) => value.ToUInt16();
    public static implicit operator uint(Variant value) => value.ToUInt32();
    public static implicit operator ulong(Variant value) => value.ToUInt64();
    public static implicit operator float(Variant value) => value.ToSingle();
    public static implicit operator double(Variant value) => value.ToDouble();
    public static implicit operator Decimal64(Variant value) => value.ToDecimal64();
    public static implicit operator decimal(Variant value) => value.ToDecimal();
    public static implicit operator char(Variant value) => value.ToChar();
    public static implicit operator Rune(Variant value) => value.ToRune();
    public static implicit operator String(Variant value) => value.ToString();
    public static implicit operator DateOnly(Variant value) => value.ToDateOnly();
    public static implicit operator TimeOnly(Variant value) => value.ToTimeOnly();
    public static implicit operator DateTime(Variant value) => value.ToDateTime();
    public static implicit operator TimeSpan(Variant value) => value.ToTimeSpan();
    public static implicit operator Guid(Variant value) => value.ToGuid();

    public static explicit operator bool?(Variant value) => value. IsNull ? default : value.ToBool();
    public static implicit operator sbyte?(Variant value) => value.IsNull ? default : value.ToInt8();
    public static implicit operator short?(Variant value) => value.IsNull ? default : value.ToInt16();
    public static implicit operator int?(Variant value) => value.IsNull ? default : value.ToInt32();
    public static implicit operator long?(Variant value) => value.IsNull ? default : value.ToInt64();
    public static implicit operator byte?(Variant value) => value.IsNull ? default : value.ToByte();
    public static implicit operator ushort?(Variant value) => value.IsNull ? default : value.ToUInt16();
    public static implicit operator uint?(Variant value) => value.IsNull ? default : value.ToUInt32();
    public static implicit operator ulong?(Variant value) => value.IsNull ? default : value.ToUInt64();
    public static implicit operator float?(Variant value) => value.IsNull ? default : value.ToSingle();
    public static implicit operator double?(Variant value) => value.IsNull ? default : value.ToDouble();
    public static implicit operator Decimal64?(Variant value) => value.IsNull ? default : value.ToDecimal64();
    public static implicit operator decimal?(Variant value) => value.IsNull ? default : value.ToDecimal();
    public static implicit operator char?(Variant value) => value.IsNull ? default : value.ToChar();
    public static implicit operator Rune?(Variant value) => value.IsNull ? default : value.ToRune();
    public static implicit operator DateOnly?(Variant value) => value.IsNull ? default : value.ToDateOnly();
    public static implicit operator TimeOnly?(Variant value) => value.IsNull ? default : value.ToTimeOnly();
    public static implicit operator DateTime?(Variant value) => value.IsNull ? default : value.ToDateTime();
    public static implicit operator TimeSpan?(Variant value) => value.IsNull ? default : value.ToTimeSpan();
    public static implicit operator Guid?(Variant value) => value.IsNull ? default : value.ToGuid();

    private static VariantKind GetKind(Type type)
    {
        var kind = GetKind(Type.GetTypeCode(type));       
        if (kind != VariantKind.Object)
            return kind;

        if (type.IsEnum)
            return VariantKind.Enum;

        if (s_typeToKindMap.TryGetValue(type, out kind))
            return kind;

        return VariantKind.Object;
    }

    private static VariantKind GetKind(TypeCode code)
    {
        switch (code)
        {
            case TypeCode.Boolean:
                return VariantKind.Bool;
            case TypeCode.SByte:
                return VariantKind.Int8;
            case TypeCode.Byte:
                return VariantKind.UInt8;
            case TypeCode.Int16:
                return VariantKind.Int16;
            case TypeCode.UInt16:
                return VariantKind.UInt16;
            case TypeCode.Int32:
                return VariantKind.Int32;
            case TypeCode.UInt32:
                return VariantKind.UInt32;
            case TypeCode.Char:
                return VariantKind.Char16;
            case TypeCode.Single:
                return VariantKind.Float32;
            case TypeCode.Double:
                return VariantKind.Float64;
            case TypeCode.Decimal:
                return VariantKind.Decimal128;
            case TypeCode.DateTime:
                return VariantKind.DateTime;
            case TypeCode.String:
                return VariantKind.String;
            case TypeCode.DBNull:
                return VariantKind.Null;
            default:
                return VariantKind.Object;
        }
    }

    private static readonly Dictionary<Type, VariantKind> s_typeToKindMap = 
        new Dictionary<Type, VariantKind>
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

        public Rune Char32
        {
            get => new Rune(Int32);
            set => Int32 = value.Value;
        }

        public DateOnly DateOnly
        {
            get => DateOnly.FromDayNumber(Int32);
            set => Int32 = value.DayNumber;
        }

        public TimeOnly TimeOnly
        {
            get => new TimeOnly(Int64);
            set => Int64 = value.Ticks;
        }

        public DateTime DateTime
        {
            get => new DateTime(Int64);
            set => Int64 = value.Ticks;
        }

        public TimeSpan TimeSpan
        {
            get => TimeSpan.FromTicks(Int64);
            set => Int64 = value.Ticks;
        }

        public Decimal64 Decimal64
        {
            get => Decimal64.FromBits(Int64);
            set => Int64 = value.GetBits();
        }
    }

    private abstract class VariantInfo
    {
        public abstract Type Type { get; }
        public abstract VariantKind Kind { get; }
        public abstract bool IsType<T>(in Variant variant);
        public abstract bool TryGet<T>(in Variant variant, out T value);
        public abstract string ToString(in Variant variant);
        public abstract object ToObject(in Variant variant);

        public virtual int GetHashCode(in Variant variant) =>
            variant._structValue.Int64.GetHashCode();

        public virtual bool Equals(in Variant variant, in Variant other)
        {
            return false;
        }

        public virtual bool TryConvertToInt64(in Variant variant, out long value) 
        { 
            value = default; 
            return false; 
        }

        public virtual bool TryConvertToDecimal(in Variant variant, out decimal value)
        {
            if (TryConvertToInt64(in variant, out var int64Value))
            {
                value = int64Value;
                return true;
            }
            value = default;
            return false;
        }

        public virtual bool TryConvertToBool(in Variant variant, out bool value)
        {
            if (TryConvertToInt64(in variant, out var longVal))
            {
                value = longVal != 0;
                return true;
            }
            value = default;
            return false;
        }

        public virtual bool TryConvertToInt8(in Variant variant, out sbyte value)
        {
            if (TryConvertToInt64(in variant, out var val)
                && val >= sbyte.MinValue && val <= sbyte.MaxValue)
            {
                value = (sbyte)val;
                return true;
            }
            value = default;
            return false;
        }

        public virtual bool TryConvertToInt16(in Variant variant, out short value)
        {
            if (TryConvertToInt64(in variant, out var val)
                && val >= short.MinValue && val <= short.MaxValue)
            {
                value = (short)val;
                return true;
            }
            value = default;
            return false;
        }

        public virtual bool TryConvertToInt32(in Variant variant, out int value)
        {
            if (TryConvertToInt64(in variant, out var val)
                && val >= int.MinValue && val <= int.MaxValue)
            {
                value = (int)val;
                return true;
            }
            value = default;
            return false;
        }

        public virtual bool TryConvertToUInt8(in Variant variant, out byte value)
        {
            if (TryConvertToInt64(in variant, out var val)
                && val >= byte.MinValue && val <= byte.MaxValue)
            {
                value = (byte)val;
                return true;
            }
            value = default;
            return false;
        }

        public virtual bool TryConvertToUInt16(in Variant variant, out ushort value)
        {
            if (TryConvertToInt64(in variant, out var val)
                && val >= ushort.MinValue && val <= ushort.MaxValue)
            {
                value = (ushort)val;
                return true;
            }
            value = default;
            return false;
        }

        public virtual bool TryConvertToUInt32(in Variant variant, out uint value)
        {
            if (TryConvertToInt64(in variant, out var val)
                && val >= uint.MinValue && val <= uint.MaxValue)
            {
                value = (uint)val;
                return true;
            }
            value = default;
            return false;
        }

        public virtual bool TryConvertToUInt64(in Variant variant, out ulong value)
        {
            if (TryConvertToDecimal(in variant, out var val)
                && val >= ulong.MinValue && val <= ulong.MaxValue)
            {
                value = (ulong)val;
                return true;
            }
            value = default;
            return false;
        }

        public virtual bool TryConvertToDecimal64(in Variant variant, out Decimal64 value)
        {
            if (TryConvertToDecimal(in variant, out var decVal))
            {
                return Dumbo.Decimal64.TryConvert(decVal, out value);
            }
            value = default;
            return false;
        }

        public virtual bool TryConvertToFloat32(in Variant variant, out float value)
        {
            if (TryConvertToDecimal(in variant, out var decVal))
            {
                value = (float)decVal;
                return true;
            }
            value = default;
            return false;
        }

        public virtual bool TryConvertToFloat64(in Variant variant, out double value)
        {
            if (TryConvertToDecimal(in variant, out var decVal))
            {
                value = (double)decVal;
                return true;
            }
            value = default;
            return false;
        }

        public virtual bool TryConvertToChar16(in Variant variant, out char value)
        {
            if (TryConvertToInt16(in variant, out var shortValue))
            {
                value = (char)shortValue;
                return true;
            }
            value = default;
            return false;
        }

        public virtual bool TryConvertToChar32(in Variant variant, out Rune value)
        {
            if (TryConvertToInt32(in variant, out var intValue)
                && intValue >= int.MinValue && intValue <= int.MaxValue
                && Rune.IsValid(intValue))
            {
                value = new Rune(intValue);
                return true;
            }
            value = default;
            return false;
        }

        public virtual bool TryConvertToDateOnly(in Variant variant, out DateOnly converted)
        {
            converted = default;
            return false;
        }

        public virtual bool TryConvertToTimeOnly(in Variant variant, out TimeOnly converted)
        {
            converted = default;
            return false;
        }

        public virtual bool TryConvertToDateTime(in Variant variant, out DateTime converted)
        {
            converted = default;
            return false;
        }

        public virtual bool TryConvertToTimeSpan(in Variant variant, out TimeSpan converted)
        {
            converted = default;
            return false;
        }

        public static VariantInfo Bool = new BoolInfo();
        public static VariantInfo Int8 = new Int8Info();
        public static VariantInfo Int16 = new Int16Info();
        public static VariantInfo Int32 = new Int32Info();
        public static VariantInfo Int64 = new Int64Info();
        public static VariantInfo UInt8 = new UInt8Info();
        public static VariantInfo UInt16 = new UInt16Info();
        public static VariantInfo UInt32 = new UInt32Info();
        public static VariantInfo UInt64 = new UInt64Info();
        public static VariantInfo Float32 = new Float32Info();
        public static VariantInfo Float64 = new Float64Info();
        public static VariantInfo Char16 = new Char16Info();
        public static VariantInfo Char32 = new Char32Info();
        public static VariantInfo Decimal64 = new Decimal64Info();
        public static VariantInfo Decimal128 = new Decimal128Info();
        public static VariantInfo DateOnly = new DateOnlyInfo();
        public static VariantInfo TimeOnly = new TimeOnlyInfo();
        public static VariantInfo DateTime = new DateTimeInfo();
        public static VariantInfo TimeSpan = new TimeSpanInfo();

        private sealed class BoolInfo : VariantInfo
        {
            public override Type Type => typeof(bool);
            public override VariantKind Kind => VariantKind.Bool;
            public bool GetValue(in Variant variant) => variant._structValue.Bool;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(bool) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (GetValue(in variant) is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToInt64(in Variant variant, out long int64Value) 
            { 
                int64Value = GetValue(in variant) ? 1 : 0; 
                return true; 
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToBool(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private sealed class Int8Info : VariantInfo
        {
            public override Type Type => typeof(sbyte);
            public override VariantKind Kind => VariantKind.Int8;
            public sbyte GetValue(in Variant variant) => variant._structValue.Int8;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(sbyte) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (GetValue(in variant) is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToInt64(in Variant variant, out long int64Value)
            {
                int64Value = GetValue(in variant);
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToInt8(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private sealed class Int16Info : VariantInfo
        {
            public override Type Type => typeof(short);
            public override VariantKind Kind => VariantKind.Int16;
            public short GetValue(in Variant variant) => variant._structValue.Int16;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(short) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (GetValue(in variant) is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToInt64(in Variant variant, out long int64Value)
            {
                int64Value = GetValue(in variant);
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToInt16(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private sealed class Int32Info : VariantInfo
        {
            public override Type Type => typeof(int);
            public override VariantKind Kind => VariantKind.Int32;
            public int GetValue(in Variant variant) => variant._structValue.Int32;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(int) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (GetValue(in variant) is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToInt64(in Variant variant, out long int64Value)
            {
                int64Value = GetValue(in variant);
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToInt32(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private sealed class Int64Info : VariantInfo
        {
            public override Type Type => typeof(long);
            public override VariantKind Kind => VariantKind.Int64;
            public long GetValue(in Variant variant) => variant._structValue.Int64;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(long) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (GetValue(in variant) is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToInt64(in Variant variant, out long int64Value)
            {
                int64Value = GetValue(in variant);
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToInt64(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private sealed class UInt8Info : VariantInfo
        {
            public override Type Type => typeof(byte);
            public override VariantKind Kind => VariantKind.UInt8;
            public byte GetValue(in Variant variant) => variant._structValue.UInt8;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(byte) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (GetValue(in variant) is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToInt64(in Variant variant, out long int64Value)
            {
                int64Value = GetValue(in variant);
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToInt8(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private sealed class UInt16Info : VariantInfo
        {
            public override Type Type => typeof(ushort);
            public override VariantKind Kind => VariantKind.UInt16;
            public ushort GetValue(in Variant variant) => variant._structValue.UInt16;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(ushort) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (GetValue(in variant) is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToInt64(in Variant variant, out long int64Value)
            {
                int64Value = GetValue(in variant);
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToUInt16(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private sealed class UInt32Info : VariantInfo
        {
            public override Type Type => typeof(uint);
            public override VariantKind Kind => VariantKind.UInt32;
            public uint GetValue(in Variant variant) => variant._structValue.UInt32;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(uint) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (GetValue(in variant) is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToInt64(in Variant variant, out long int64Value)
            {
                int64Value = GetValue(in variant);
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToUInt32(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private sealed class UInt64Info : VariantInfo
        {
            public override Type Type => typeof(ulong);
            public override VariantKind Kind => VariantKind.UInt64;
            public ulong GetValue(in Variant variant) => variant._structValue.UInt64;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(ulong) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (GetValue(in variant) is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToInt64(in Variant variant, out long value)
            {
                var val = GetValue(in variant);
                if (val <= long.MaxValue)
                {
                    value = unchecked((long)val);
                    return true;
                }
                value = default;
                return false;
            }

            public override bool TryConvertToDecimal(in Variant variant, out decimal value)
            {
                value = GetValue(in variant);
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToUInt64(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private sealed class Float32Info : VariantInfo
        {
            public override Type Type => typeof(float);
            public override VariantKind Kind => VariantKind.Float32;
            public float GetValue(in Variant variant) => variant._structValue.Float32;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(float) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (GetValue(in variant) is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToInt64(in Variant variant, out long value)
            {
                var val = GetValue(in variant);
                if (val >= long.MinValue && val <= long.MaxValue)
                {
                    value = (long)val;
                    return true;
                }
                value = default;
                return true;
            }

            public override bool TryConvertToFloat64(in Variant variant, out double value)
            {
                value = GetValue(in variant);
                return true;
            }

            public override bool TryConvertToDecimal(in Variant variant, out decimal value)
            {
                var val = GetValue(in variant);
                if (val >= (double)decimal.MinValue && val <= (double)decimal.MaxValue)
                {
                    value = (decimal)val;
                    return true;
                }
                value = default;
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToSingle(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private sealed class Float64Info : VariantInfo
        {
            public override Type Type => typeof(double);
            public override VariantKind Kind => VariantKind.Float64;
            public double GetValue(in Variant variant) => variant._structValue.Float64;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(double) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (GetValue(in variant) is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToInt64(in Variant variant, out long value)
            {
                var val = GetValue(in variant);
                if (val >= long.MinValue && val <= long.MaxValue)
                {
                    value = (long)val;
                    return true;
                }
                value = default;
                return true;
            }

            public override bool TryConvertToFloat64(in Variant variant, out double value)
            {
                value = GetValue(in variant);
                return true;
            }

            public override bool TryConvertToDecimal(in Variant variant, out decimal value)
            {
                var val = GetValue(in variant);
                if (val >= (double)decimal.MinValue && val <= (double)decimal.MaxValue)
                {
                    value = (decimal)val;
                    return true;
                }
                value = default;
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToDouble(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private sealed class Decimal64Info : VariantInfo
        {
            public override Type Type => typeof(Decimal64);
            public override VariantKind Kind => VariantKind.Decimal64;
            public Decimal64 GetValue(in Variant variant) => variant._structValue.Decimal64;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(Decimal64) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (GetValue(in variant) is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToInt64(in Variant variant, out long value)
            {
                var val = GetValue(in variant);
                if (val >= long.MinValue && val <= long.MaxValue)
                {
                    value = (long)val;
                    return true;
                }
                value = default;
                return true;
            }

            public override bool TryConvertToDecimal(in Variant variant, out decimal decValue)
            {
                var val = GetValue(in variant);
                if (val >= decimal.MinValue && val <= decimal.MaxValue)
                {
                    decValue = (decimal)val;
                    return true;
                }
                decValue = default;
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToDecimal64(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private sealed class Decimal128Info : VariantInfo
        {
            public override Type Type => typeof(decimal);
            public override VariantKind Kind => VariantKind.Decimal128;
            public decimal GetValue(in Variant variant) => variant._structValue.Decimal64;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(decimal) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (GetValue(in variant) is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToInt64(in Variant variant, out long value)
            {
                var val = GetValue(in variant);
                if (val >= long.MinValue && val <= long.MaxValue)
                {
                    value = (long)val;
                    return true;
                }
                value = default;
                return true;
            }

            public override bool TryConvertToDecimal(in Variant variant, out decimal value)
            {
                var val = GetValue(in variant);
                if (val >= decimal.MinValue && val <= decimal.MaxValue)
                {
                    value = val;
                    return true;
                }
                value = default;
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToDecimal(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private sealed class Char16Info : VariantInfo
        {
            public override Type Type => typeof(char);
            public override VariantKind Kind => VariantKind.Char16;
            public char GetValue(in Variant variant) => variant._structValue.Char16;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(char) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (GetValue(in variant) is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToInt64(in Variant variant, out long value)
            {
                value = GetValue(in variant);
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToChar(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private sealed class Char32Info : VariantInfo
        {
            public override Type Type => typeof(Rune);
            public override VariantKind Kind => VariantKind.Char32;
            public Rune GetValue(in Variant variant) => variant._structValue.Char32;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(Rune) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (GetValue(in variant) is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToInt64(in Variant variant, out long value)
            {
                value = GetValue(in variant).Value;
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToRune(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private sealed class DateOnlyInfo : VariantInfo
        {
            public override Type Type => typeof(DateOnly);
            public override VariantKind Kind => VariantKind.DateOnly;
            public DateOnly GetValue(in Variant variant) => variant._structValue.DateOnly;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(DateOnly) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (GetValue(in variant) is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToDateOnly(in Variant variant, out DateOnly value)
            {
                value = GetValue(in variant);
                return true;
            }

            public override bool TryConvertToDateTime(in Variant variant, out DateTime value)
            {
                value = GetValue(in variant).ToDateTime(default);
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToDateOnly(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private sealed class TimeOnlyInfo : VariantInfo
        {
            public override Type Type => typeof(TimeOnly);
            public override VariantKind Kind => VariantKind.TimeOnly;
            public TimeOnly GetValue(in Variant variant) => variant._structValue.TimeOnly;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(TimeOnly) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T tvalue)
            {
                if (GetValue(in variant) is T tval)
                {
                    tvalue = tval;
                    return true;
                }
                tvalue = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToTimeOnly(in Variant variant, out TimeOnly value)
            {
                value = GetValue(in variant);
                return true;
            }

            public override bool TryConvertToTimeSpan(in Variant variant, out TimeSpan value)
            {
                value = GetValue(in variant).ToTimeSpan();
                return true;
            }

            public override bool TryConvertToDateTime(in Variant variant, out DateTime value)
            {
                value = default(DateOnly).ToDateTime(GetValue(in variant));
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToTimeOnly(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private sealed class DateTimeInfo : VariantInfo
        {
            public override Type Type => typeof(DateTime);
            public override VariantKind Kind => VariantKind.DateTime;
            public DateTime GetValue(in Variant variant) => variant._structValue.DateTime;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(DateTime) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (GetValue(in variant) is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToDateTime(in Variant variant, out DateTime value)
            {
                value = GetValue(in variant);
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToDateTime(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private sealed class TimeSpanInfo : VariantInfo
        {
            public override Type Type => typeof(TimeSpan);
            public override VariantKind Kind => VariantKind.TimeSpan;
            public TimeSpan GetValue(in Variant variant) => variant._structValue.TimeSpan;

            public override bool IsType<T>(in Variant variant) =>
                typeof(T) == typeof(TimeSpan) || GetValue(in variant) is T;

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (GetValue(in variant) is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                GetValue(in variant);

            public override string ToString(in Variant variant) =>
                GetValue(in variant).ToString();

            public override bool TryConvertToTimeOnly(in Variant variant, out TimeOnly value)
            {
                value = new TimeOnly(GetValue(in variant).Ticks);
                return true;
            }

            public override bool TryConvertToDateTime(in Variant variant, out DateTime value)
            {
                value = new DateTime(GetValue(in variant).Ticks);
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToTimeSpan(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private class EnumInfo<TEnum> : VariantInfo
        {
            public override Type Type => typeof(TEnum);
            public override VariantKind Kind => VariantKind.Enum;
            public long GetValue(in Variant variant) => variant._structValue.Int64;

            public override bool IsType<TType>(in Variant variant) =>
                typeof(TType) == typeof(TEnum)
                || (TryConvertLongToEnum<TEnum>(GetValue(in variant), out var value) && value is TType);

            public override bool TryGet<T>(in Variant variant, out T value)
            {
                if (TryConvertLongToEnum<TEnum>(GetValue(in variant), out var enumValue)
                    && enumValue is T tval)
                {
                    value = tval;
                    return true;
                }
                value = default!;
                return false;
            }

            public override object ToObject(in Variant variant) =>
                TryConvertLongToEnum<TEnum>(GetValue(in variant), out var enumValue)
                    ? enumValue
                    : default!;

            public override string ToString(in Variant variant) =>
                TryConvertLongToEnum<TEnum>(GetValue(in variant), out var enumValue)
                    ? enumValue?.ToString() ?? ""
                    : "";

            public override bool TryConvertToInt64(in Variant variant, out long value)
            {
                value = GetValue(in variant);
                return true;
            }

            public override bool Equals(in Variant variant, in Variant other) =>
                other.TryConvertToInt64(out var otherValue)
                && GetValue(in variant) == otherValue;
        }

        private static ImmutableDictionary<Type, VariantInfo> s_typeToInfoMap =
            ImmutableDictionary<Type, VariantInfo>.Empty;

        public static VariantInfo GetEnumInfo<TEnum>()
        {
            var map = s_typeToInfoMap;
            var enumType = typeof(TEnum);
            if (!map.TryGetValue(enumType, out var info))
            {
                info = ImmutableInterlocked.GetOrAdd(ref s_typeToInfoMap, enumType, new EnumInfo<TEnum>());
            }
            return info;
        }
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
}
#endif