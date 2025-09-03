using Microsoft.EntityFrameworkCore;
using MessageApi.Models;

namespace MessageApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Messages");
            entity.HasKey(m => m.Id);

            entity.Property(m => m.MessageText)
                  .HasColumnName("Text"); 

            entity.Property(m => m.CreatedAt)
                  .HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
