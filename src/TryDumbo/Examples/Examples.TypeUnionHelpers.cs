using Dumbo;
using System.Text;
using Dumbo.TypeUnions.Existing.Fat;

public partial class Examples
{
    /// <summary>
    /// Example of using <see cref="TypeUnion"/> helper methods to create/access union values when
    /// union is boxed or weakly typed.
    /// </summary>
    public static void TypeUnionHelpers()
    {
        // gets the possible types of the union
        TestUnion<OneOf<string, int, float>>(10);
        TestUnion<OneOf<string, int, float>>("ten");
        TestUnion<OneOf<string, int, float>>(10.0f);
    }

    private static void TestUnion<TUnion>(object value)
    {
        var kinds = TypeUnion.GetTypes(typeof(TUnion));
        var valid = TypeUnion.IsValidValue(typeof(TUnion), value);
        var union = TypeUnion.ConvertTo<TUnion>(value);
        var gotten = TypeUnion.GetValue(union);
    }
}
