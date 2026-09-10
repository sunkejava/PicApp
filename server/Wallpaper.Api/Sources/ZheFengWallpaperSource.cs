using System.Security.Cryptography;
using System.Text;
using AngleSharp;
using PicApp.Api.Models;

namespace PicApp.Api.Sources;

/// <summary>
/// 只读取来源公开 HTML 中的链接和图片元数据。页面结构变化时可单独替换该适配器。
/// 不模拟登录、不破解限制、不下载或转存原图。
/// </summary>
public sealed class ZheFengWallpaperSource(HttpClient httpClient, IConfiguration configuration, ILogger<ZheFengWallpaperSource> logger) : IWallpaperSource
{
    public string Name => configuration["WallpaperSource:Name"] ?? "哲风壁纸";

    public async Task<IReadOnlyList<Wallpaper>> FetchAsync(CancellationToken cancellationToken)
    {
        var pageUrl = configuration["WallpaperSource:SourcePageUrl"] ?? "https://haowallpaper.com/";
        using var request = new HttpRequestMessage(HttpMethod.Get, pageUrl);
        request.Headers.UserAgent.ParseAdd("ThemeWallpaperMiniApp/1.0 (metadata collector; contact: admin@example.com)");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync(cancellationToken);
        var document = await BrowsingContext.New(Configuration.Default).OpenAsync(req => req.Content(html).Address(pageUrl), cancellationToken);
        var uri = new Uri(pageUrl);
        var result = new List<Wallpaper>();

        foreach (var image in document.Images)
        {
            var imageUrl = image.GetAttribute("data-src") ?? image.GetAttribute("src");
            if (string.IsNullOrWhiteSpace(imageUrl) || imageUrl.StartsWith("data:", StringComparison.OrdinalIgnoreCase)) continue;
            var normalizedImageUrl = new Uri(uri, imageUrl).ToString();
            var link = image.Closest("a")?.GetAttribute("href");
            var sourcePageUrl = string.IsNullOrWhiteSpace(link) ? pageUrl : new Uri(uri, link).ToString();
            var title = image.GetAttribute("alt") ?? image.GetAttribute("title") ?? "哲风壁纸";
            var externalId = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(normalizedImageUrl)))[..32];
            result.Add(new Wallpaper
            {
                ExternalId = externalId, Title = title.Trim(), ThumbnailUrl = normalizedImageUrl,
                // 仅在来源公开页直接给出图片地址时使用；详情页链接始终保留给用户溯源。
                OriginalUrl = normalizedImageUrl, SourceName = Name, SourcePageUrl = sourcePageUrl,
                Category = InferCategory(title), Width = int.TryParse(image.GetAttribute("width"), out var w) ? w : 0,
                Height = int.TryParse(image.GetAttribute("height"), out var h) ? h : 0
            });
        }
        logger.LogInformation("从 {Source} 解析到 {Count} 条公开壁纸元数据", Name, result.Count);
        return result.DistinctBy(x => x.ExternalId).Take(200).ToArray();
    }

    private static string InferCategory(string title) => title.Contains("动漫") ? "动漫" : title.Contains("美女") ? "人物" : title.Contains("风景") ? "风景" : "推荐";
}
