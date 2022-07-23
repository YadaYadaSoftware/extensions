namespace System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Class)]
public sealed class EditModelAttribute : Attribute
{
    public Type EditModelType { get; }

    // See the attribute guidelines at 
    //  http://go.microsoft.com/fwlink/?LinkId=85236
    public EditModelAttribute(Type editModelType)
    {
        EditModelType = editModelType;
    }
}