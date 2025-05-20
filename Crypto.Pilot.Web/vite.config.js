import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  build: {
    rollupOptions: {
      output: {
        manualChunks: {
          'vendor': ['react', 'react-dom', '@mui/material', '@azure/msal-browser', '@azure/msal-react'],
        },
        // Combine chunks to reduce file count
        chunkFileNames: 'assets/[name]-[hash].js',
        entryFileNames: 'assets/[name]-[hash].js',
        assetFileNames: 'assets/[name]-[hash][extname]'
      },
    },
    // Minimize CSS and JS
    minify: 'esbuild',
    cssMinify: true,
    // Disable sourcemaps
    sourcemap: false,
    // Clean output directory
    emptyOutDir: true,
    // Reduce chunk size
    chunkSizeWarningLimit: 1000,
    // Enable additional optimizations
    target: 'esnext',
    assetsInlineLimit: 4096
  }
})
