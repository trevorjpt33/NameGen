using Microsoft.EntityFrameworkCore;
using NameGen.Core.Models;

namespace NameGen.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<HumanName> HumanNames => Set<HumanName>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<HumanName>(entity =>
        {
            entity.ToTable("human_names");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Component).IsRequired();
            entity.Property(e => e.Gender).IsRequired();
            entity.Property(e => e.Origin).HasMaxLength(100);
            entity.HasIndex(e => e.Component);
            entity.HasIndex(e => e.Gender);
        });
    }
}