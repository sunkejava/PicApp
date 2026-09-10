import { useEffect, useState } from 'react'
import { View, Text, Image } from '@tarojs/components'
import Taro from '@tarojs/taro'
import { getWallpaper, WallpaperDetail } from '../../services/api'
import './index.scss'

export default function Favorite() { const [items, setItems] = useState<WallpaperDetail[]>([]); useEffect(() => { Promise.all((Taro.getStorageSync<number[]>('favorite-wallpapers') || []).map(getWallpaper)).then(setItems).catch(() => {}) }, []); return <View className='page'><Text className='heading'>我的收藏</Text>{items.length === 0 ? <Text className='empty'>还没有收藏喜欢的壁纸</Text> : <View className='grid'>{items.map(x => <View className='card' key={x.id} onClick={() => Taro.navigateTo({ url: `/pages/detail/index?id=${x.id}` })}><Image src={x.thumbnailUrl} mode='aspectFill' /><Text>{x.title}</Text></View>)}</View>}</View> }
