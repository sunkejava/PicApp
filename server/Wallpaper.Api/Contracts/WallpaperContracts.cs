namespace Wallpaper.Api.Contracts;

public sealed record WallpaperListItem(long Id, string Title, string? ThumbnailUrl, string Category, int Width, int Height, string SourceName);
public sealed record WallpaperDetail(long Id, string Title, string? ThumbnailUrl, string? OriginalUrl, string Category, int Width, int Height, string SourceName, string SourcePageUrl);
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int Total);
