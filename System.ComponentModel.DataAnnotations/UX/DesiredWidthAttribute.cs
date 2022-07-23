namespace System.ComponentModel.DataAnnotations.UX
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DesiredWidthAttribute : Attribute
    {
        public int Width { get; }

        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        public DesiredWidthAttribute(int width)
        {
            Width = width;
        }
    }
}
