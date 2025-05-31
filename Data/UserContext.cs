using Microsoft.EntityFrameworkCore;
using AuthC_.Models;

namespace AuthC_.Data;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
}