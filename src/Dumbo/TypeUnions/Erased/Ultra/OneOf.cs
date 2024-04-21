using System.Diagnostics.CodeAnalysis;

namespace Dumbo.TypeUnions.Erased.Ultra;


public struct Union
{
    public readonly ushort Id;
    public readonly ushort Kind;
    public readonly Variant Value;

    public Union(ushort id, ushort kind, Variant value)
    {
        this.Id = id;
        this.Kind = kind;
        this.Value = value;
    }

    private static ushort _lastId;
    public static ushort CreateId() => ++_lastId;
}

public struct OneOf<T1, T2> // implicit extension of variant
    where T1 : notnull
    where T2 : notnull
{
    private Union _union;

    private OneOf(Union union)
    {
        _union = union;
    }

    private static readonly ushort _id = Union.CreateId();

    public int Tag => _union.Id == _id ? _union.Kind
        : _type1Encoder.IsType(_union.Value) ? 1
        : _type2Encoder.IsType(_union.Value) ? 2
        : default;

    public bool IsNull => _union.Value.IsNull;
    public static OneOf<T1, T2> Null => default;

    public static implicit operator Union(OneOf<T1, T2> union) => union._union;
    public static explicit operator OneOf<T1, T2>(Union union) => new OneOf<T1, T2>(union);

    public override string ToString() => _union.Value.ToString();

    private static Variant.Encoder<T1> _type1Encoder = Variant.Encoder<T1>.Instance;
    private static Variant.Encoder<T2> _type2Encoder = Variant.Encoder<T2>.Instance;

    public bool IsType<TType>() => _union.Value.IsType<TType>();
    public bool TryGet<TType>([NotNullWhen(true)] out TType value) => _union.Value.TryGet<TType>(out value);
    public TType Get<TType>() => _union.Value.Get<TType>();

    public bool IsType1 => _type1Encoder.IsType(_union.Value);
    public bool TryGetType1(out T1 value) => _type1Encoder.TryDecode(_union.Value, out value);
    public T1 GetType1() => TryGetType1(out var value) ? value : throw new InvalidCastException();
    public static implicit operator OneOf<T1, T2>(T1 value) => new OneOf<T1, T2>(new Union(_id, 1, _type1Encoder.Encode(value)));
    public static explicit operator T1(OneOf<T1, T2> union) => union.GetType1();

    public bool IsType2 => _type2Encoder.IsType(_union.Value);
    public bool TryGetType2(out T2 value) => _type2Encoder.TryDecode(_union.Value, out value);
    public T2 GetType2() => TryGetType2(out var value) ? value : throw new InvalidCastException();
    public static implicit operator OneOf<T1, T2>(T2 value) => new OneOf<T1, T2>(new Union(_id, 2, _type2Encoder.Encode(value)));
    public static explicit operator T2(OneOf<T1, T2> union) => union.GetType2();
}


public struct OneOf<T1, T2, T3> // implicit extension of variant
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
{
    private Union _union;

    private OneOf(Union union)
    {
        _union = union;
    }

    private static readonly ushort _id = Union.CreateId();

    public int Tag => _union.Id == _id ? _union.Kind
        : _type1Encoder.IsType(_union.Value) ? 1
        : _type2Encoder.IsType(_union.Value) ? 2
        : _type3Encoder.IsType(_union.Value) ? 3
        : default;

    public bool IsNull => _union.Value.IsNull;
    public static OneOf<T1, T2, T3> Null => default;

    public static implicit operator Union(OneOf<T1, T2, T3> union) => union._union;
    public static explicit operator OneOf<T1, T2, T3>(Union union) => new OneOf<T1, T2, T3>(union);

    public override string ToString() => _union.Value.ToString();

    private static Variant.Encoder<T1> _type1Encoder = Variant.Encoder<T1>.Instance;
    private static Variant.Encoder<T2> _type2Encoder = Variant.Encoder<T2>.Instance;
    private static Variant.Encoder<T3> _type3Encoder = Variant.Encoder<T3>.Instance;

    public bool IsType<TType>() => _union.Value.IsType<TType>();
    public bool TryGet<TType>([NotNullWhen(true)] out TType value) => _union.Value.TryGet<TType>(out value);
    public TType Get<TType>() => _union.Value.Get<TType>();

    public bool IsType1 => _type1Encoder.IsType(_union.Value);
    public bool TryGetType1(out T1 value) => _type1Encoder.TryDecode(_union.Value, out value);
    public T1 GetType1() => TryGetType1(out var value) ? value : throw new InvalidCastException();
    public static implicit operator OneOf<T1, T2, T3>(T1 value) => new OneOf<T1, T2, T3>(new Union(_id, 1, _type1Encoder.Encode(value)));
    public static explicit operator T1(OneOf<T1, T2, T3> union) => union.GetType1();

    public bool IsType2 => _type2Encoder.IsType(_union.Value);
    public bool TryGetType2(out T2 value) => _type2Encoder.TryDecode(_union.Value, out value);
    public T2 GetType2() => TryGetType2(out var value) ? value : throw new InvalidCastException();
    public static implicit operator OneOf<T1, T2, T3>(T2 value) => new OneOf<T1, T2, T3>(new Union(_id, 2, _type2Encoder.Encode(value)));
    public static explicit operator T2(OneOf<T1, T2, T3> union) => union.GetType2();

    public bool IsType3 => _type3Encoder.IsType(_union.Value);
    public bool TryGetType3(out T3 value) => _type3Encoder.TryDecode(_union.Value, out value);
    public T3 GetType3() => TryGetType3(out var value) ? value : throw new InvalidCastException();
    public static implicit operator OneOf<T1, T2, T3>(T3 value) => new OneOf<T1, T2, T3>(new Union(_id, 3, _type3Encoder.Encode(value)));
    public static explicit operator T3(OneOf<T1, T2, T3> union) => union.GetType3();
}
