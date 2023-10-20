using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Dumbo;

/// <summary>
/// A decimal value in 64 bits. Because, why not.
/// </summary>
public readonly struct Decimal64 :
    IComparable<Decimal64>,
    IEquatable<Decimal64>,
    IParsable<Decimal64>,
    ISpanParsable<Decimal64>,
    IFormattable
{
    private readonly long _bits;

    public static readonly long MaxMagnitude = 0x07FF_FFFF_FFFF_FFFF;
    public static readonly long MinMagnitude = ~MaxMagnitude;
    public static readonly byte MaxScale = 15;

    public static readonly Decimal64 Zero = new Decimal64(0);
    public static readonly Decimal64 One = new Decimal64(1, 0);
    public static readonly Decimal64 MinValue = new Decimal64(MinMagnitude, 0);
    public static readonly Decimal64 MaxValue = new Decimal64(MaxMagnitude, 0);

    private Decimal64(long bits)
    {
        _bits = bits;
    }

    /// <summary>
    /// Constructs a new <see cref="Decimal64"/> given a magnitude and scale.
    /// The magnitude is the number as an integer without any decimal places.
    /// The scale is the number of decimal places.
    /// </summary>
    public Decimal64(long magnitude, byte scale)
    {
        if (magnitude < MinMagnitude || magnitude > MaxMagnitude)
            throw new ArgumentOutOfRangeException(nameof(magnitude));
        if (scale < 0 || scale > 15)
            throw new ArgumentOutOfRangeException(nameof(scale));
        _bits = magnitude << 4 | scale;
    }

    /// <summary>
    /// Constructs a new <see cref="Decimal64"/> from bits encoded in a <see cref="Int64"/>.
    /// </summary>
    public static Decimal64 FromBits(long bits) => new Decimal64(bits);

    /// <summary>
    /// Gets the encoded bits of the <see cref="Decimal64"/> as a <see cref="Int64"/>.
    /// </summary>
    /// <returns></returns>
    public long GetBits() => _bits;

    /// <summary>
    /// The number of decimal places in the value, a value between 0 and 15.
    /// </summary>
    public byte Scale => (byte)(_bits & 0xF);

    /// <summary>
    /// The unscaled integer magnitude of the <see cref="SmallDecimal"/>.
    /// </summary>
    public long Magnitude => _bits >> 4;

    /// <summary>
    /// True if the <see cref="Decimal64"/> is an integer.
    /// </summary>
    public bool IsInteger => Scale == 0;

    /// <summary>
    /// Converts a <see cref="Decimal"/> value to a <see cref="Decimal64"/>
    /// Returns true if conversion succeeded.
    public static bool TryConvert(decimal value, out Decimal64 dec64)
    {
        var scale = value.Scale;
        if (scale <= 15)
        {
            // get unscaled magnitude (which may be larger than can fit in long)
            var magnitude = value * s_scaleFactor[scale];

            if (magnitude >= MinMagnitude && magnitude <= MaxMagnitude)
            {
                dec64 = new Decimal64((long)magnitude, scale);
                return true;
            }
        }

        dec64 = default;
        return false;
    }

    // because indexing is faster than Math.Pow()
    private static readonly long[] s_scaleFactor = new long[]
    {
        1,                  // 0
        10,                 // 1
        100,                // 2
        1000,               // 3
        10000,              // 4
        100000,             // 5
        1000000,            // 6
        10000000,           // 7
        100000000,          // 8
        1000000000,         // 9
        10000000000,        // 10
        100000000000,       // 11
        1000000000000,      // 12
        10000000000000,     // 13
        100000000000000,    // 14
        1000000000000000    // 15
    };

    /// <summary>
    /// Converts a <see cref="Int64"/> value to a <see cref="Decimal64"/>.
    /// Returns true if conversion succeeded.
    /// </summary>
    public static bool TryConvert(long value, out Decimal64 dec64)
    {
        if (value >= MinMagnitude && value <= MaxMagnitude)
        {
            dec64 = new Decimal64(value, 0);
            return true;
        }

        dec64 = default;
        return false;
    }

    /// <summary>
    /// Converts a <see cref="Decimal"/> to a <see cref="Decimal64"/>
    /// or throws an <see cref="OverflowException"/> if it is not possible.
    public static Decimal64 Convert(decimal value) =>
        TryConvert(value, out var dec64) ? dec64 : throw new OverflowException();

    /// <summary>
    /// Converts a <see cref="Int64"/> to a <see cref="DEcimal64"/>
    /// or throws an <see cref="OverflowException"/> if it is not possible.
    /// </summary>
    public static Decimal64 Convert(long value) =>
        TryConvert(value, out var dec64) ? dec64 : throw new OverflowException();

    /// <summary>
    /// Removes the decimal fraction.
    /// </summary>
    public readonly Decimal64 Truncate()
    {
        var tens = (int)Math.Pow(10, Math.Abs(Scale));
        var newMagnitude = Magnitude / tens;
        return new Decimal64(newMagnitude, 0);
    }

    /// <summary>
    /// Converts the <see cref="Decimal64"/> to a <see cref="Decimal"/> value.
    /// </summary>
    public readonly decimal ToDecimal()
    {
        var scale = (byte)(_bits & 0xF);
        var negative = _bits < 0;
        var magnitude = Math.Abs(_bits >> 4);
        var lo = unchecked((int)(magnitude & 0x0000_0000_FFFF_FFFF));
        var mid = unchecked((int)((magnitude & 0x0FFF_FFFF_0000_0000) >> 32));
        var d = new decimal(lo, mid, 0, negative, scale);
        return d;
    }

    public readonly override string ToString() =>
        ToDecimal().ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) =>
        ToDecimal().ToString(format, formatProvider);

    public readonly int CompareTo(Decimal64 other) =>
        ToDecimal().CompareTo(other.ToDecimal());

    public readonly int CompareTo(decimal other) =>
        ToDecimal().CompareTo(other);

    public readonly bool Equals(Decimal64 other) =>
        ToDecimal().Equals(other.ToDecimal());

    public readonly bool Equals(decimal other) =>
        ToDecimal().Equals(other);

    public readonly override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is Decimal64 sd && Equals(sd)
        || obj is decimal d && ToDecimal().Equals(d);

    public readonly override int GetHashCode() =>
        ToDecimal().GetHashCode();

    public static Decimal64 Parse(string s, IFormatProvider? provider) =>
        Convert(decimal.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Decimal64 result)
    {
        if (decimal.TryParse(s, provider, out var value) && TryConvert(value, out result))
            return true;
        result = default;
        return false;
    }

    public static Decimal64 Parse(string s) =>
        Convert(decimal.Parse(s, null));

    public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out Decimal64 result) =>
        TryParse(s, null, out result);

    public static Decimal64 Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
        Convert(decimal.Parse(s, provider));

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Decimal64 result)
    {
        if (decimal.TryParse(s, provider, out var value) && TryConvert(value, out result))
            return true;
        result = default;
        return false;
    }

    public static bool operator ==(Decimal64 a, Decimal64 b) =>
        a.Equals(b);

    public static bool operator ==(Decimal64 a, decimal b) =>
        a.Equals(b);

    public static bool operator ==(decimal a, Decimal64 b) =>
        a.Equals(b.ToDecimal());

    public static bool operator !=(Decimal64 a, Decimal64 b) =>
        !a.Equals(b);

    public static bool operator !=(Decimal64 a, decimal b) =>
        !a.Equals(b);

    public static bool operator !=(decimal a, Decimal64 b) =>
        !a.Equals(b.ToDecimal());

    public static implicit operator decimal(Decimal64 dec64) =>
        dec64.ToDecimal();

    public static implicit operator Decimal64(int value) =>
        Convert(value);

    public static explicit operator Decimal64(decimal value) =>
        Convert(value);

    public static explicit operator Decimal64(long value) =>
        Convert(value);

    public static explicit operator long(Decimal64 value) =>
        value.Truncate().Magnitude;
}
