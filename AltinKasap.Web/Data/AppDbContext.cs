using AltinKasap.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AltinKasap.Web.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductTag> ProductTags => Set<ProductTag>();
    public DbSet<ProductTagMapping> ProductTagMappings => Set<ProductTagMapping>();
    public DbSet<QrCode> QrCodes => Set<QrCode>();
    public DbSet<QrScanLog> QrScanLogs => Set<QrScanLog>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<DailySpecial> DailySpecials => Set<DailySpecial>();
    public DbSet<PriceHistory> PriceHistories => Set<PriceHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Restaurant>(b =>
        {
            b.HasIndex(r => r.Slug).IsUnique();
        });

        modelBuilder.Entity<Category>(b =>
        {
            b.HasIndex(c => c.Slug).IsUnique();
        });

        modelBuilder.Entity<Product>(b =>
        {
            b.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(p => new { p.CategoryId, p.IsActive, p.SortOrder });
        });

        modelBuilder.Entity<ProductTagMapping>(b =>
        {
            b.HasKey(m => new { m.ProductId, m.ProductTagId });

            b.HasOne(m => m.Product)
                .WithMany(p => p.TagMappings)
                .HasForeignKey(m => m.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(m => m.ProductTag)
                .WithMany(t => t.ProductMappings)
                .HasForeignKey(m => m.ProductTagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<QrScanLog>(b =>
        {
            b.HasOne(l => l.QrCode)
                .WithMany(q => q.ScanLogs)
                .HasForeignKey(l => l.QrCodeId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasIndex(l => new { l.ScannedAt, l.QrCodeId });
        });

        modelBuilder.Entity<PriceHistory>(b =>
        {
            b.HasOne(h => h.Product)
                .WithMany(p => p.PriceHistories)
                .HasForeignKey(h => h.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DailySpecial>(b =>
        {
            b.HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
