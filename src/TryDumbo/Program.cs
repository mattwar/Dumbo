using Dumbo;

//  switch (animal)
//  { 
//      case Cat cat: ...;
//      case Dog dog: ...;
//  }


// Tagged union no additional types
Animal0 animal0 = Animal0.Cat("Fudge", true);
switch (animal0.Kind)
{
    case Animal0.Tag.Cat:
        if (!animal0.TryGetCat(out var cat))
            goto default;
        Console.WriteLine($"I am a cat named {cat.name}");
        break;
    case Animal0.Tag.Dog:
        if (!animal0.TryGetDog(out var dog))
            goto default;
        Console.WriteLine($"I am a dog named {dog.name}");
        break;
    default:
        throw new InvalidOperationException($"Missing match for {nameof(animal0)}.");
}


// Tagged union with nested types
Animal1 animal1 = new Animal1.Cat("Fudge", true);
switch (animal1.Kind)
{
    case Animal1.Tag.Cat:
        if (!animal1.TryGetCat(out var cat))
            goto default;
        Console.WriteLine($"I am a cat named {cat.name}");
        break;
    case Animal1.Tag.Dog:
        if (!animal1.TryGetDog(out var dog))
            goto default;
        Console.WriteLine($"I am a dog named {dog.name}");
        break;
    default:
        throw new InvalidOperationException($"Missing match for {nameof(animal1)}.");
}


// Tagged union with external types
var c = new Cat("Fudge", true);
Animal2 animal2 = c;
switch (animal2.Kind)
{
    case Animal2.Tag.Cat:
        if (!animal2.TryGetCat(out var cat))
            goto default;
        Console.WriteLine($"I am a cat named {cat.name}");
        break;
    case Animal2.Tag.Dog:
        if (!animal2.TryGetDog(out var dog))
            goto default;
        Console.WriteLine($"I am a dog named {dog.name}");
        break;
    default:
        throw new InvalidOperationException($"Missing match for {nameof(animal2)}.");
}


// Typed union with external types
Animal3 animal3 = c;
switch (animal3.Kind)
{
    case Animal3.Tag.Type1:
        if (!animal3.TryGetType1(out var cat))
            goto default;
        Console.WriteLine($"I am a cat named {cat.name}");
        break;
    case Animal3.Tag.Type2:
        if (!animal3.TryGetType2(out var dog))
            goto default;
        Console.WriteLine($"I am a dog named {dog.name}");
        break;
    default:
        throw new InvalidOperationException($"Missing match for {nameof(animal3)}.");
}

// Typed union with generic types
Animal4<Cat, Dog> animal4 = c;
switch (animal4.Kind)
{
    case Animal4<Cat, Dog>.Tag.Type1:
        if (!animal4.TryGetType1(out var cat))
            goto default;
        Console.WriteLine($"I am a cat named {cat.name}");
        break;
    case Animal4<Cat, Dog>.Tag.Type2:
        if (!animal4.TryGetType2(out var dog))
            goto default;
        Console.WriteLine($"I am a dog named {dog.name}");
        break;
    default:
        throw new InvalidOperationException($"Missing match for {nameof(animal4)}.");
}

// generic type union
OneOf<Cat, Dog> animal = c;
switch (animal.Kind)
{
    case OneOf<Cat, Dog>.Tag.Type1:
        if (!animal.TryGetType1(out var cat))
            goto default;
        Console.WriteLine($"I am a cat named {cat.name}");
        break;
    case OneOf<Cat, Dog>.Tag.Type2:
        if (!animal.TryGetType2(out var dog))
            goto default;
        Console.WriteLine($"I am a dog named {dog.name}");
        break;
    default:
        throw new InvalidOperationException($"Missing match for {nameof(animal)}.");
}

Console.WriteLine(animal1);
Console.ReadLine();


