# Discriminated Union Implementation Stratgies

This document describes the different stategies used to implement discriminated unions
in this project.


## Tagged Union Strategies

A tagged union is a type that may exists in one of many named states (**tags**)
and for each state may contain a set of associated values that differ depending on that state. 
When none of the states has any associated values, it acts similar to a C# enum.

The implementation strategies differ on:
>>- **Footprint** - the amount of memory the union consumes on the stack or within another type.
>>- **Allocation** - the amount of allocation required when the union is constructed.
>>- **Tearing** - whether tearing is possible when assigning unions to locations that are visible to multiple threads.
>>- **Default** - the union's state when assigned from default.

- ### Fat  

    This tagged union is implemented as a struct with an enum field for tag state 
    and separate strongly typed fields for each possible tag state value.  

    It has the largest footprint.  
    It never allocates.  
    It is possible for tearing to occur.  
    It is undefined when assigned from default.

    ```
    struct Union { 
        Tag _tag;
        TA _tag1_value1;
        TB _tag1_value2;
        TA _tag2_value1;
        TC _tag2_value2;
        TC _tag3_value3;
    
        enum Tag { Tag1 = 1, Tag2, ... }          

        public static Union Tag1(TA v1, TB v2) { ... };
        public static Union Tag2(TA v2, TC v2, TC v3) { ... };

        public IsTag1 => _tag == Tag.Tag1;
        public IsTag2 => _tag == Tag.Tag2;

        public bool TryGetTag1(out t1v1 v1, out t1v2 v2) { ... }
        public bool TryGetTag2(out t2v1 v1, out t2v2 v2, out t2v3 v3) { ... }
    }
    ```

- ### Shared  

    This tagged union is implemented as a struct with an enum field for tag state
    and enough strongly type fields to account for all tag values
    with some values across different tags sharing the same field if they share the same type.  

    It has a footprint less than or equal to **Fat**.  
    It never allocates.  
    It is possible for tearing to occur.  
    It is undefined when assigned from default.

    ```
    struct Union { 
        Tag _tag;
        TA _valueA1;
        TB _valueB1;
        TC _valueC1;
        TC _valueC2;

        enum Tag { Tag1 = 1, Tag2, ... }          

        public static Union Tag1(TA v1, TB v2) { ... };
        public static Union Tag2(TA v1, TC v2, TD v3) { ... };

        public IsTag1 => _tag == Tag.Tag1;
        public IsTag2 => _tag == Tag.Tag2;

        public bool TryGetTag1(out TA v1, out TB v2) { ... }
        public bool TryGetTag2(out TA v1, out TC v2, out TC v3) { ... }
    }
    ```

- ### Overlapped  

    This tagged union is implemented as a struct with an enum field for tag state
    and shared fields containing overlapping references and overlapping structs,
    and separate fields for any non-overlappable values, shared if possible across tag states.  

    It has a footprint less than or equal to **Shared**.
    It never allocates.  
    It is possible for tearing to occur.  
    It is undefined when assigned from default.

    ```
    struct Union { 
        Tag _tag;
        OverlappedRefs _refs;
        OverlappedVals _vals;
        NonOverlappableType1 _type1;
        NonOverlappableType2 _type2;

        enum Tag { Tag1 = 1, Tag2, ... }          
        struct OverlappedRefs { 
            OverlappedTag1Refs tag1; OverlappedTag2Refs tag2, ... }
        struct OverlappedVals { 
            OverlappedTag1Vals tag1; OverlappedTag2Vals tag2, ... }
        struct OverlappedTag1Refs { ... }
        struct OverlappedTag1Vals { ... }
        struct OverlappedTag2Refs { ... }
        struct OverlappedTag2Vals { ... }

        public static Union Tag1(TA v1, TB v2) { ... };
        public static Union Tag2(TA v2, TC v2, TC v3) { ... };

        public IsTag1 => _tag == Tag.Tag1;
        public IsTag2 => _tag == Tag.Tag2;

        public bool TryGetTag1(out TA v1, out TB v2) { ... }
        public bool TryGetTag2(out TA v1, out TC v2, out TC v3) { ... }
    }    
    ```

