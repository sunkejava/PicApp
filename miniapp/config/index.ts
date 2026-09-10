import type { UserConfigExport } from '@tarojs/cli'

export default {
  projectName: 'theme-wallpaper-miniapp',
  date: '2026-09-10',
  sourceRoot: 'src', outputRoot: 'dist',
  plugins: ['@tarojs/plugin-framework-react'],
  framework: 'react', compiler: 'webpack5',
  mini: {}, h5: {}
} satisfies UserConfigExport
