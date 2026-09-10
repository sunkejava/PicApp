using Microsoft.EntityFrameworkCore;
using Wallpaper.Api.Data;
using Wallpaper.Api.Sources;

namespace Wallpaper.Api.Services;

public sealed class WallpaperSyncService(IServiceScopeFactory scopeFactory, IWallpaperSource source, ILogger<WallpaperSyncService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await SyncAsync(stoppingToken);
        using var timer = new PeriodicTimer(TimeSpan.FromHours(6));
        while (await timer.WaitForNextTickAsync(stoppingToken)) await SyncAsync(stoppingToken);
    }

    public async Task<int> SyncAsync(CancellationToken cancellationToken)
    {
        try
        {
            var collected = await source.FetchAsync(cancellationToken);
            await using var scope = scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<WallpaperDbContext>();
            var ids = collected.Select(x => x.ExternalId).ToArray();
            var existing = await db.Wallpapers.Where(x => ids.Contains(x.ExternalId)).ToDictionaryAsync(x => x.ExternalId, cancellationToken);
            foreach (var item in collected)
            {
                if (existing.TryGetValue(item.ExternalId, out var saved))
                {
                    saved.Title = item.Title; saved.ThumbnailUrl = item.ThumbnailUrl; saved.OriginalUrl = item.OriginalUrl;
                    saved.SourcePageUrl = item.SourcePageUrl; saved.Category = item.Category; saved.UpdatedAtUtc = DateTime.UtcNow;
                }
                else db.Wallpapers.Add(item);
            }
            await db.SaveChangesAsync(cancellationToken);
            return collected.Count;
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogError(ex, "壁纸来源同步失败");
            return 0;
        }
    }
}
