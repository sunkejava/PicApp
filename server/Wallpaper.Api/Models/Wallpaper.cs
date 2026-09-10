namespace Wallpaper.Api.Models;

/// <summary>采集后缓存在本系统的壁纸元数据，不保存或再分发图片文件。</summary>
public sealed class Wallpaper
{
    public long Id { get; set; }
    public required string ExternalId { get; set; }
    public required string Title { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? OriginalUrl { get; set; }
    public required string SourceName { get; set; }
    public required string SourcePageUrl { get; set; }
    public string Category { get; set; } = "推荐";
    public int Width { get; set; }
    public int Height { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
