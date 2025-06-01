using Microsoft.EntityFrameworkCore;
using AuthC_.Models;

namespace AuthC_.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Token> Tokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure User entity properties
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasColumnType("varchar(64)");

        modelBuilder.Entity<User>()
            .Property(u => u.FirstName)
            .HasColumnType("varchar(32)");

        modelBuilder.Entity<User>()
            .Property(u => u.LastName)
            .HasColumnType("varchar(32)");

        modelBuilder.Entity<User>()
            .Property(u => u.Hash)
            .HasColumnType("varchar(255)");

        modelBuilder.Entity<User>()
            .Property(u => u.CreatedAt)
            .HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<User>()
            .Property(u => u.UpdatedAt)
            .HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

        // Configure Token entity properties
        modelBuilder.Entity<Token>()
            .Property(t => t.RefreshToken)
            .HasColumnType("varchar(250)"); 

        modelBuilder.Entity<Token>()
            .Property(t => t.ExpiresIn)
            .HasColumnType("varchar(64)");
        
        modelBuilder.Entity<Token>()
            .Property(t => t.CreatedAt)
            .HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Token>()    
            .Property(t => t.UpdatedAt)
            .HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

        // Configure indexes and relationships
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Token>()
            .HasIndex(t => t.RefreshToken)
            .IsUnique();

        modelBuilder.Entity<Token>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}