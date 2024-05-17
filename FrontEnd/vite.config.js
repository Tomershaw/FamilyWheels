import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react-swc';

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:7189', // Replace with the URL and port of your server
        changeOrigin: true,
        secure: false,
      },
    },
  },
});