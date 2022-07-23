namespace Microsoft.EntityFrameworkCore;

public class ContextBaseOptions<T> : DbContextOptions<T> where T : DbContext
{
    public ContextBaseOptions()
    {
    }
}