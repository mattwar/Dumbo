using Dumbo;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.CustomUnions
{
    [TestClass]
    public class BoxedUnionTests
    {
        public record Cat(string name);
        public record Dog(string name);

        /// <summary>
        /// Custom TypeUnion wrapper over object field
        /// </summary>
        public readonly struct CatOrDog : ITypeUnion
        {
            private readonly object _value;

            private CatOrDog(object value)
            {
                _value = value;
            }

            public static implicit operator CatOrDog(Cat cat) => new CatOrDog(cat);
            public static implicit operator CatOrDog(Dog dog) => new CatOrDog(dog);

            public static CatOrDog Convert<T>(T value)
            {
                if (value is Cat cat)
                {
                    return cat;
                }
                else if (value is Dog dog)
                {
                    return dog;
                }
                else if (value is ITypeUnion union)
                {
                    if (union.TryGet<Cat>(out var ucat))
                        return ucat;
                    else if (union.TryGet<Dog>(out var udog))
                        return udog;
                }
                throw new InvalidCastException();
            }

            public bool IsCat => _value is Cat;
            public bool IsDog => _value is Dog;

            public Cat Cat => _value is Cat cat ? cat : null!;
            public Dog Dog => _value is Dog dog ? dog : null!;

            public bool TryGetCat(out Cat cat) => TryGet(out cat);
            public bool TryGetDog(out Dog dog) => TryGet(out dog);

            private bool TryGet<T>(out T value)
            {
                if (_value is T t)
                {
                    value = t;
                    return true;
                }
                value = default!;
                return false;
            }

            //Type? ITypeUnion.Type => _value?.GetType();
            bool ITypeUnion.IsType<T>() => _value is T;
            bool ITypeUnion.TryGet<T>([NotNullWhen(true)] out T value) => TryGet(out value);
        }
    }
}