/// <summary>
/// Animal as tagged union (w/o additional types)
/// </summary>
public readonly struct Animal0
{
    public enum Tag { Cat = 1, Dog = 2 };

    private record CatData(string name, bool mouser);
    private record DogData(string name, int toys);

    private readonly Tag _kind;
    private readonly CatData _valueCat;
    private readonly DogData _valueDog;

    private Animal0(CatData value)
    {
        _kind = Tag.Cat;
        _valueCat = value;
        _valueDog = default!;
    }

    private Animal0(DogData value)
    {
        _kind = Tag.Cat;
        _valueCat = default!;
        _valueDog = value;
    }

    public static Animal0 Cat(string name, bool mouser) => new Animal0(new CatData(name, mouser));
    public static Animal0 Dog(string name, int toys) => new Animal0(new DogData(name, toys));

    public Tag Kind => _kind;

    public bool TryGetCat(out (string name, bool mouser) value)
    {
        if (_kind == Tag.Cat)
        {
            value = (_valueCat.name, _valueCat.mouser);
            return true;
        }
        value = default!;
        return false;
    }

    public bool TryGetDog(out (string name, int toyes) value)
    {
        if (_kind == Tag.Dog)
        {
            value = (_valueDog.name, _valueDog.toys);
            return true;
        }
        value = default!;
        return false;
    }

    public override string ToString()
    {
        switch (_kind)
        {
            case Tag.Cat:
                return _valueCat.ToString();
            case Tag.Dog:
                return _valueDog.ToString();
            default:
                return "";
        }
    }
}

/// <summary>
/// Animal as tagged union (with nested declared types corresponding to tags)
/// </summary>
public readonly struct Animal1
{
    public enum Tag { Cat = 1, Dog = 2 };

    public record Cat(string name, bool mouser);
    public record Dog(string name, int toys);

    private readonly Tag _kind;
    private readonly Cat _valueCat;
    private readonly Dog _valueDog;

    private Animal1(Cat value) 
    {
        _kind = Tag.Cat;
        _valueCat = value;
        _valueDog = default!;
    }

    private Animal1(Dog value) 
    {
        _kind = Tag.Dog;
        _valueCat = default!;
        _valueDog = value;
    }

    public Tag Kind => _kind;

    public bool TryGetCat(out Cat value)
    {
        if (_kind == Tag.Cat)
        {
            value = _valueCat;
            return true;
        }
        value = default!;
        return false;
    }

    public bool TryGetDog(out Dog value)
    {
        if (_kind == Tag.Dog)
        {
            value = _valueDog;
            return true;
        }
        value = default!;
        return false;
    }

    public static implicit operator Animal1(Cat value) => new Animal1(value);
    public static implicit operator Animal1(Dog value) => new Animal1(value);

    public override string ToString() 
    {
        switch (_kind)
        {
            case Tag.Cat:
                return _valueCat.ToString();
            case Tag.Dog:
                return _valueDog.ToString();
            default:
                return "";
        }
    }
}


public record Cat(string name, bool mouser);
public record Dog(string name, int toys);

/// <summary>
/// Animal as tagged union with external declared types corresponding to tags 
/// </summary>
public readonly struct Animal2
{
    public enum Tag { Cat = 1, Dog = 2 };

    private readonly Tag _kind;
    private readonly Cat _valueCat;
    private readonly Dog _valueDog;

    private Animal2(Cat value)
    {
        _kind = Tag.Cat;
        _valueCat = value;
        _valueDog = default!;
    }

    private Animal2(Dog value)
    {
        _kind = Tag.Dog;
        _valueCat = default!;
        _valueDog = value;
    }

    public Tag Kind => _kind;

    public bool TryGetCat(out Cat cat)
    {
        if (_kind == Tag.Cat)
        {
            cat = _valueCat;
            return true;
        }
        cat = default!;
        return false;
    }

    public bool TryGetDog(out Dog dog)
    {
        if (_kind == Tag.Dog)
        {
            dog = _valueDog;
            return true;
        }
        dog = default!;
        return false;
    }

    public static implicit operator Animal2(Cat value) => new Animal2(value);
    public static implicit operator Animal2(Dog value) => new Animal2(value);

    public override string ToString()
    {
        switch (_kind)
        {
            case Tag.Cat:
                return _valueCat.ToString();
            case Tag.Dog:
                return _valueDog.ToString();
            default:
                return "";
        }
    }
}

