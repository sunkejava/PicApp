using Microsoft.EntityFrameworkCore;
using PicApp.Api.Contracts;
using PicApp.Api.Data;
using PicApp.Api.Sources;
using PicApp.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<WallpaperDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddHttpClient<IWallpaperSource, ZheFengWallpaperSource>(client => client.Timeout = TimeSpan.FromSeconds(20));
builder.Services.AddSingleton<WallpaperSyncService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<WallpaperSyncService>());
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader();
}));
builder.Services.AddOpenApi();

var app = builder.Build();
using (var scope = app.Services.CreateScope()) await scope.ServiceProvider.GetRequiredService<WallpaperDbContext>().Database.EnsureCreatedAsync();
app.UseCors();
app.MapOpenApi();

app.MapGet("/api/wallpapers", async (WallpaperDbContext db, string? category, string? keyword, int page = 1, int pageSize = 20, CancellationToken ct = default) =>
{
    page = Math.Max(1, page); pageSize = Math.Clamp(pageSize, 1, 50);
    var query = db.Wallpapers.AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(category) && category != "全部") query = query.Where(x => x.Category == category);
    if (!string.IsNullOrWhiteSpace(keyword)) query = query.Where(x => x.Title.Contains(keyword));
    var total = await query.CountAsync(ct);
    var items = await query.OrderByDescending(x => x.UpdatedAtUtc).Skip((page - 1) * pageSize).Take(pageSize)
        .Select(x => new WallpaperListItem(x.Id, x.Title, x.ThumbnailUrl, x.Category, x.Width, x.Height, x.SourceName)).ToListAsync(ct);
    return Results.Ok(new PagedResult<WallpaperListItem>(items, page, pageSize, total));
});

app.MapGet("/api/wallpapers/{id:long}", async (long id, WallpaperDbContext db, CancellationToken ct) =>
{
    var item = await db.Wallpapers.AsNoTracking().Where(x => x.Id == id)
        .Select(x => new WallpaperDetail(x.Id, x.Title, x.ThumbnailUrl, x.OriginalUrl, x.Category, x.Width, x.Height, x.SourceName, x.SourcePageUrl)).SingleOrDefaultAsync(ct);
    return item is null ? Results.NotFound() : Results.Ok(item);
});
app.MapGet("/api/categories", async (WallpaperDbContext db, CancellationToken ct) => Results.Ok(await db.Wallpapers.AsNoTracking().Select(x => x.Category).Distinct().OrderBy(x => x).ToListAsync(ct)));
app.MapPost("/api/admin/sources/sync", async (HttpRequest request, IConfiguration config, WallpaperSyncService sync, CancellationToken ct) =>
{
    var expectedKey = config["Admin:SyncApiKey"];
    if (string.IsNullOrWhiteSpace(expectedKey) || expectedKey.StartsWith("请通过")) return Results.Problem("未配置同步接口密钥", statusCode: 503);
    if (!request.Headers.TryGetValue("X-Api-Key", out var key) || !string.Equals(key, expectedKey, StringComparison.Ordinal)) return Results.Unauthorized();
    return Results.Ok(new { count = await sync.SyncAsync(ct) });
});
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.Run();
