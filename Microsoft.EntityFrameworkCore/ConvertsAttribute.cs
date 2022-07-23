namespace Microsoft.EntityFrameworkCore;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class ConvertsAttribute : Attribute
{
    public Type TypeToConvertTo { get; }

    // See the attribute guidelines at 
    //  http://go.microsoft.com/fwlink/?LinkId=85236
    public ConvertsAttribute(Type typeToConvertTo)
    {
        this.TypeToConvertTo = typeToConvertTo;
    }
}