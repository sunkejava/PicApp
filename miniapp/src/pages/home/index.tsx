import { useEffect, useState } from 'react'
import { View, Text, Image, Input, ScrollView } from '@tarojs/components'
import Taro from '@tarojs/taro'
import { getCategories, getWallpapers, Wallpaper } from '../../services/api'
import './index.scss'

export default function Home() {
  const [items, setItems] = useState<Wallpaper[]>([]), [categories, setCategories] = useState(['全部'])
  const [category, setCategory] = useState('全部'), [keyword, setKeyword] = useState(''), [page, setPage] = useState(1), [loading, setLoading] = useState(false)
  const load = async (targetPage = 1, reset = false) => { if (loading) return; setLoading(true); try { const r = await getWallpapers(targetPage, category, keyword); setItems(reset ? r.items : v => [...v, ...r.items]); setPage(targetPage) } catch { Taro.showToast({ title: '加载失败，请检查服务地址', icon: 'none' }) } finally { setLoading(false) } }
  useEffect(() => { getCategories().then(v => setCategories(['全部', ...v])).catch(() => {}); load(1, true) }, [category])
  const search = () => load(1, true)
  return <View className='page'>
    <View className='hero'><Text className='eyebrow'>WALLPAPER STUDIO</Text><Text className='headline'>找到今天的好心情</Text><View className='search'><Input value={keyword} onInput={e => setKeyword(e.detail.value)} confirmType='search' onConfirm={search} placeholder='搜索动漫、风景、人物…' /><Text onClick={search}>搜索</Text></View></View>
    <ScrollView scrollX className='tabs'><View className='tabs-inner'>{categories.map(x => <Text key={x} className={x === category ? 'active' : ''} onClick={() => setCategory(x)}>{x}</Text>)}</View></ScrollView>
    <View className='grid'>{items.map(item => <View className='card' key={item.id} onClick={() => Taro.navigateTo({ url: `/pages/detail/index?id=${item.id}` })}><Image mode='aspectFill' src={item.thumbnailUrl} /><View><Text>{item.title}</Text><Text className='source'>{item.sourceName}</Text></View></View>)}</View>
    <View className='more' onClick={() => load(page + 1)}>{loading ? '加载中…' : '加载更多'}</View>
  </View>
}
