using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumbo;

public interface ITypeUnion
{
    //Type? Type { get; }
    bool IsType<T>();
    bool TryGet<T>([NotNullWhen(true)] out T value);
    virtual T AsType<T>() => TryGet<T>(out var value) ? value : default!;
    virtual T Get<T>() => TryGet<T>(out var value) ? value : throw new InvalidCastException();
}

public interface ITypeUnion<TSelf> : ITypeUnion
{
    abstract static bool TryCreate<T>(T value, [NotNullWhen(true)] out TSelf union);
    abstract static TSelf Create<T>(T value);
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
