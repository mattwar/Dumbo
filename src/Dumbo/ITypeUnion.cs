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

public interface IClosedTypeUnion<TSelf> : ITypeUnion<TSelf>
{
    abstract static IReadOnlyList<Type> Types { get; }
}

/// <summary>
/// A class that facilitates conditionally constructing type unions via <see cref="ITypeUnion{TSelf}"/>,
/// if the interface is supported.
/// </summary>
public abstract class TypeUnionFactory<TUnion>
{
    private static readonly Type? _factoryType;
    private static TypeUnionFactory<TUnion>? _instance;

    static TypeUnionFactory()
    {
        if (typeof(TUnion).IsAssignableTo(typeof(ITypeUnion<>).MakeGenericType(typeof(TUnion))))
        {
            _factoryType = typeof(TypeUnionFactoryImpl<>).MakeGenericType(typeof(TUnion));
        }
    }

    public static bool TryGetFactory([NotNullWhen(true)] out TypeUnionFactory<TUnion> factory)
    {
        if (_instance == null && _factoryType != null)
        {
            factory = (TypeUnionFactory<TUnion>)Activator.CreateInstance(_factoryType)!;
            Interlocked.CompareExchange(ref _instance, factory, null);
        }

        factory = _instance!;
        return _instance is not null;
    }

    public abstract bool TryCreate<T>(T value, [NotNullWhen(true)] out TUnion union);
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
    private static readonly Type? _accessorType;
    private static TypeUnionAccessor<TUnion>? _instance;

    static TypeUnionAccessor()
    {
        if (typeof(TUnion).IsAssignableTo(typeof(ITypeUnion)))
        {
            _accessorType = typeof(TypeUnionAccessorImpl<>).MakeGenericType(typeof(TUnion));
        }
    }

    public static bool TryGetAccessor([NotNullWhen(true)] out TypeUnionAccessor<TUnion> accessor)
    {
        if (_instance == null && _accessorType != null)
        {
            accessor = (TypeUnionAccessor<TUnion>)Activator.CreateInstance(_accessorType)!;
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

public class TypeUnionAttribute : Attribute
{
    public IReadOnlyList<Type> Types { get; }

    public TypeUnionAttribute(params Type[] types)
    {
        this.Types = types;
    }
}
