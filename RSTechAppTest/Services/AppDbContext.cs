using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RSTechAppTest.Models;
using System;
using System.Reflection.Emit;

namespace RSTechAppTest.Services;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Npgsql требует, чтобы все таймстампы были DateTimeKind.Utc,
        // но при обновлении записей они сбрасываются до Unspecified.
        // Здесь они ставятся обратно
        var utcConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.Property(e => e.CreatedAt)
                  .HasConversion(utcConverter);
        });
    }
}
