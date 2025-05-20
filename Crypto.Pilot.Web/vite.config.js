import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import viteCompression from 'vite-plugin-compression'

export default defineConfig({
  plugins: [
    react({
      jsxImportSource: '@emotion/react',
      babel: {
        plugins: ['@emotion/babel-plugin']
      }
    }),
    viteCompression()
  ],
  build: {
    rollupOptions: {
      output: {
        manualChunks: {
          vendor: ['react', 'react-dom'],
          mui: ['@mui/material', '@emotion/react', '@emotion/styled', '@emotion/cache'],
          auth: ['@azure/msal-browser', '@azure/msal-react']
        }
      }
    },
    minify: 'esbuild',
    sourcemap: false,
    emptyOutDir: true,
    chunkSizeWarningLimit: 1000,
    cssCodeSplit: false,
    assetsInlineLimit: 4096
  },
  resolve: {
    alias: {
      '@': '/src'
    }
  }
})
