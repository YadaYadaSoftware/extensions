using Microsoft.EntityFrameworkCore;

namespace YadaYada.Data.Library;

public class ContextBaseOptions<T> : DbContextOptions<T> where T : DbContext
{
    public ContextBaseOptions()
    {
    }
}