namespace System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class HasDefaultSqlValueAttribute : Attribute
{
    // See the attribute guidelines at 
    //  http://go.microsoft.com/fwlink/?LinkId=85236
    public HasDefaultSqlValueAttribute(string value)
    {
        this.Value = value;
    }

    public string Value { get; set; }
}