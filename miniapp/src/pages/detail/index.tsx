import { useEffect, useState } from 'react'
import { View, Text, Image, Button } from '@tarojs/components'
import Taro from '@tarojs/taro'
import { getWallpaper, WallpaperDetail } from '../../services/api'
import './index.scss'

const favoriteKey = 'favorite-wallpapers'
export default function Detail() {
  const [wallpaper, setWallpaper] = useState<WallpaperDetail | null>(null), [saved, setSaved] = useState(false)
  useEffect(() => { const id = Number(Taro.getCurrentInstance().router?.params.id); getWallpaper(id).then(x => { setWallpaper(x); setSaved((Taro.getStorageSync<number[]>(favoriteKey) || []).includes(x.id)) }).catch(() => Taro.showToast({ title: '壁纸不存在', icon: 'none' })) }, [])
  const toggle = () => { if (!wallpaper) return; const ids = Taro.getStorageSync<number[]>(favoriteKey) || []; const next = saved ? ids.filter(x => x !== wallpaper.id) : [...ids, wallpaper.id]; Taro.setStorageSync(favoriteKey, next); setSaved(!saved); Taro.showToast({ title: saved ? '已取消收藏' : '已收藏', icon: 'success' }) }
  const save = () => { if (!wallpaper?.originalUrl) return; Taro.showLoading({ title: '下载中' }); Taro.downloadFile({ url: wallpaper.originalUrl }).then(r => Taro.saveImageToPhotosAlbum({ filePath: r.tempFilePath })).then(() => Taro.showToast({ title: '已保存到相册', icon: 'success' })).catch(() => Taro.showModal({ title: '无法保存', content: '请确认图片域名已配置下载白名单，并在来源页面下载。', showCancel: false })).finally(() => Taro.hideLoading()) }
  if (!wallpaper) return <View className='loading'>加载中…</View>
  return <View className='page'><Image className='cover' mode='widthFix' src={wallpaper.originalUrl || wallpaper.thumbnailUrl} /><View className='info'><Text className='title'>{wallpaper.title}</Text><Text className='meta'>{wallpaper.category} · 图片来源：{wallpaper.sourceName}</Text><Text className='copyright'>图片版权归原作者及来源网站所有，仅供个人欣赏，请勿擅自商用。</Text><Button className='primary' onClick={save}>保存到相册</Button><Button className='secondary' onClick={toggle}>{saved ? '取消收藏' : '加入收藏'}</Button><Text className='origin' onClick={() => Taro.setClipboardData({ data: wallpaper.sourcePageUrl })}>复制来源页面链接</Text></View></View>
}
