using Microsoft.EntityFrameworkCore;
using MessageApi.Models;

namespace MessageApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    public DbSet<Item> Items => Set<Item>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Eliminadas configuraciones de Client/Invoice/Detail/Product (no usadas)

        // Configuración de Person
        modelBuilder.Entity<Person>(entity =>
        {
            entity.Property(p => p.FirstName).IsRequired();
            entity.Property(p => p.LastName).IsRequired();
            entity.Property(p => p.Email).IsRequired();
            entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(p => p.UpdatedAt).IsRequired(false);
        });

        // Configuración de Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(o => o.Number).IsRequired();
            entity.Property(o => o.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(o => o.UpdatedAt).IsRequired(false);
            
            entity.HasOne(o => o.Person)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de Item
        modelBuilder.Entity<Item>(entity =>
        {
            entity.Property(i => i.Name).IsRequired();
            entity.Property(i => i.Price).HasColumnType("decimal(18,2)");
            entity.Property(i => i.Description).HasMaxLength(500);
            entity.Property(i => i.Stock).HasDefaultValue(0);
            entity.Property(i => i.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(i => i.UpdatedAt).IsRequired(false);
        });

        // Configuración de OrderDetail
        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.Property(od => od.Price).HasColumnType("decimal(18,2)");
            entity.Property(od => od.Total).HasColumnType("decimal(18,2)");
            entity.Property(od => od.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(od => od.UpdatedAt).IsRequired(false);
            
            entity.HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(od => od.Item)
                .WithMany(i => i.OrderDetails)
                .HasForeignKey(od => od.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de Message
        modelBuilder.Entity<Message>(entity =>
        {
            entity.Property(m => m.MessageText)
                .IsRequired()
                .HasDefaultValue("Mensaje predeterminado")
                .HasMaxLength(1000);
            entity.Property(m => m.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(m => m.UpdatedAt).IsRequired(false);
        });
    }
    
}

