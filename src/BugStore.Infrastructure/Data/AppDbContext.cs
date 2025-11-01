using BugStore.Domain.Entities;
using BugStore.Domain.Reports;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderLine> OrderLines { get; set; } = null!;

    public DbSet<CustomerOrderSummary> CustomerOrderSummary { get; set; } = null!;
    public DbSet<RevenueByPeriod> RevenueByPeriod { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<CustomerOrderSummary>()
            .HasNoKey()
            .ToView("vwCustomerOrderSummary");

        modelBuilder.Entity<RevenueByPeriod>()
            .HasNoKey();
    }
}