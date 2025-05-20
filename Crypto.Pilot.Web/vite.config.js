import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import viteCompression from 'vite-plugin-compression'

export default defineConfig({
  plugins: [
    react(),
    viteCompression()
  ],
  build: {
    rollupOptions: {
      output: {
        manualChunks: {
          vendor: ['react', 'react-dom', '@mui/material', '@mui/icons-material', '@emotion/react', '@emotion/styled', '@azure/msal-browser', '@azure/msal-react']
        }
      }
    },
    minify: 'esbuild',
    sourcemap: false,
    emptyOutDir: true,
    chunkSizeWarningLimit: 1000,
    cssCodeSplit: false,
    assetsInlineLimit: 4096
  }
})
