namespace System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
public sealed class TabDelimitedAttribute : Attribute
{
    // See the attribute guidelines at 
    //  http://go.microsoft.com/fwlink/?LinkId=85236
    public TabDelimitedAttribute(string columnName, string propertyName = null, string lookupKey = null)
    {
        if (string.IsNullOrEmpty(propertyName)) propertyName = columnName.Replace(" ", string.Empty);
        this.PropertyName = propertyName;
        this.ColumnName = columnName;
        this.LookupKey = lookupKey;
    }

    public string LookupKey { get; }
    public string ColumnName { get; }

    public string PropertyName { get; set; }
}