# PicApp - 主题壁纸小程序

基于 **.NET 10 Minimal API + EF Core SQLite** 与 **Taro 4（React）** 的主题壁纸小程序。

## 功能

- 首页推荐、分类浏览、关键词搜索与瀑布流加载
- 壁纸详情、收藏、本地下载/保存相册入口、来源跳转
- 后台定时采集公开网页中可访问的壁纸元数据，并缓存到本地 SQLite
- 支持配置多个来源；默认提供哲风壁纸（haowallpaper.com）适配器

## 重要说明

图片版权归原作者及来源网站所有。项目只采集公开展示的标题、预览图、原始链接与来源链接；请在上线前取得来源方的书面授权，并在小程序隐私政策、内容页中展示来源和版权声明。请不要绕过登录、付费、反爬、访问频率限制或下载限制。

## 目录

```text
wallpaper-miniapp/
├── server/Wallpaper.Api       # .NET 10 后端
└── miniapp                    # Taro 微信小程序
```

## 启动

### 后端

```bash
cd server/Wallpaper.Api
dotnet restore
dotnet run
```

默认监听 `http://localhost:5188`，数据库创建在 `App_Data/wallpaper.db`。

### 小程序

```bash
cd miniapp
npm install
npm run dev:weapp
```

在微信开发者工具导入 `miniapp/dist`。把 `src/config.ts` 的 `apiBaseUrl` 改为已备案 HTTPS 域名；开发环境可用开发者工具的“不校验合法域名”。

## 采集配置

`server/Wallpaper.Api/appsettings.json` 中的 `WallpaperSource:SourcePageUrl` 指向公开列表页。后台每 6 小时采集一次，也可调用 `POST /api/admin/sources/sync` 立即同步。若来源方提供正式 API，应实现 `IWallpaperSource` 并替换网页采集方式。

生产环境请通过环境变量 `Admin__SyncApiKey` 配置同步密钥，并把小程序的正式 HTTPS 域名加入 `Cors:AllowedOrigins`；手动同步请求需携带 `X-Api-Key` 请求头。

## 自动发布

推送到 `main` 会执行构建并保留流水线产物。推送标签（例如 `git tag v0.1.0 && git push origin v0.1.0`）会自动创建 GitHub Release，附件包括：

- `wallpaper-api-win-x64.zip`：Windows x64 后端单文件运行包
- `wallpaper-api-linux-x64.zip`：Linux x64 后端单文件运行包
- `theme-wallpaper-weapp.zip`：微信小程序构建产物，导入微信开发者工具即可上传