- ### Boxed  

    This tagged union is implemented as a struct with an enum field for tag state
    and a number of object fields equal to the maximum number of values per tag state.  

    It has a footprint relative to the maximum number of values per tag state.  
    It sometimes allocates due to boxing.  
    It is possible for tearing to occur.  
    It is undefined when assigned from default.

    ```
    struct Union { 
        Tag _tag;
        object _value1;
        object _value2;
        object _value3;

        enum Tag { Tag1 = 1, Tag2, ... }          

        public static Union Tag1(TA v1, TB v2) { ... };
        public static Union Tag2(TA v2, TC v2, TC v3) { ... };

        public IsTag1 => _tag == Tag.Tag1;
        public IsTag2 => _tag == Tag.Tag2;

        public bool TryGetTag1(out TA v1, out TB v2) { ... }
        public bool TryGetTag2(out TA v1, out TC v2, out TC v3) { ... }
    }
    ```

- ### Hybrid  

    This tagged union is implemented as a struct with an enum field for tag state
    and a number of hybrid fields equal to the maximum number of values per tag state
    that can either contain a primitive without boxing or an object reference.  

    It has a footprint relative to the maximum number of values per tag state, but more than **Boxed**.  
    It sometimes allocates due to boxing value types that are not primitives.  
    It is possible for tearing to occur.  
    It is undefined when assigned from default.

    ```
    struct Union { 
        Tag _tag;
        hybrid _value1;
        hybrid _value2;
        hybrid _value3;

        enum Tag { Tag1 = 1, Tag2, ... }          

        public static Union Tag1(TA v1, TB v2) { ... };
        public static Union Tag2(TA v2, TC v2, TC v3) { ... };

        public IsTag1 => _tag == Tag.Tag1;
        public IsTag2 => _tag == Tag.Tag2;

        public bool TryGetTag1(out TA v1, out TB v2) { ... }
        public bool TryGetTag2(out TA v1, out TC v2, out TC v3) { ... }
    }
    ```

- ### Hidden (Heirarchy)

    This tagged union is implemented as a record
    with private nested derived records for each tag state and associated values.  

    It has the smallest footprint (8 bytes).  
    It always allocates.  
    It is not possible for tearing to occur.  
    It is null when assigned from default.

    ```
    abstract record Union {     
        private record Tag1Data(TA v1, TB v2) : Union;
        private record Tag2Data(TA v1, TC v2, TC v3) : Union;

        public static Union Tag1(TA v1, TB v2) => new Tag1Data(v1, v2);
        public static Union Tag2(TA v1, TC v2, TC v3) new Tag2Data(v1, v2, v3);

        public bool IsTag1 => this is Tag1Data;
        public bool IsTag2 => this is Tag2Data;

        public bool TryGetTag1(out TA v1, out TB v2) { ... }
        public bool TryGetTag2(out TA v1, out TC v2, out TC v3) { ... }
    }
    ```

- ### Heirarchy

    This tagged union is implemented as a record
    with public nested derived records for each tag state and associated values.  

    It has the smallest footprint (8 bytes).  
    It always allocates.  
    It is not possible for tearing to occur.
    It is null when assigned from default.

    ```
    abstract record Union {     
        public record Tag1(TA v1, TB v2) : Union;
        public record Tag2(TA v1, TC v2, TC v3) : Union;
    }
    ```


## Type Union Strategies

A type union is a type that may contain (or be) a value of one of a set of possible member types.
It differs from a tagged union in that there is no named state other than the type of the value itself.

A type union is often used as a constraint to limit the values of a variable or parameter
to one of a set of types.

The implementation strategies differ on:  
>>- **Footprint** - the amount of memory the union consumes on the stack or within another type.  
>>- **Allocation** - the amount of allocation required when the union is constructed.  
>>- **Tearing** - whether tearing is possible when assigning unions to locations that are visible to multiple threads.
>>- **Default** - the union's state when assigned from default.
>>- **Untyped Matching** - if member type matching is possible when the union value is typed as object or generic.

