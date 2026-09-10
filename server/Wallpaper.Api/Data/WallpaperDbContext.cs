using Microsoft.EntityFrameworkCore;
using PicApp.Api.Models;

namespace PicApp.Api.Data;

public sealed class WallpaperDbContext(DbContextOptions<WallpaperDbContext> options) : DbContext(options)
{
    public DbSet<Wallpaper> Wallpapers => Set<Wallpaper>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Wallpaper>(entity =>
        {
            entity.HasIndex(x => x.ExternalId).IsUnique();
            entity.HasIndex(x => new { x.Category, x.UpdatedAtUtc });
            entity.Property(x => x.Title).HasMaxLength(300);
            entity.Property(x => x.SourceName).HasMaxLength(80);
        });
    }
}
