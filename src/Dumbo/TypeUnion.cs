using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Dumbo;

public static class TypeUnion
{
    private class UnionTypeInfo
    {
        public ConditionalWeakTable<Type, MemberInfo?> ValueTypeToFactoryMemberMap =
            new ConditionalWeakTable<Type, MemberInfo?>();

        public IReadOnlyList<Type>? Types;
    }

    private static ConditionalWeakTable<Type, UnionTypeInfo> _unionTypeToInfoMap =
        new ConditionalWeakTable<Type, UnionTypeInfo>();


    private static UnionTypeInfo GetUnionTypeInfo(Type unionType)
    {
        if (!_unionTypeToInfoMap.TryGetValue(unionType, out var info))
        {
            info = _unionTypeToInfoMap.GetOrCreateValue(unionType);
        }

        return info;
    }

    /// <summary>
    /// Returns a list of possible types that the type union can be or can contain.
    /// </summary>
    public static IReadOnlyList<Type> GetTypes(Type unionType)
    {
        var info = GetUnionTypeInfo(unionType);

        if (info.Types == null)
        {
            var tmp = FetchTypes();
            Interlocked.CompareExchange(ref info.Types, tmp, null);
        }

        return info.Types;

        IReadOnlyList<Type> FetchTypes()
        {
            // look for explicit Types property
            var typesProperty = unionType.GetProperty("Types", BindingFlags.Public | BindingFlags.Static);
            if (typesProperty != null)
            {
                return (IReadOnlyList<Type>)typesProperty.GetValue(null)!;
            }

            if (!unionType.IsValueType)
            {
                // look for sub types published in the same assembly
                var subTypes = unionType.Assembly.GetTypes()
                    .Where(t => !t.IsAbstract && t.IsAssignableTo(unionType))
                    .ToImmutableList();

                if (subTypes.Count > 0)
                    return subTypes;
            }
            else
            {
                // look for types in constructors
                var constructorTypes = unionType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(c => 
                        c.GetParameters() is { } parameters
                        && parameters.Length == 1)
                    .Select(c => c.GetParameters()[0].ParameterType)
                    .ToImmutableList();

                if (constructorTypes.Count > 0)
                    return constructorTypes;

                // look for types in factory methods
                var factoryMethods = unionType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(m =>
                        m.ReturnType == unionType
                        && !m.IsGenericMethod
                        && m.GetParameters().Length == 1)
                    .ToList();
                var factoryTypes =
                    factoryMethods
                    .Select(m => m.GetParameters()[0].ParameterType)
                    .Distinct()
                    .ToImmutableList();

                if (factoryTypes.Count > 0)
                    return factoryTypes;
            }

            return Array.Empty<Type>();
        }
    }

    /// <summary>
    /// Returns true if the value is a valid value for the type union type.
    /// </summary>
    public static bool IsValidValue(Type unionType, object value)
    {
        if (value != null)
        {
            var valueType = value.GetType();
            var types = GetTypes(unionType);
            return types.Any(t => valueType.IsAssignableTo(t));
        }
        else
        {
            // TODO: determine if null value is allowed
            return !unionType.IsValueType;
        }
    }

    /// <summary>
    /// Gets the corresponding factory member used to construct the union type.
    /// </summary>
    private static MemberInfo? GetFactoryMember(Type unionType, Type valueType)
    {
        var info = GetUnionTypeInfo(unionType);

        if (!info.ValueTypeToFactoryMemberMap.TryGetValue(valueType, out var member))
        {
            var tmp = FindFactoryMember();
            member = info.ValueTypeToFactoryMemberMap.GetValue(valueType, _ => tmp);
        }

        return member;

        MemberInfo? FindFactoryMember()
        {
            // union type has constructor taking value's type?
            var matchingConstructor = unionType.GetConstructors(BindingFlags.Instance|BindingFlags.Public|BindingFlags.DeclaredOnly)
                .FirstOrDefault(c => 
                    c.GetParameters() is { } parameters
                    && parameters.Length == 1
                    && parameters[0].ParameterType.IsAssignableFrom(valueType));

            if (matchingConstructor != null)
                return matchingConstructor;

            // union type has factory taking value's type?
            var matchingMethod = unionType.GetMethods(BindingFlags.Static|BindingFlags.Public|BindingFlags.DeclaredOnly)
                .FirstOrDefault(m =>
                    m.ReturnType == unionType
                    && !m.IsGenericMethod
                    && m.GetParameters() is { } parameters
                    && parameters.Length == 1
                    && parameters[0].ParameterType.IsAssignableFrom(valueType));

            if (matchingMethod != null)
                return matchingMethod;

            return null;
        }
    }

    /// <summary>
    /// Converts the value to the type union type if possible.
    /// </summary>
    public static bool TryConvert(Type unionType, object value, [NotNullWhen(true)] out object? union)
    {
        if (value != null)
        {
            var valueType = value.GetType();

            // value is instance of union type (or sub type)
            if (unionType.IsAssignableFrom(valueType))
            {
                union = value;
                return true;
            }

            var factoryMember = GetFactoryMember(unionType, valueType);
            if (factoryMember is ConstructorInfo constructor)
            {
                union = constructor.Invoke(new object[] { value });
                return true;
            }
            else if (factoryMember is MethodInfo method)
            {
                union = method.Invoke(null, new object[] { value })!;
                return true;
            }
        }

        union = null;
        return false;
    }

    /// <summary>
    /// Converts the value to the type union type if possible.
    /// </summary>
    public static bool TryConvert<TUnion>(object value, out TUnion union)
    {
        if (TryConvert(typeof(TUnion), value, out var boxedUnion)
            && boxedUnion is TUnion typedUnion)
        {
            union = typedUnion;
            return true;
        }

        union = default!;
        return false;
    }

    /// <summary>
    /// Converts the value to the type union or throws an <see cref="InvalidCastException"/>.
    /// </summary>
    public static object ConvertTo(object value, Type unionType) =>
        TryConvert(unionType, value, out var union) 
            ? union 
            : throw new InvalidCastException($"Cannot convert type '{value?.GetType().Name ?? "null"}' to union type '{unionType.Name}'");

    /// <summary>
    /// Converts the value to the type union or throws an <see cref="InvalidCastException"/>.
    /// </summary>
    public static TUnion ConvertTo<TUnion>(object value) =>
        value is TUnion union 
            ? union
            : (TUnion)ConvertTo(value, typeof(TUnion));

    /// <summary>
    /// Gets the underlying value from the boxed union type.
    /// </summary>
    public static object? GetValue(object? boxedUnion)
    {
        // union type must implement ITypeUnion or be a hierarchy subtype
        if (boxedUnion is ITypeUnion itu)
        {
            if (itu.TryGet<object>(out var value))
                return value;

            // TODO: alternate APIs?
            return null!;
        }
        else 
        {
            // already the value?
            return boxedUnion;
        }
    }
}