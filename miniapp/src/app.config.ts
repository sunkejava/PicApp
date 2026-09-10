export default defineAppConfig({
  pages: ['pages/home/index', 'pages/detail/index', 'pages/favorite/index'],
  window: { navigationBarTitleText: '主题壁纸', navigationBarBackgroundColor: '#111827', navigationBarTextStyle: 'white', backgroundColor: '#f7f8fc' },
  tabBar: { color: '#8891a8', selectedColor: '#6f5cff', backgroundColor: '#ffffff', list: [
    { pagePath: 'pages/home/index', text: '发现' }, { pagePath: 'pages/favorite/index', text: '收藏' }
  ] }
})
