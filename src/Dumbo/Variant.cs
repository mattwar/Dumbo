using Reference = System.Object;
using Bits = System.UInt64;
using System.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

namespace Dumbo;

/// <summary>
/// A wrapper type with a small footprint that can hold any value and avoids boxing of small structs that do not contain references.
/// </summary>
public readonly struct Variant : ITypeUnion<Variant>, IEquatable<Variant>
{
    private readonly Reference? _reference;
    private readonly Bits _bits;

    private Variant(Reference? reference, Bits bits)
    {
        _reference = reference;
        _bits = bits;
    }

    /// <summary>
    /// A variant with the null value.
    /// </summary>
    public static readonly Variant Null = default;

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
    /// The encoding of this variant
    /// </summary>
    private VariantEncoding Encoding => 
        VariantEncoding.GetEncoding(in this);

    /// <summary>
    /// The type of the value held within this variant.
    /// </summary>
    public Type Type =>
        Encoding.GetType(in this);

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

    public static explicit operator bool(Variant value) => value.Get<bool>();
    public static implicit operator sbyte(Variant value) => value.Get<sbyte>();
    public static implicit operator short(Variant value) => value.Get<short>();
    public static implicit operator int(Variant value) => value.Get<int>();
    public static implicit operator long(Variant value) => value.Get<long>();
    public static implicit operator byte(Variant value) => value.Get<byte>();
    public static implicit operator ushort(Variant value) => value.Get<ushort>();
    public static implicit operator uint(Variant value) => value.Get<uint>();
    public static implicit operator ulong(Variant value) => value.Get<ulong>();
    public static implicit operator float(Variant value) => value.Get<float>();
    public static implicit operator double(Variant value) => value.Get<double>();
    public static implicit operator Decimal64(Variant value) => value.Get<Decimal64>();
    public static implicit operator decimal(Variant value) => value.Get<decimal>();
    public static implicit operator char(Variant value) => value.Get<char>();
    public static implicit operator Rune(Variant value) => value.Get<Rune>();
    public static implicit operator String(Variant value) => value.Get<string>();
    public static implicit operator DateOnly(Variant value) => value.Get<DateOnly>();
    public static implicit operator TimeOnly(Variant value) => value.Get<TimeOnly>();
    public static implicit operator DateTime(Variant value) => value.Get<DateTime>();
    public static implicit operator TimeSpan(Variant value) => value.Get<TimeSpan>();
    public static implicit operator Guid(Variant value) => value.Get<Guid>();

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
                else if (sizeof(TValue) <= sizeof(Bits))
                {
                    // no references and small enough to fit in bits
                    return new BitsEncoder<TValue>();
                }
                else if (ttype == typeof(decimal))
                {
                    // use decimal encoder to conditionaly fit most common decimals in bits
                    return (VariantEncoder<TValue>)(object)new DecimalEncoder();
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
            return new Variant(_encoding, bits);
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
            return new Variant(refValue, (Bits)_encodingId);
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
                var bits = Unsafe.As<Decimal64, Bits>(ref decVal);
                return new Variant(DecimalEncoding.Instance, bits);
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
        public virtual bool IsNull(in Variant variant) => false;
        public abstract bool IsType<T>(in Variant variant);
        public abstract bool TryGet<T>(in Variant variant, out T value);
        public abstract string GetString(in Variant variant);
        public abstract bool IsEqual<T>(in Variant variant, T value);
        public abstract bool IsEqual(in Variant variant, in Variant other);
        public abstract int GetHashCode(in Variant variant);

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
            else if (variant._bits > 0)
            {
                // _bits has the encoding's id
                var encodingId = (int)variant._bits;
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

    /// <summary>
    /// Abstract base for a strongly typed variant encoding.
    /// </summary>
    private abstract class VariantEncoding<TValue> : VariantEncoding
    {
        public abstract TValue Decode(in Variant variant);

        public override Type GetType(in Variant variant) => 
            typeof(TValue);

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
            Bits bits = variant._bits;
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

        public override Type GetType(in Variant variant) =>
            variant._reference?.GetType() ?? typeof(object);

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
            Bits bits = variant._bits;
            return Unsafe.As<Bits, Decimal64>(ref bits).ToDecimal();

        }
    }
    #endregion
}