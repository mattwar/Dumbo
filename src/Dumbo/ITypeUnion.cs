using System.Diagnostics.CodeAnalysis;

namespace Dumbo;

public interface ITypeUnion
{
    bool IsType<T>();
    bool TryGet<T>([NotNullWhen(true)] out T value);

    virtual Variant ToVariant() =>
        TryGet<object>(out var value)
            ? Variant.Create(value)
            : Variant.Null;
}

public interface ITypeUnion<TSelf> : ITypeUnion
{
    abstract static bool TryCreate<T>(T value, [NotNullWhen(true)] out TSelf union);
}

/// <summary>
/// A class that facilitates conditionally constructing type unions via <see cref="ITypeUnion{TSelf}"/>,
/// if the interface is supported.
/// </summary>
public abstract class TypeUnionFactory<TUnion>
{
    private static readonly bool _isValidUnionType;
    private static TypeUnionFactory<TUnion>? _instance;

    static TypeUnionFactory()
    {
        _isValidUnionType = typeof(TUnion).IsAssignableTo(typeof(ITypeUnion<>).MakeGenericType(typeof(TUnion)));
    }

    public static bool TryGetFactory([NotNullWhen(true)] out TypeUnionFactory<TUnion> factory)
    {
        if (_instance == null && _isValidUnionType)
        {
            var type = typeof(TypeUnionFactoryImpl<>).MakeGenericType(typeof(TUnion));
            factory = (TypeUnionFactory<TUnion>)Activator.CreateInstance(type)!;
            Interlocked.CompareExchange(ref _instance, factory, null);
        }

        factory = _instance!;
        return _instance is not null;
    }

    public abstract bool TryCreate<T>(T value, [NotNullWhen(true)] out TUnion union);

    private class NotATypeUnion : TypeUnionFactory<TUnion>
    {
        public static readonly NotATypeUnion Instance = new NotATypeUnion();

        public override bool TryCreate<T>(T value, [NotNullWhen(true)] out TUnion union)
        {
            union = default!;
            return false;
        }
    }
}

internal class TypeUnionFactoryImpl<TUnion> : TypeUnionFactory<TUnion>
    where TUnion : ITypeUnion<TUnion>
{
    public override bool TryCreate<T>(T value, [NotNullWhen(true)] out TUnion union) =>
        TUnion.TryCreate(value, out union);
}

/// <summary>
/// A class that facilitates accessing a type union's value via <see cref="ITypeUnion"/> 
/// without boxing of the union.
/// </summary>
public abstract class TypeUnionAccessor<TUnion>
{
    private static readonly bool _isValidUnionType;
    private static TypeUnionAccessor<TUnion>? _instance;

    static TypeUnionAccessor()
    {
        _isValidUnionType = typeof(TUnion).IsAssignableTo(typeof(ITypeUnion));
    }

    public static bool TryGetAccessor([NotNullWhen(true)] out TypeUnionAccessor<TUnion> accessor)
    {
        if (_instance == null && _isValidUnionType)
        {
            var type = typeof(TypeUnionAccessorImpl<>).MakeGenericType(typeof(TUnion));
            accessor = (TypeUnionAccessor<TUnion>)Activator.CreateInstance(type)!;
            Interlocked.CompareExchange(ref _instance, accessor, null);
        }

        accessor = _instance!;
        return _instance is not null;
    }

    public abstract bool TryGet<T>(in TUnion union, [NotNullWhen(true)] out T value);
}

internal class TypeUnionAccessorImpl<TUnion> : TypeUnionAccessor<TUnion>
    where TUnion : ITypeUnion
{
    public override bool TryGet<T>(in TUnion union, [NotNullWhen(true)] out T value) =>
        union.TryGet<T>(out value);
}



#if false
public class TypeUnionAttribute : Attribute
{
    public IReadOnlyList<Type> Types { get; }

    public TypeUnionAttribute(params Type[] types)
    {
        this.Types = types;
    }
}
#endif
