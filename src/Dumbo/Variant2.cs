#if VARIANT2
using System.Runtime.CompilerServices;

using Reference = System.Object;
using Bits = System.UInt64;
using System.Text;

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

namespace Dumbo;

public readonly struct Variant
{
    private readonly Reference? _reference;
    private readonly Bits _bits;

    private Variant(Reference? reference, Bits bits)
    {
        _reference = reference;
        _bits = bits;
    }

#if false
    internal enum Kind
    {
        SmallValue,
        WrapperStruct,
        Reference,
    }

    internal Kind Encoding =>
        _reference is TypeHolder ? Kind.SmallValue :
        _bits > 0 ? Kind.WrapperStruct :
        Kind.Reference;
#endif

    public static readonly Variant Null = default;

    public static unsafe Variant Create<T>(T value)
    {
        // Null
        if (value == null) 
            return new Variant(null, default);

        // Value type
        if (typeof(T).IsValueType)
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                // Wrapper struct around a single reference
                if (sizeof(T) == sizeof(Reference))
                {
                    // store wrapped reference in _reference
                    var reference = Unsafe.As<T, Reference>(ref value);
                    // store encoding id in _bits
                    var bits = (Bits)WrapperStructEncoding<T>.Instance.Id;
                    return new Variant(reference, bits);
                }
            }
            // Small non-nullable struct without references
            else if (sizeof(T) <= sizeof(Bits) && default(T) != null)
            {
                // store encoding in _reference
                Reference referenced = SmallValueEncoding<T>.Instance;
                // store small value in _bits
                Bits bits = default;
                Unsafe.As<Bits, T>(ref bits) = value;
                return new Variant(referenced, bits);
            }
        }

        // Reference or boxed value
        // store referenced/boxed value in _reference
        // store zero in _bits
        return new Variant(value, default);
    }

    private bool TryGetEncoding(out TypeEncoding encoding)
    {
        if (_reference is TypeEncoding enc)
        {
            encoding = enc;
            return true;
        }
        else if (_bits > 0)
        {
            // _bits has the encoding's id
            var encodingId = (int)_bits;
            encoding = TypeEncoding.GetEncoding(encodingId);
            return true;
        }
        else
        {
            encoding = null!;
            return false;
        }
    }

    public Type? Type =>
        TryGetEncoding(out var encoding)
            ? encoding.Type
            : _reference?.GetType();

    public bool IsNull => 
        TryGetEncoding(out var encoding)
            ? encoding.IsNull(in this)
            : _reference is null;

    public bool IsType<T>() =>
        TryGetEncoding(out var encoding)
            ? encoding.Is<T>(in this)
            : _reference is T;

    public T AsType<T>() =>
        TryGet<T>(out var value) ? value! : default!;

    public bool TryGet<T>(out T value)
    {
        if (TryGetEncoding(out var encoding)
            && encoding.TryGet(in this, out value))
        {
            return true;
        }
        else if (_reference is T tval)
        {
            value = tval;
            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }

    public T Get<T>() =>
        TryGet<T>(out var value) ? value : throw new InvalidCastException();

    public override string ToString() =>
        TryGetEncoding(out var encoding)
            ? encoding.GetString(in this)
            : _reference?.ToString() ?? "";

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

    private abstract class TypeEncoding
    {
        public abstract int Id { get; }
        public abstract Type Type { get; }
        public abstract bool IsNull(in Variant variant);
        public abstract bool Is<T>(in Variant variant);
        public abstract bool TryGet<T>(in Variant variant, out T value);
        public abstract string GetString(in Variant variant);

        public static TypeEncoding GetEncoding(int id) =>
            s_encodingList[id];

        protected static List<TypeEncoding> s_encodingList =
            new List<TypeEncoding>(capacity: 1024);
    }

    private abstract class TypeEncoding<T> : TypeEncoding
    {
        private int _id = 0;

        public override int Id
        {
            get
            {
                if (_id == 0)
                {
                    lock (s_encodingList)
                    {
                        s_encodingList.Add(this);
                        _id = s_encodingList.Count;
                    }
                }

                return _id;
            }
        }

        public abstract T GetValue(in Variant variant);

        public override Type Type => typeof(T);

        public override bool Is<TOther>(in Variant variant) =>
            TryGet<TOther>(in variant, out _);

        public override bool TryGet<TOther>(in Variant variant, out TOther value)
        {
            if (GetValue(in variant) is TOther other)
            {
                value = other;
                return true;
            }

            value = default!;
            return false;
        }

        public override string GetString(in Variant variant) =>
            TryGet<T>(in variant, out var value) ? value?.ToString() ?? "" : "";
    }

    /// <summary>
    /// Encoding for a small value or overlappable struct that fits within the bits.
    /// </summary>
    private sealed class SmallValueEncoding<T> : TypeEncoding<T>
    {
        internal static SmallValueEncoding<T> Instance = new SmallValueEncoding<T>();

        public override T GetValue(in Variant variant)
        {
            Bits bits = variant._bits;
            return Unsafe.As<Bits, T>(ref bits);
        }

        public override bool IsNull(in Variant variant) =>
            false;
    }

    /// <summary>
    /// Encoding for a struct wrapper around a single reference.
    /// </summary>
    private sealed class WrapperStructEncoding<T> : TypeEncoding<T>
    {
        internal static WrapperStructEncoding<T> Instance = new WrapperStructEncoding<T>();

        public override T GetValue(in Variant variant)
        {
            // wrapped reference is stored in _reference
            Reference? refValue = variant._reference;
            return Unsafe.As<Reference, T>(ref refValue!);
        }

        public override bool IsNull(in Variant variant) =>
            variant._reference == null;
    }
}
#endif