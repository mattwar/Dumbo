using System.Runtime.InteropServices;

namespace Dumbo;


// [TypeUnion(typeof(int), typeof(string)]
// public explicit extension IntOrString for object;

[TypeUnion(typeof(int), typeof(string))]
public struct IntOrString
{
    public readonly object Value;

    public IntOrString(object value)
    {
        this.Value = value;
    }
}

