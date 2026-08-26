using Microsoft.EntityFrameworkCore;
using TestTask.Api.Domain;

namespace TestTask.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Lead> Leads => Set<Lead>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var lead = modelBuilder.Entity<Lead>();
        lead.ToTable("leads");
        lead.HasKey(x => x.Id);
        lead.Property(x => x.Name).HasMaxLength(120).IsRequired();
        lead.Property(x => x.Phone).HasMaxLength(30).IsRequired();
        lead.Property(x => x.Comment).HasMaxLength(2000);
        lead.Property(x => x.Source).HasMaxLength(80).IsRequired();
        lead.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        lead.HasIndex(x => x.Status);
        lead.HasIndex(x => x.CreatedAt);
    }
}
