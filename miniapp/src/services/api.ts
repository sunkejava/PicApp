import Taro from '@tarojs/taro'
import { apiBaseUrl } from '../config'

export interface Wallpaper { id: number; title: string; thumbnailUrl?: string; category: string; width: number; height: number; sourceName: string }
export interface WallpaperDetail extends Wallpaper { originalUrl?: string; sourcePageUrl: string }

async function request<T>(path: string): Promise<T> {
  const response = await Taro.request<T>({ url: `${apiBaseUrl}${path}`, method: 'GET' })
  if (response.statusCode >= 400) throw new Error('服务暂不可用')
  return response.data
}
export const getWallpapers = (page: number, category = '全部', keyword = '') => request<{ items: Wallpaper[]; total: number }>(`/api/wallpapers?page=${page}&pageSize=20&category=${encodeURIComponent(category)}&keyword=${encodeURIComponent(keyword)}`)
export const getWallpaper = (id: number) => request<WallpaperDetail>(`/api/wallpapers/${id}`)
export const getCategories = () => request<string[]>('/api/categories')
