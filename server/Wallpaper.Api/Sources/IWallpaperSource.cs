using Wallpaper.Api.Models;

namespace Wallpaper.Api.Sources;

public interface IWallpaperSource
{
    string Name { get; }
    Task<IReadOnlyList<Wallpaper>> FetchAsync(CancellationToken cancellationToken);
}