/// <summary>
/// Animal as type union with specific types
/// </summary>
public readonly struct Animal3
{
    public enum Tag { Type1 = 1, Type2 = 2 };

    private readonly Tag _kind;
    private readonly Cat _valueType1;
    private readonly Dog _valueType2;

    private Animal3(Cat value)
    {
        _kind = Tag.Type1;
        _valueType1 = value;
        _valueType2 = default!;
    }

    private Animal3(Dog value)
    {
        _kind = Tag.Type2;
        _valueType1 = default!;
        _valueType2 = value;
    }

    public Tag Kind => _kind;

    public bool TryGetType1(out Cat value)
    {
        if (_kind == Tag.Type1)
        {
            value = _valueType1;
            return true;
        }
        value = default!;
        return false;
    }

    public bool TryGetType2(out Dog value)
    {
        if (_kind == Tag.Type2)
        {
            value = _valueType2;
            return true;
        }
        value = default!;
        return false;
    }

    public static implicit operator Animal3(Cat value) => new Animal3(value);
    public static implicit operator Animal3(Dog value) => new Animal3(value);

    public override string ToString()
    {
        switch (_kind)
        {
            case Tag.Type1:
                return _valueType1!.ToString()!;
            case Tag.Type2:
                return _valueType2!.ToString()!;
            default:
                return "";
        }
    }
}

/// <summary>
/// Animal as as type union with generic types
/// </summary>
public readonly struct Animal4<Type1, Type2>
{
    public enum Tag { Type1 = 1, Type2 = 2 };

    private readonly Tag _kind;
    private readonly Type1 _valueType1;
    private readonly Type2 _valueType2;

    private Animal4(Type1 value)
    {
        _kind = Tag.Type1;
        _valueType1 = value;
        _valueType2 = default!;
    }

    private Animal4(Type2 value)
    {
        _kind = Tag.Type2;
        _valueType1 = default!;
        _valueType2 = value;
    }

    public Tag Kind => _kind;

    public bool TryGetType1(out Type1 value)
    {
        if (_kind == Tag.Type1)
        {
            value = _valueType1;
            return true;
        }
        value = default!;
        return false;
    }

    public bool TryGetType2(out Type2 value)
    {
        if (_kind == Tag.Type2)
        {
            value = _valueType2;
            return true;
        }
        value = default!;
        return false;
    }

    public static implicit operator Animal4<Type1, Type2>(Type1 value) => new Animal4<Type1, Type2>(value);
    public static implicit operator Animal4<Type1, Type2>(Type2 value) => new Animal4<Type1, Type2>(value);

    public override string ToString()
    {
        switch (_kind)
        {
            case Tag.Type1:
                return _valueType1!.ToString()!;
            case Tag.Type2:
                return _valueType2!.ToString()!;
            default:
                return "";
        }
    }
}

/// <summary>
/// generic type union
/// </summary>
public readonly struct OneOf<Type1, Type2>
{
    public enum Tag { Type1 = 1, Type2 = 2 };

    private readonly Tag _kind;
    private readonly Type1 _valueType1;
    private readonly Type2 _valueType2;

    private OneOf(Type1 value)
    {
        _kind = Tag.Type1;
        _valueType1 = value;
        _valueType2 = default!;
    }

    private OneOf(Type2 value)
    {
        _kind = Tag.Type2;
        _valueType1 = default!;
        _valueType2 = value;
    }

    public Tag Kind => _kind;

    public bool TryGetType1(out Type1 value)
    {
        if (_kind == Tag.Type1)
        {
            value = _valueType1;
            return true;
        }
        value = default!;
        return false;
    }

    public bool TryGetType2(out Type2 value)
    {
        if (_kind == Tag.Type2)
        {
            value = _valueType2;
            return true;
        }
        value = default!;
        return false;
    }

    public static implicit operator OneOf<Type1, Type2>(Type1 value) => new OneOf<Type1, Type2>(value);
    public static implicit operator OneOf<Type1, Type2>(Type2 value) => new OneOf<Type1, Type2>(value);

    public override string ToString()
    {
        switch (_kind)
        {
            case Tag.Type1:
                return _valueType1!.ToString()!;
            case Tag.Type2:
                return _valueType2!.ToString()!;
            default:
                return "";
        }
    }
}