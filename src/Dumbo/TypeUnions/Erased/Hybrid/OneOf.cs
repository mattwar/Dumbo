using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Dumbo.TypeUnions.Erased.Hybrid;

public struct OneOf<T1, T2> // implicit extension of variant
    where T1 : notnull
    where T2 : notnull
{
    private Variant _variant;
    private OneOf(Variant value)
    {
        _variant = value;
    }

    public int Tag
    {
        get
        {
            var type = _variant.Type;
            
            if (!_typeToTagMap.TryGetValue(type, out var tag))
            {
                tag = ImmutableInterlocked.GetOrAdd(ref _typeToTagMap, type, ComputeTag());
            }

            return tag;
        }
    }

    private static ImmutableDictionary<Type, int> _typeToTagMap =
        ImmutableDictionary<Type, int>.Empty
        .Add(typeof(T1), 1)
        .Add(typeof(T2), 2);

    private int ComputeTag() =>
        _type1Encoder.IsType(_variant) ? 1
        : _type2Encoder.IsType(_variant) ? 2
        : 0;

    public bool IsNull => _variant.IsNull;
    public static OneOf<T1, T2> Null => default;

    public static implicit operator Variant(OneOf<T1, T2> union) => union._variant;
    public static explicit operator OneOf<T1, T2>(Variant variant) => new OneOf<T1, T2>(variant);

    public override string ToString() => _variant.ToString();

    private static Variant.Encoder<T1> _type1Encoder = Variant.Encoder<T1>.Instance;
    private static Variant.Encoder<T2> _type2Encoder = Variant.Encoder<T2>.Instance;

    public bool IsType<TType>() => _variant.IsType<TType>();
    public bool TryGet<TType>([NotNullWhen(true)] out TType value) => _variant.TryGet<TType>(out value);
    public TType Get<TType>() => _variant.Get<TType>();

    public bool IsType1 => _type1Encoder.IsType(_variant);
    public bool TryGetType1(out T1 value) => _type1Encoder.TryDecode(_variant, out value);
    public T1 GetType1() => TryGetType1(out var value) ? value : throw new InvalidCastException();
    public static implicit operator OneOf<T1, T2>(T1 value) => new OneOf<T1, T2>(_type1Encoder.Encode(value));
    public static explicit operator T1(OneOf<T1, T2> union) => union.GetType1();

    public bool IsType2 => _type2Encoder.IsType(_variant);
    public bool TryGetType2(out T2 value) => _type2Encoder.TryDecode(_variant, out value);
    public T2 GetType2() => TryGetType2(out var value) ? value : throw new InvalidCastException();
    public static implicit operator OneOf<T1, T2>(T2 value) => new OneOf<T1, T2>(_type2Encoder.Encode(value));
    public static explicit operator T2(OneOf<T1, T2> union) => union.GetType2();
}

public struct OneOf<T1, T2, T3> // implicit extension of variant
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
{
    private Variant _variant;

    private OneOf(Variant value)
    {
        _variant = value;
    }

    public int Tag
    {
        get
        {
            var type = _variant.Type;

            if (!_typeToTagMap.TryGetValue(type, out var tag))
            {
                tag = ImmutableInterlocked.GetOrAdd(ref _typeToTagMap, type, ComputeTag());
            }

            return tag;
        }
    }

    private static ImmutableDictionary<Type, int> _typeToTagMap =
        ImmutableDictionary<Type, int>.Empty
        .Add(typeof(T1), 1)
        .Add(typeof(T2), 2)
        .Add(typeof(T3), 3);

    private int ComputeTag() =>
        _type1Encoder.IsType(_variant) ? 1
        : _type2Encoder.IsType(_variant) ? 2
        : _type3Encoder.IsType(_variant) ? 3
        : 0;

    public static implicit operator Variant(OneOf<T1, T2, T3> union) => union._variant;
    public static explicit operator OneOf<T1, T2, T3>(Variant variant) => new OneOf<T1, T2, T3>(variant);

    public bool IsNull => _variant.IsNull;
    public static OneOf<T1, T2, T3> Null => default;

    public override string ToString() => _variant.ToString();

    private static Variant.Encoder<T1> _type1Encoder = Variant.Encoder<T1>.Instance;
    private static Variant.Encoder<T2> _type2Encoder = Variant.Encoder<T2>.Instance;
    private static Variant.Encoder<T3> _type3Encoder = Variant.Encoder<T3>.Instance;

    public bool IsType<TType>() => _variant.IsType<TType>();
    public bool TryGet<TType>([NotNullWhen(true)] out TType value) => _variant.TryGet<TType>(out value);
    public TType Get<TType>() => _variant.Get<TType>();

    public bool IsType1 => _type1Encoder.IsType(_variant);
    public bool TryGetType1(out T1 value) => _type1Encoder.TryDecode(_variant, out value);
    public T1 GetType1() => TryGetType1(out var value) ? value : throw new InvalidCastException();
    public static implicit operator OneOf<T1, T2, T3>(T1 value) => new OneOf<T1, T2, T3>(_type1Encoder.Encode(value));
    public static explicit operator T1(OneOf<T1, T2, T3> union) => union.GetType1();

    public bool IsType2 => _type2Encoder.IsType(_variant);
    public bool TryGetType2(out T2 value) => _type2Encoder.TryDecode(_variant, out value);
    public T2 GetType2() => TryGetType2(out var value) ? value : throw new InvalidCastException();
    public static implicit operator OneOf<T1, T2, T3>(T2 value) => new OneOf<T1, T2, T3>(_type2Encoder.Encode(value));
    public static explicit operator T2(OneOf<T1, T2, T3> union) => union.GetType2();

    public bool IsType3 => _type3Encoder.IsType(_variant);
    public bool TryGetType3(out T3 value) => _type3Encoder.TryDecode(_variant, out value);
    public T3 GetType3() => TryGetType3(out var value) ? value : throw new InvalidCastException();
    public static implicit operator OneOf<T1, T2, T3>(T3 value) => new OneOf<T1, T2, T3>(_type3Encoder.Encode(value));
    public static explicit operator T3(OneOf<T1, T2, T3> union) => union.GetType3();
}
