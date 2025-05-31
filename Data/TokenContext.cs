using Microsoft.EntityFrameworkCore;
using AuthC_.Models;

namespace AuthC_.Data;

public class TokenContext : DbContext
{
    public TokenContext(DbContextOptions<TokenContext> options)
        : base(options)
    {
    }

    public DbSet<Token> Tokens { get; set; } = null!;
}