namespace System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class DynamicLinqSelectAttribute : Attribute
{
    // See the attribute guidelines at 
    //  http://go.microsoft.com/fwlink/?LinkId=85236
    public DynamicLinqSelectAttribute(params string[] path)
    {
        this.Path = string.Join('.', path);
    }

    public string Path { get; }
}