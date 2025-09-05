using Microsoft.EntityFrameworkCore;
using MessageApi.Models;

namespace MessageApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Detail> Details => Set<Detail>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    public DbSet<Item> Items => Set<Item>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuración de Client
        modelBuilder.Entity<Client>(entity =>
        {
            entity.Property(c => c.FirstName).IsRequired();
            entity.Property(c => c.LastName).IsRequired();
            entity.Property(c => c.Email).IsRequired();
            entity.Property(c => c.Nit).IsRequired();
            entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(c => c.UpdatedAt).IsRequired(false);
        });

        // Configuración de Invoice
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.Property(i => i.Serial).IsRequired();
            entity.Property(i => i.Number).IsRequired();
            entity.Property(i => i.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(i => i.UpdatedAt).IsRequired(false);
            
            entity.HasOne(i => i.Client)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
                
            entity.Property(p => p.UpdatedAt)
                .IsRequired(false);
        });

        // Configuración de Detail
        modelBuilder.Entity<Detail>(entity =>
        {
            entity.Property(d => d.Price).HasColumnType("decimal(18,2)");
            entity.Property(d => d.Total).HasColumnType("decimal(18,2)");
            entity.Property(d => d.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(d => d.UpdatedAt).IsRequired(false);
            
            entity.HasOne(d => d.Invoice)
                .WithMany(i => i.Details)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(d => d.Product)
                .WithMany(p => p.Details)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

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