- ### Fat

    This type union is implemented as a struct with a field for each possible member type of the union.  
    and an enum field for determining which type and field is in use.
  
    It has the largest footprint, since it must include space for all the possible member types.  
    It never allocates.  
    It is possible for tearing to occur.  
    It is undefined when assigned from default.
    It cannot be matched from object or generic.  

    ```
    struct Union {
        Tag _tag;
        T1 _type1_value;
        T2 _type2_value;
        T3 _type3_value;

        enum Tag { Type1 = 1, Type2, Type3 }

        public static Union Create(T1 value) => ...;
        public static Union Create(T2 value) => ...;
        public static Union Create(T3 value) => ...;

        public IsType<T>() { ... }
        public bool TryGet<T>(out T value) { ... };
    }    
    ```

- ### Boxed

    This type union is implemented as a struct with a single object field.

    It has the smallest footprint (8 bytes).  
    It may allocate due to boxing.  
    It is not possible for tearing to occur.  
    It is undefined when assigned from default.  
    It cannot be matched from object or generic.  

    ```
    struct Union {
        object _value;

        public static Union Create(T1 value) => ...;
        public static Union Create(T2 value) => ...;
        public static Union Create(T3 value) => ...;

        public bool IsType<T>() => _value is T;
        public bool TryGet<T>(out T value) { ... }
    }
    ```

- ### Hybrid

    This type union is implemented as a struct with a single hybrid field
    that can either contain a primitive without boxing or an object reference.
    
    It has a footprint larger than **Boxed** (16 bytes).  
    It may allocate due to boxing of non primitive value types.  
    It is possible for tearing to occur.  
    It is undefined when assigned from default.
    It cannot be matched from object or generic.  

    ```
    struct Union {
        hybrid _value;

        public static Union Create(T1 value) => ...;
        public static Union Create(T2 value) => ...;
        public static Union Create(T3 value) => ...;

        public bool IsType<T>() => _value is T;
        public bool TryGet<T>(out T value) { ... }
    }
    ```

- ### Overlapped

    This type union is implemented as a struct with a layout similar to tagged union overlapped style.  
    It only works for member types that are record structs.  

    It has a footprint that is less than or equal to **Fat**.  
    It never allocates.  
    It is possible for tearing to occur.  
    It is undefined when assigned from default.
    It cannot be matched from object or generic.  

    ```
    struct Union { 
        Kind _kind;
        OverlappedRefs _refs;
        OverlappedVals _vals;
        NonOverlappableType1 _type1;
        NonOverlappableType2 _type2;

        enum Kind { Type1 = 1, Type2, ... }          

        struct OverlappedRefs { 
            OverlappedTag1Refs tag1; OverlappedTag2Refs tag2, ... }
        struct OverlappedVals { 
            OverlappedTag1Vals tag1; OverlappedTag2Vals tag2, ... }
        struct OverlappedTag1Refs { ... }
        struct OverlappedTag1Vals { ... }
        struct OverlappedTag2Refs { ... }
        struct OverlappedTag2Vals { ... }

        public static Union Type1(Type1 value) { ... };
        public static Union Type2(Type2 value) { ... };

        public bool IsType<T1>() { ... };
        public bool TryGet<T>(out T value) { ... };
    }    
    ```

- ### Existing

    This type union is implemented with an existing type like OneOf<T1, T2>.  

    It depends on the implementation of the existing type.  
    It cannot be optimized for specific member types.  
    It may be limited to the number of member types that can be included in the union.  
    It is undefined when assigned from default.
    It cannot be matched from object or generic.  

    ```
        global using Union = OneOf<Type1, Type2>;
    ```
    
- ### Erasure

    This type union is implemented as an object reference via erasure.  
    It must use means other than the type system to encode the member types of the union.  

    It has the smallest footprint (8 bytes).  
    It may allocate due to boxing.  
    It is not possible for tearing to occur.  
    It is null when assigned from default.  
    It can be matched from object or generic.  

    ```
        global using Union = object;
    ```
