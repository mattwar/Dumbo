using Reference = System.Object;
using Bits = System.UInt64;
using System.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Reflection;

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

namespace Dumbo;

/// <summary>
/// A wrapper struct that can contain any value 
/// that has a nominal footprint and avoids boxing of small structs that do not contain references.
/// </summary>
public readonly struct Variant : ITypeUnion<Variant>, IEquatable<Variant>
{
    private readonly Reference? _reference;
    private readonly OverlappedBits _overlapped;

    private Variant(Reference? reference, OverlappedBits bits)
    {
        _reference = reference;
        _overlapped = bits;
    }

    /// <summary>
    /// The encoding of this variant
    /// </summary>
    private VariantEncoding Encoding =>
        VariantEncoding.GetEncoding(in this);

    /// <summary>
    /// A variant containing the null value.
    /// </summary>
    public static readonly Variant Null = default;

    #region Non-generic API
    /// <summary>
    /// True if the value held in this variant is a boxed value type.
    /// </summary>
    public bool IsBoxed =>
        Encoding is ReferenceEncoding
        && Type.IsValueType;

    /// <summary>
    /// True if the value is null.
    /// </summary>
    public bool IsNull =>
        Encoding.IsNull(in this);


    /// <summary>
    /// The type of the value held within this variant.
    /// </summary>
    public Type Type =>
        Encoding.GetType(in this);

    /// <summary>
    /// The kind of value contained.
    /// </summary>
    public VariantKind Kind =>
        Encoding.GetKind(in this);

    #region encodings
    private static BoolEncoding _boolEncoding = BoolEncoding.Instance;
    private static ByteEncoding _byteEncoding = ByteEncoding.Instance;
    private static SByteEncoding _sbyteEncoding = SByteEncoding.Instance;
    private static Int16Encoding _int16Encoding = Int16Encoding.Instance;
    private static UInt16Encoding _uint16Encoding = UInt16Encoding.Instance;
    private static Int32Encoding _int32Encoding = Int32Encoding.Instance;
    private static UInt32Encoding _uint32Encoding = UInt32Encoding.Instance;
    private static Int64Encoding _int64Encoding = Int64Encoding.Instance;
    private static UInt64Encoding _uint64Encoding = UInt64Encoding.Instance;
    private static SingleEncoding _singleEncoding = SingleEncoding.Instance;
    private static DoubleEncoding _doubleEncoding = DoubleEncoding.Instance;
    private static DecimalEncoding _decimalEncoding = DecimalEncoding.Instance;
    private static Decimal64Encoding _decimal64Encoding = Decimal64Encoding.Instance;
    private static CharEncoding _charEncoding = CharEncoding.Instance;
    private static RuneEncoding _runeEncoding = RuneEncoding.Instance;
    private static DateTimeEncoding _dateTimeEncoding = DateTimeEncoding.Instance;
    private static DateOnlyEncoding _dateOnlyEncoding = DateOnlyEncoding.Instance;
    private static TimeSpanEncoding _timeSpanEncoding = TimeSpanEncoding.Instance;
    private static TimeOnlyEncoding _timeOnlyEncoding = TimeOnlyEncoding.Instance;
    #endregion

    #region Create
    public static Variant Create(bool value) =>
        new Variant(_boolEncoding, new OverlappedBits { _boolVal = value });

    public static Variant Create(bool? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(byte value) =>
        new Variant(_byteEncoding, new OverlappedBits { _byteVal = value });

    public static Variant Create(byte? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(sbyte value) =>
        new Variant(_sbyteEncoding, new OverlappedBits { _sbyteVal = value });

    public static Variant Create(sbyte? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(short value) =>
        new Variant(_int16Encoding, new OverlappedBits { _int16Val = value });

    public static Variant Create(short? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(ushort value) =>
        new Variant(_uint16Encoding, new OverlappedBits { _uint16Val = value });

    public static Variant Create(ushort? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(int value) =>
        new Variant(_int32Encoding, new OverlappedBits { _int32Val = value });

    public static Variant Create(int? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(uint value) =>
        new Variant(_uint32Encoding, new OverlappedBits { _uint32Val = value });

    public static Variant Create(uint? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(long value) =>
        new Variant(_int64Encoding, new OverlappedBits { _int64Val = value });

    public static Variant Create(long? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(ulong value) =>
        new Variant(_uint64Encoding, new OverlappedBits { _uint64Val = value });

    public static Variant Create(ulong? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(float value) =>
        new Variant(_singleEncoding, new OverlappedBits { _singleVal = value });

    public static Variant Create(float? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(double value) =>
        new Variant(_doubleEncoding, new OverlappedBits { _doubleVal = value });

    public static Variant Create(double? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(Decimal64 value) =>
        new Variant(_decimal64Encoding, new OverlappedBits { _decimal64Val = value });

    public static Variant Create(Decimal64? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(decimal value) =>
        Decimal64.TryCreate(value, out var dec64)
            ? new Variant(_decimalEncoding, new OverlappedBits { _decimal64Val = dec64 })
            : new Variant(value, default);

    public static Variant Create(decimal? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(char value) =>
        new Variant(_charEncoding, new OverlappedBits { _charVal = value });

    public static Variant Create(char? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(Rune value) =>
        new Variant(_runeEncoding, new OverlappedBits { _runeVal = value });

    public static Variant Create(Rune? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(string? value) =>
        (value == null) ? Null : new Variant(value, default);

    public static Variant Create(Guid value) =>
        new Variant(value, default);

    public static Variant Create(Guid? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(DateTime value) =>
        new Variant(_dateTimeEncoding, new OverlappedBits { _dateTimeVal = value });

    public static Variant Create(DateTime? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(DateOnly value) =>
        new Variant(_dateOnlyEncoding, new OverlappedBits { _dateOnlyVal = value });

    public static Variant Create(DateOnly? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(TimeSpan value) =>
        new Variant(_timeSpanEncoding, new OverlappedBits { _timeSpanVal = value });

    public static Variant Create(TimeSpan? value) =>
        (value == null) ? Null : Create(value.Value);

    public static Variant Create(TimeOnly value) =>
        new Variant(_timeOnlyEncoding, new OverlappedBits { _timeOnlyVal = value });

    public static Variant Create(TimeOnly? value) =>
        (value == null) ? Null : Create(value.Value);
    #endregion

    #region Is tests
    public bool IsBoolean =>
        Encoding.GetType() == typeof(bool);

    public bool IsByte =>
        Encoding.GetType() == typeof(byte);

    public bool IsSByte =>
        Encoding.GetType() == typeof(sbyte);

    public bool IsInt16 =>
        Encoding.GetType() == typeof(short);

    public bool IsUInt16 =>
        Encoding.GetType() == typeof(ushort);

    public bool IsInt32 =>
        Encoding.GetType() == typeof(int);

    public bool IsUInt32 =>
        Encoding.GetType() == typeof(uint);

    public bool IsInt64 =>
        Encoding.GetType() == typeof(long);

    public bool IsUInt64 =>
        Encoding.GetType() == typeof(ulong);

    public bool IsSingle =>
        Encoding.GetType() == typeof(float);

    public bool IsDouble =>
        Encoding.GetType() == typeof(double);

    public bool IsDecimal =>
        Encoding.GetType() == typeof(decimal);

    public bool IsDecimal64 =>
        Encoding.GetType() == typeof(Decimal64);

    public bool IsChar =>
        Encoding.GetType() == typeof(char);

    public bool IsRune =>
        Encoding.GetType() == typeof(Rune);

    public bool IsString =>
        _reference is string;

    public bool IsGuid =>
        _reference is Guid;

    public bool IsDateTime =>
        Encoding.GetType() == typeof(DateTime);

    public bool IsDateOnly =>
        Encoding.GetType() == typeof(DateOnly);

    public bool IsTimeSpan =>
        Encoding.GetType() == typeof(TimeSpan);

    public bool IsTimeOnly =>
        Encoding.GetType() == typeof(TimeOnly);

    public bool IsOther =>
        _reference != null;
    #endregion

    #region TryGet
    public bool TryGet(out bool value)
    {
        if (_reference == _boolEncoding) { value = _overlapped._boolVal; return true; }
        if (_reference is VariantEncoding<bool> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet(out byte value)
    {
        if (_reference == _byteEncoding) { value = _overlapped._byteVal; return true; }
        if (_reference is VariantEncoding<byte> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet(out sbyte value)
    {
        if (_reference == _sbyteEncoding) { value = _overlapped._sbyteVal; return true; }
        if (_reference is VariantEncoding<sbyte> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet(out short value)
    {
        if (_reference == _int16Encoding) { value = _overlapped._int16Val; return true; }
        if (_reference is VariantEncoding<short> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet(out ushort value)
    {
        if (_reference == _uint16Encoding) { value = _overlapped._uint16Val; return true; }
        if (_reference is VariantEncoding<ushort> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet(out int value)
    {
        if (_reference == _int32Encoding) { value = _overlapped._int32Val; return true; }
        if (_reference is VariantEncoding<int> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet(out uint value)
    {
        if (_reference == _uint32Encoding) { value = _overlapped._uint32Val; return true; }
        if (_reference is VariantEncoding<uint> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet(out long value)
    {
        if (_reference == _int32Encoding) { value = _overlapped._int32Val; return true; }
        if (_reference is VariantEncoding<long> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet(out ulong value)
    {
        if (_reference == _uint64Encoding) { value = _overlapped._uint64Val; return true; }
        if (_reference is VariantEncoding<ulong> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet(out float value)
    {
        if (_reference == _singleEncoding) { value = _overlapped._singleVal; return true; }
        if (_reference is VariantEncoding<float> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet(out double value)
    {
        if (_reference == _doubleEncoding) { value = _overlapped._doubleVal; return true; }
        if (_reference is VariantEncoding<double> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet(out Decimal64 value)
    {
        if (_reference == _decimal64Encoding) { value = _overlapped._decimal64Val; return true; }
        if (_reference is VariantEncoding<Decimal64> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet(out decimal value)
    {
        if (_reference == _decimalEncoding) { value = _overlapped._decimal64Val.ToDecimal(); return true; }
        if (_reference is VariantEncoding<decimal> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet(out char value)
    {
        if (_reference == _charEncoding) { value = _overlapped._charVal; return true; }
        if (_reference is VariantEncoding<char> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet(out Rune value)
    {
        if (_reference == _runeEncoding) { value = _overlapped._runeVal; return true; }
        if (_reference is VariantEncoding<Rune> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet([NotNullWhen(true)] out string? value)
    {
        if (_reference is string sval) { value = sval; return true; }
        value = default!;
        return false;
    }

    public bool TryGet(out Guid value)
    {
        if (_reference is Guid guid) { value = guid; return true; }
        value = default;
        return false;
    }

    public bool TryGet(out DateTime value)
    {
        if (_reference == _dateTimeEncoding) { value = _overlapped._dateTimeVal; return true; }
        if (_reference is VariantEncoding<DateTime> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet(out DateOnly value)
    {
        if (_reference == _dateOnlyEncoding) { value = _overlapped._dateOnlyVal; return true; }
        if (_reference is VariantEncoding<DateOnly> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet(out TimeSpan value)
    {
        if (_reference == _timeSpanEncoding) { value = _overlapped._timeSpanVal; return true; }
        if (_reference is VariantEncoding<TimeSpan> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet(out TimeOnly value)
    {
        if (_reference == _timeOnlyEncoding) { value = _overlapped._timeOnlyVal; return true; }
        if (_reference is VariantEncoding<TimeOnly> encoding) { value = encoding.Decode(in this); return true; }
        value = default;
        return false;
    }

    public bool TryGet([NotNullWhen(true)] out object? value)
    {
        if (_reference is VariantEncoding encoding) { value = encoding.GetBoxed(this); return true; }
        if (_reference != null && _reference is not VariantEncoding) { value = _reference; return true; }
        value = default!;
        return false;
    }
    #endregion

    #region Get
    public bool GetBoolean() =>
        TryGet(out bool value) ? value : throw new InvalidCastException();

    public byte GetByte() =>
        TryGet(out byte value) ? value : throw new InvalidCastException();

    public sbyte GetSByte() =>
        TryGet(out sbyte value) ? value : throw new InvalidCastException();

    public short GetInt16() =>
        TryGet(out short value) ? value : throw new InvalidCastException();

    public ushort GetUInt16() =>
        TryGet(out ushort value) ? value : throw new InvalidCastException();

    public int GetInt32() =>
        TryGet(out int value) ? value : throw new InvalidCastException();

    public uint GetUInt32() =>
        TryGet(out uint value) ? value : throw new InvalidCastException();

    public long GetInt64() =>
        TryGet(out long value) ? value : throw new InvalidCastException();

    public ulong GetUInt64() =>
        TryGet(out ulong value) ? value : throw new InvalidCastException();

    public float GetSingle() =>
        TryGet(out float value) ? value : throw new InvalidCastException();

    public double GetDouble() =>
        TryGet(out double value) ? value : throw new InvalidCastException();

    public Decimal64 GetDecimal64() =>
        TryGet(out Decimal64 value) ? value : throw new InvalidCastException();

    public decimal GetDecimal() =>
        TryGet(out decimal value) ? value : throw new InvalidCastException();

    public char GetChar() =>
        TryGet(out char value) ? value : throw new InvalidCastException();

    public Rune GetRune() =>
        TryGet(out Rune value) ? value : throw new InvalidCastException();

    public string GetString() =>
        TryGet(out string? value) ? value : throw new InvalidCastException();

    public Guid GetGuid() =>
        TryGet(out Guid value) ? value : throw new InvalidCastException();

    public DateTime GetDateTime() =>
        TryGet(out DateTime value) ? value : throw new InvalidCastException();

    public DateOnly GetDateOnly() =>
        TryGet(out DateOnly value) ? value : throw new InvalidCastException();

    public TimeSpan GetTimeSpan() =>
        TryGet(out TimeSpan value) ? value : throw new InvalidCastException();

    public TimeOnly GetTimeOnly() =>
        TryGet(out TimeOnly value) ? value : throw new InvalidCastException();

    public object GetOther() =>
        TryGet(out object? value) ? value : throw new InvalidCastException();
    #endregion

    #endregion

    #region generic API
    /// <summary>
    /// Create a <see cref="Variant"/> from a value.
    /// </summary>
    public static unsafe Variant Create<TValue>(TValue value)
    {
        if (value == null)
            return Null;

        return VariantEncoder<TValue>.Instance.Encode(value);
    }

    /// <summary>
    /// Create a <see cref="Variant"/> from a value.
    /// </summary>
    public static bool TryCreate<TValue>(TValue value, [NotNullWhen(true)] out Variant variant)
    {
        variant = Create(value);
        return true;
    }

    /// <summary>
    /// True if the value is of the specified type.
    /// </summary>
    public bool IsType<T>() =>
        Encoding.IsType<T>(in this);

    /// <summary>
    /// Returns the value as the specified type if the value is of the specified type or the default value of the type if not.
    /// </summary>
    public T AsType<T>() =>
        TryGet<T>(out var value) 
            ? value! 
            : default!;

    /// <summary>
    /// Returns <see langword="true"/> and the value as the specified type if the value is of the specified type, otherwise returns <see langword="false"/>.
    /// </summary>
    public bool TryGet<T>([NotNullWhen(true)] out T value) =>
        Encoding.TryGet(in this, out value);

    /// <summary>
    /// Returns the value as the specified type if the value is of the specified type, otherwise throws <see cref="InvalidCastException"/>.
    /// </summary>
    public T Get<T>() =>
        TryGet<T>(out var value) 
            ? value 
            : throw new InvalidCastException();

    /// <summary>
    /// Returns the value converted to a string.
    /// </summary>
    public override string ToString() =>
        Encoding.GetString(in this);

    /// <summary>
    /// Returns true if the value held by the variant is equal to the specified value.
    /// </summary>
    public bool Equals<T>(T value) =>
        Encoding.IsEqual(in this, value);

    /// <summary>
    /// Returns true if the value held by the variant is equal to the value held by the specified variant.
    /// </summary>
    public bool Equals(Variant variant) =>
        Encoding.IsEqual(in this, in variant);

    /// <summary>
    /// Returns true if the value held by the variant is equal to the specified value.
    /// </summary>
    public override bool Equals([NotNullWhen(true)] object? value) =>
        Encoding.IsEqual(in this, value);

    /// <summary>
    /// Returns the hash code of the value held by the variant.
    /// </summary>
    public override int GetHashCode() =>
        Encoding.GetHashCode(in this);

    #endregion

    #region Operators
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

    public static explicit operator bool(Variant value) => value.GetBoolean();
    public static implicit operator sbyte(Variant value) => value.GetSByte();
    public static implicit operator short(Variant value) => value.GetInt16();
    public static implicit operator int(Variant value) => value.GetInt32();
    public static implicit operator long(Variant value) => value.GetInt64();
    public static implicit operator byte(Variant value) => value.GetByte();
    public static implicit operator ushort(Variant value) => value.GetUInt16();
    public static implicit operator uint(Variant value) => value.GetUInt32();
    public static implicit operator ulong(Variant value) => value.GetUInt64();
    public static implicit operator float(Variant value) => value.GetSingle();
    public static implicit operator double(Variant value) => value.GetDouble();
    public static implicit operator Decimal64(Variant value) => value.GetDecimal64();
    public static implicit operator decimal(Variant value) => value.GetDecimal();
    public static implicit operator char(Variant value) => value.GetChar();
    public static implicit operator Rune(Variant value) => value.GetRune();
    public static implicit operator String(Variant value) => value.GetString();
    public static implicit operator DateOnly(Variant value) => value.GetDateOnly();
    public static implicit operator TimeOnly(Variant value) => value.GetTimeOnly();
    public static implicit operator DateTime(Variant value) => value.GetDateTime();
    public static implicit operator TimeSpan(Variant value) => value.GetTimeSpan();
    public static implicit operator Guid(Variant value) => value.GetGuid();

    public static explicit operator bool?(Variant value) => value.IsNull ? default : (bool)value;
    public static implicit operator sbyte?(Variant value) => value.IsNull ? default : (sbyte)value;
    public static implicit operator short?(Variant value) => value.IsNull ? default : (short)value;
    public static implicit operator int?(Variant value) => value.IsNull ? default : (int)value;
    public static implicit operator long?(Variant value) => value.IsNull ? default : (long)value;
    public static implicit operator byte?(Variant value) => value.IsNull ? default : (byte)value;
    public static implicit operator ushort?(Variant value) => value.IsNull ? default : (ushort)value;
    public static implicit operator uint?(Variant value) => value.IsNull ? default : (uint)value;
    public static implicit operator ulong?(Variant value) => value.IsNull ? default : (ulong)value;
    public static implicit operator float?(Variant value) => value.IsNull ? default : (float)value;
    public static implicit operator double?(Variant value) => value.IsNull ? default : (double)value;
    public static implicit operator Decimal64?(Variant value) => value.IsNull ? default : (Decimal64)value;
    public static implicit operator decimal?(Variant value) => value.IsNull ? default : (decimal)value;
    public static implicit operator char?(Variant value) => value.IsNull ? default : (char)value;
    public static implicit operator Rune?(Variant value) => value.IsNull ? default : (Rune)value;
    public static implicit operator DateOnly?(Variant value) => value.IsNull ? default : (DateOnly)value;
    public static implicit operator TimeOnly?(Variant value) => value.IsNull ? default : (TimeOnly)value;
    public static implicit operator DateTime?(Variant value) => value.IsNull ? default : (DateTime)value;
    public static implicit operator TimeSpan?(Variant value) => value.IsNull ? default : (TimeSpan)value;
    public static implicit operator Guid?(Variant value) => value.IsNull ? default : (Guid)value;
    #endregion

    #region Encoders
    private abstract class VariantEncoder<TValue>
    {
        private static VariantEncoder<TValue>? _instance;

        public static VariantEncoder<TValue> Instance
        {
            get
            {
                if (_instance == null)
                {
                    var encoder = CreateEncoder();
                    Interlocked.CompareExchange(ref _instance, encoder, null);
                }

                return _instance;
            }
        }

        private static unsafe VariantEncoder<TValue> CreateEncoder()
        {
            var ttype = typeof(TValue);
            if (ttype.IsValueType)
            {
                if (default(TValue) == null)
                {
                    // value is some Nullable<T>
                    var elementType = ttype.GetGenericArguments()[0];
                    var encoderType = typeof(NullableEncoder<>).MakeGenericType(elementType);
                    return (VariantEncoder<TValue>)Activator.CreateInstance(encoderType)!;
                }
                else if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
                {
                    // references, but only room for one, must be wrapper struct
                    if (sizeof(TValue) == sizeof(Reference))
                    {
                        return new WrapperStructEncoder<TValue>();
                    }
                }
                else if (ttype == typeof(decimal))
                {
                    // use decimal encoder to conditionaly fit most common decimals in bits
                    return (VariantEncoder<TValue>)(object)new DecimalEncoder();
                }
                else if (sizeof(TValue) <= sizeof(Bits))
                {
                    // no references and small enough to fit in bits
                    return new BitsEncoder<TValue>();
                }
                else if (ttype == typeof(Variant))
                {
                    // use variant encoder to pass the value right back
                    return (VariantEncoder<TValue>)(object)new VariantVariantEncoder();
                }
                
                if (ttype.IsAssignableTo(typeof(ITypeUnion)))
                {
                    // struct that is a type union, use ITypeUnion interface to access value as a variant.
                    var encoderType = typeof(TypeUnionEncoder<>).MakeGenericType(ttype);
                    return (VariantEncoder<TValue>)Activator.CreateInstance(encoderType)!;
                }
            }

            // Nothing left but being a reference (boxed if a value type)
            return new ReferenceEncoder<TValue>();
        }

        /// <summary>
        /// Encodes a value into a <see cref="Variant"/>
        /// </summary>
        public abstract Variant Encode(TValue value);
    }

    /// <summary>
    /// An <see cref="VariantEncoder"/> for values that can be stored in the bits field.
    /// </summary>
    private sealed class BitsEncoder<TValue> : VariantEncoder<TValue>
    {
        private readonly VariantEncoding _encoding = 
            BitsEncoding<TValue>.Instance;

        public unsafe BitsEncoder()
        {
            Debug.Assert(typeof(TValue).IsValueType);
            Debug.Assert(default(TValue) != null);
            Debug.Assert(!RuntimeHelpers.IsReferenceOrContainsReferences<TValue>());
            Debug.Assert(sizeof(TValue) <= sizeof(Bits));
        }

        public override Variant Encode(TValue value)
        {
            var bits = Unsafe.As<TValue, Bits>(ref value);
            return new Variant(_encoding, new OverlappedBits { _bitsVal = bits });
        }
    }

    /// <summary>
    /// An <see cref="VariantEncoder{TValue}"/> for types that are references or must be boxed.
    /// </summary>
    private sealed class ReferenceEncoder<TValue> : VariantEncoder<TValue>
    {
        public override Variant Encode(TValue value) =>
            new Variant(value, default);
    }

    /// <summary>
    /// An <see cref="VariantEncoder{TValue}"/> for types that are struct wrappers around
    /// a single reference.
    /// </summary>
    private sealed class WrapperStructEncoder<TValue> : VariantEncoder<TValue>
    {
        private readonly int _encodingId =
            WrapperStructEncoding<TValue>.Instance.GetId();

        public unsafe WrapperStructEncoder()
        {
            Debug.Assert(typeof(TValue).IsValueType);
            Debug.Assert(default(TValue) != null);
            Debug.Assert(RuntimeHelpers.IsReferenceOrContainsReferences<TValue>());
            Debug.Assert(sizeof(TValue) == sizeof(Reference));
        }

        public override Variant Encode(TValue value)
        {
            // interior pointer from value is stored in the reference field.
            // ID of encoding is stored in the bits field.
            var refValue = Unsafe.As<TValue, Reference>(ref value);
            return new Variant(refValue, new OverlappedBits { _int32Val = _encodingId });
        }
    }

    /// <summary>
    /// An <see cref="VariantEncoder{TValue}"/> for Nullable&lt;T&gt; values.
    /// </summary>
    private sealed class NullableEncoder<TElement> : VariantEncoder<Nullable<TElement>>
        where TElement : struct
    {
        private readonly VariantEncoder<TElement> _encoder =
            VariantEncoder<TElement>.Instance;

        public override Variant Encode(TElement? value) =>
            _encoder.Encode(value.GetValueOrDefault()); // null is already handled
    }

    /// <summary>
    /// A <see cref="VariantEncoder{TValue}"/> for decimal values
    /// </summary>
    private sealed class DecimalEncoder : VariantEncoder<decimal>
    {
        public override Variant Encode(decimal value)
        {
            if (Decimal64.TryCreate(value, out var decVal))
            {
                return new Variant(DecimalEncoding.Instance, new OverlappedBits { _decimal64Val = decVal });
            }
            else
            {
                // box it!
                return new Variant(value, default);
            }
        }
    }

    /// <summary>
    /// An encoder for variant values that just returns the value.
    /// </summary>
    private sealed class VariantVariantEncoder : VariantEncoder<Variant>
    {
        public override Variant Encode(Variant variant) =>
            variant;
    }

    /// <summary>
    /// An encoder for type unions that access the unions value as a variant.
    /// </summary>
    private sealed class TypeUnionEncoder<TUnion> : VariantEncoder<TUnion>
        where TUnion : ITypeUnion
    {
        public TypeUnionEncoder()
        {
            Debug.Assert(typeof(TUnion).IsAssignableTo(typeof(ITypeUnion)));
        }

        public override Variant Encode(TUnion value) =>
            value.ToVariant();
    }

    #endregion

    #region Encodings
    /// <summary>
    /// A type that faciliates access to the value encoded inside a <see cref="Variant"/>
    /// </summary>
    private abstract class VariantEncoding
    {
        public abstract Type GetType(in Variant variant);
        public abstract VariantKind GetKind(in Variant variant);
        public virtual bool IsNull(in Variant variant) => false;
        public abstract bool IsType<T>(in Variant variant);
        public abstract bool TryGet<T>(in Variant variant, out T value);
        public abstract string GetString(in Variant variant);
        public abstract bool IsEqual<T>(in Variant variant, T value);
        public abstract bool IsEqual(in Variant variant, in Variant other);
        public abstract int GetHashCode(in Variant variant);
        public abstract object GetBoxed(in Variant variant);

        /// <summary>
        /// Return the <see cref="VariantEncoding"/> used by the <see cref="Variant"/>,
        /// or null if the variant holds a value as a reference.
        /// </summary>
        public static VariantEncoding GetEncoding(in Variant variant)
        {
            if (variant._reference is VariantEncoding enc)
            {
                return enc;
            }
            else if (variant._overlapped._bitsVal > 0)
            {
                // _bits has the encoding's id
                var encodingId = (int)variant._overlapped._bitsVal;
                return s_encodingList[encodingId];
            }
            else
            {
                return ReferenceEncoding.Instance;
            }
        }

        protected static List<VariantEncoding> s_encodingList =
            new List<VariantEncoding>(capacity: 1024) { null! };
    }

    private static VariantKind Unknown = (VariantKind)(-1);

    private static VariantKind GetKind(Type type)
    {
        switch (TypeInfo.GetTypeCode(type))
        {
            case TypeCode.Boolean:
                return VariantKind.Boolean;
            case TypeCode.Byte:
                return VariantKind.Byte;
            case TypeCode.SByte:
                return VariantKind.SByte;
            case TypeCode.Int16:
                return VariantKind.Int16;
            case TypeCode.UInt16:
                return VariantKind.UInt16;
            case TypeCode.Int32:
                return VariantKind.Int32;
            case TypeCode.UInt32:
                return VariantKind.UInt32;
            case TypeCode.Int64:
                return VariantKind.Int64;
            case TypeCode.UInt64:
                return VariantKind.UInt64;
            case TypeCode.Single:
                return VariantKind.Single;
            case TypeCode.Double:
                return VariantKind.Double;
            case TypeCode.DateTime:
                return VariantKind.DateTime;
            case TypeCode.String:
                return VariantKind.String;
            case TypeCode.Char:
                return VariantKind.Char;
            default:
                if (type == typeof(Guid))
                    return VariantKind.Guid;
                else if (type == typeof(DateOnly))
                    return VariantKind.DateOnly;
                else if (type == typeof(TimeSpan))
                    return VariantKind.TimeSpan;
                else if (type == typeof(TimeOnly))
                    return VariantKind.TimeOnly;
                else if (type == typeof(Rune))
                    return VariantKind.Rune;
                return VariantKind.Other;
        }
    }

    /// <summary>
    /// Abstract base for a strongly typed variant encoding.
    /// </summary>
    private abstract class VariantEncoding<TValue> : VariantEncoding
    {
        public abstract TValue Decode(in Variant variant);

        public override object GetBoxed(in Variant variant) =>
            Decode(in variant)!;

        public override Type GetType(in Variant variant) => 
            typeof(TValue);


        private VariantKind _kind = Unknown;

        public override VariantKind GetKind(in Variant variant)
        {
            if (_kind == Unknown)
            {
                _kind = Variant.GetKind(GetType(in variant));
            }

            return _kind;
        }

        public override bool IsType<TOther>(in Variant variant) =>
            TryGet<TOther>(in variant, out _);

        public override bool TryGet<TOther>(in Variant variant, out TOther value)
        {
            var decoded = Decode(in variant);
            if (decoded is TOther other)
            {
                value = other;
                return true;
            }
            else if (TypeUnionFactory<TOther>.TryGetFactory(out var factory))
            {
                return factory.TryCreate(decoded, out value);
            }

            value = default!;
            return false;
        }

        public override string GetString(in Variant variant) =>
            Decode(in variant)?.ToString() ?? "";

        public override bool IsEqual<T>(in Variant variant, T value)
        {
            if (Decode(in variant) is T tvalue)
            {
                return EqualityComparer<T>.Default.Equals(tvalue, value);
            }
            else if (value is Variant v)
            {
                return IsEqual(in variant, in v);
            }

            return false;
        }

        public override bool IsEqual(in Variant variant, in Variant other) =>
            other.Equals(Decode(in variant));

        public override int GetHashCode(in Variant variant) =>
            Decode(in variant)?.GetHashCode() ?? 0;
    }

    /// <summary>
    /// Encoding for type that are encoded in the bits field.
    /// </summary>
    private sealed class BitsEncoding<TValue> : VariantEncoding<TValue>
    {
        public static BitsEncoding<TValue> Instance =
            new BitsEncoding<TValue>();

        private BitsEncoding() { }

        public override TValue Decode(in Variant variant)
        {
            Bits bits = variant._overlapped._bitsVal;
            return Unsafe.As<Bits, TValue>(ref bits);
        }
    }

    /// <summary>
    /// Encoding for a reference or boxed value.
    /// </summary>
    private sealed class ReferenceEncoding : VariantEncoding
    {
        internal static ReferenceEncoding Instance =
            new ReferenceEncoding();

        private ReferenceEncoding() { }

        public override object GetBoxed(in Variant variant) =>
            variant._reference!;

        public override Type GetType(in Variant variant) =>
            variant._reference?.GetType() ?? typeof(object);

        private VariantKind _kind = Unknown;

        public override VariantKind GetKind(in Variant variant)
        {
            if (_kind == Unknown)
            {
                _kind = Variant.GetKind(GetType(in variant));
            }

            return _kind;
        }

        public override bool IsNull(in Variant variant) =>
            variant._reference is null;

        public override bool IsType<T>(in Variant variant) =>
            variant._reference is T;

        public override bool TryGet<T>(in Variant variant, out T value)
        {
            if (variant._reference is T other)
            {
                value = other;
                return true;
            }
            else if (TypeUnionFactory<T>.TryGetFactory(out var factory))
            {
                return factory.TryCreate(variant._reference, out value);

            }
            value = default!;
            return false;
        }

        public override string GetString(in Variant variant) =>
            variant._reference?.ToString() ?? "";

        public override bool IsEqual<T>(in Variant variant, T value)
        {
            if (value is Variant v)
            {
                return IsEqual(in variant, in v);
            }
            else
            {
                return object.Equals(variant._reference, value);
            }
        }

        public override bool IsEqual(in Variant variant, in Variant other) =>
            other.Equals(variant._reference);

        public override int GetHashCode(in Variant variant) =>
            variant._reference?.GetHashCode() ?? 0;
    }

    /// <summary>
    /// Encoding for a struct wrapper around a single reference.
    /// </summary>
    private sealed class WrapperStructEncoding<TValue> : VariantEncoding<TValue>
    {
        public static WrapperStructEncoding<TValue> Instance =
            new WrapperStructEncoding<TValue>();

        private WrapperStructEncoding() { }

        private int _id = 0;

        public int GetId()
        {
            if (_id == 0)
            {
                lock (s_encodingList)
                {
                    _id = s_encodingList.Count;
                    s_encodingList.Add(this);
                }
            }

            return _id;
        }

        public override TValue Decode(in Variant variant)
        {
            Reference? refValue = variant._reference;
            return Unsafe.As<Reference, TValue>(ref refValue!);
        }
    }

    private sealed class BoolEncoding : VariantEncoding<bool>
    {
        public static readonly BoolEncoding Instance =
            new BoolEncoding();

        private BoolEncoding() { }

        public override bool Decode(in Variant variant)
        {
            return variant._overlapped._boolVal;
        }
    }

    private sealed class ByteEncoding : VariantEncoding<byte>
    {
        public static readonly ByteEncoding Instance =
            new ByteEncoding();

        private ByteEncoding() { }

        public override byte Decode(in Variant variant)
        {
            return variant._overlapped._byteVal;
        }
    }

    private sealed class SByteEncoding : VariantEncoding<sbyte>
    {
        public static readonly SByteEncoding Instance =
            new SByteEncoding();

        private SByteEncoding() { }

        public override sbyte Decode(in Variant variant)
        {
            return variant._overlapped._sbyteVal;
        }
    }

    private sealed class Int16Encoding : VariantEncoding<Int16>
    {
        public static readonly Int16Encoding Instance =
            new Int16Encoding();

        private Int16Encoding() { }

        public override Int16 Decode(in Variant variant)
        {
            return variant._overlapped._int16Val;
        }
    }

    private sealed class UInt16Encoding : VariantEncoding<UInt16>
    {
        public static readonly UInt16Encoding Instance =
            new UInt16Encoding();

        private UInt16Encoding() { }

        public override UInt16 Decode(in Variant variant)
        {
            return variant._overlapped._uint16Val;
        }
    }

    private sealed class Int32Encoding : VariantEncoding<Int32>
    {
        public static readonly Int32Encoding Instance =
            new Int32Encoding();

        private Int32Encoding() { }

        public override Int32 Decode(in Variant variant)
        {
            return variant._overlapped._int32Val;
        }
    }

    private sealed class UInt32Encoding : VariantEncoding<UInt32>
    {
        public static readonly UInt32Encoding Instance =
            new UInt32Encoding();

        private UInt32Encoding() { }

        public override UInt32 Decode(in Variant variant)
        {
            return variant._overlapped._uint32Val;
        }
    }

    private sealed class Int64Encoding : VariantEncoding<Int64>
    {
        public static readonly Int64Encoding Instance =
            new Int64Encoding();

        private Int64Encoding() { }

        public override Int64 Decode(in Variant variant)
        {
            return variant._overlapped._int64Val;
        }
    }

    private sealed class UInt64Encoding : VariantEncoding<UInt64>
    {
        public static readonly UInt64Encoding Instance =
            new UInt64Encoding();

        private UInt64Encoding() { }

        public override UInt64 Decode(in Variant variant)
        {
            return variant._overlapped._uint64Val;
        }
    }

    private sealed class SingleEncoding : VariantEncoding<Single>
    {
        public static readonly SingleEncoding Instance =
            new SingleEncoding();

        private SingleEncoding() { }

        public override Single Decode(in Variant variant)
        {
            return variant._overlapped._singleVal;
        }
    }

    private sealed class DoubleEncoding : VariantEncoding<Double>
    {
        public static readonly DoubleEncoding Instance =
            new DoubleEncoding();

        private DoubleEncoding() { }

        public override Double Decode(in Variant variant)
        {
            return variant._overlapped._doubleVal;
        }
    }

    private sealed class CharEncoding : VariantEncoding<Char>
    {
        public static readonly CharEncoding Instance =
            new CharEncoding();

        private CharEncoding() { }

        public override Char Decode(in Variant variant)
        {
            return variant._overlapped._charVal;
        }
    }

    private sealed class RuneEncoding : VariantEncoding<Rune>
    {
        public static readonly RuneEncoding Instance =
            new RuneEncoding();

        private RuneEncoding() { }

        public override Rune Decode(in Variant variant)
        {
            return variant._overlapped._runeVal;
        }
    }

    /// <summary>
    /// An encoding for a decimal values stored as a Decimal64 value.
    /// </summary>
    private sealed class Decimal64Encoding : VariantEncoding<Decimal64>
    {
        public static readonly Decimal64Encoding Instance =
            new Decimal64Encoding();

        private Decimal64Encoding() { }

        public override Decimal64 Decode(in Variant variant)
        {
            return variant._overlapped._decimal64Val;
        }
    }

    /// <summary>
    /// An encoding for a decimal values stored as a Decimal64 value.
    /// </summary>
    private sealed class DecimalEncoding : VariantEncoding<decimal>
    {
        public static readonly DecimalEncoding Instance = 
            new DecimalEncoding();

        private DecimalEncoding() { }

        public override decimal Decode(in Variant variant)
        {
            return variant._overlapped._decimal64Val.ToDecimal();
        }
    }

    /// <summary>
    /// An encoding for DateTime values.
    /// </summary>
    private sealed class DateTimeEncoding : VariantEncoding<DateTime>
    {
        public static readonly DateTimeEncoding Instance =
            new DateTimeEncoding();

        private DateTimeEncoding() { }

        public override DateTime Decode(in Variant variant)
        {
            return variant._overlapped._dateTimeVal;
        }
    }

    /// <summary>
    /// An encoding for DateOnly values.
    /// </summary>
    private sealed class DateOnlyEncoding : VariantEncoding<DateOnly>
    {
        public static readonly DateOnlyEncoding Instance =
            new DateOnlyEncoding();

        private DateOnlyEncoding() { }

        public override DateOnly Decode(in Variant variant)
        {
            return variant._overlapped._dateOnlyVal;
        }
    }

    /// <summary>
    /// An encoding for TimeSpan values.
    /// </summary>
    private sealed class TimeSpanEncoding : VariantEncoding<TimeSpan>
    {
        public static readonly TimeSpanEncoding Instance =
            new TimeSpanEncoding();

        private TimeSpanEncoding() { }

        public override TimeSpan Decode(in Variant variant)
        {
            return variant._overlapped._timeSpanVal;
        }
    }

    /// <summary>
    /// An encoding for TimeOnly values.
    /// </summary>
    private sealed class TimeOnlyEncoding : VariantEncoding<TimeOnly>
    {
        public static readonly TimeOnlyEncoding Instance =
            new TimeOnlyEncoding();

        private TimeOnlyEncoding() { }

        public override TimeOnly Decode(in Variant variant)
        {
            return variant._overlapped._timeOnlyVal;
        }
    }

    #endregion

    #region BitsOverlay
    [StructLayout(LayoutKind.Explicit)]
    private struct OverlappedBits
    {
        [FieldOffset(0)]
        public bool _boolVal;

        [FieldOffset(0)]
        public byte _byteVal;

        [FieldOffset(0)]
        public sbyte _sbyteVal;

        [FieldOffset(0)]
        public short _int16Val;

        [FieldOffset(0)]
        public ushort _uint16Val;

        [FieldOffset(0)]
        public int _int32Val;

        [FieldOffset(0)]
        public uint _uint32Val;

        [FieldOffset(0)]
        public long _int64Val;

        [FieldOffset(0)]
        public ulong _uint64Val;

        [FieldOffset(0)]
        public float _singleVal;

        [FieldOffset(0)]
        public double _doubleVal;

        [FieldOffset(0)]
        public Decimal64 _decimal64Val;

        [FieldOffset(0)]
        public char _charVal;

        [FieldOffset(0)]
        public Rune _runeVal;

        [FieldOffset(0)]
        public DateTime _dateTimeVal;

        [FieldOffset(0)]
        public TimeSpan _timeSpanVal;

        [FieldOffset(0)]
        public DateOnly _dateOnlyVal;

        [FieldOffset(0)]
        public TimeOnly _timeOnlyVal;

        [FieldOffset(0)]
        public Bits _bitsVal;
    }
    #endregion
}

public enum VariantKind
{
    Null = 0,
    Boolean,
    Byte,
    SByte,
    Int16,
    UInt16,
    Int32,
    UInt32,
    Int64,
    UInt64,
    Single,
    Double,
    Decimal,
    Char,
    Rune,
    String,
    Guid,
    DateTime,
    DateOnly,
    TimeSpan,
    TimeOnly,
    Other
}