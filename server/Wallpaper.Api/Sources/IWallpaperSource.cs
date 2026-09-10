using PicApp.Api.Models;

namespace PicApp.Api.Sources;

public interface IWallpaperSource
{
    string Name { get; }
    Task<IReadOnlyList<Wallpaper>> FetchAsync(CancellationToken cancellationToken);
}
