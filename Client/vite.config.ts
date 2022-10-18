import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";

import AutoImport from "unplugin-auto-import/vite";
import VueRouter from "unplugin-vue-router/vite";
import { VueRouterAutoImports } from "unplugin-vue-router";
import Components from "unplugin-vue-components/vite";
import { HeadlessUiResolver } from "unplugin-vue-components/resolvers";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    VueRouter({
      importMode: "sync",

      // Folder(s) to scan for vue components and generate routes. Can be a string, or
      // an object, or an array of those.
      routesFolder: "src/pages",

      // allowed extensions to be considered as routes
      extensions: [".vue"],

      dts: "./typed-router.d.ts",
    }),
    vue(),
    Components({
      dirs: [
        './src/components',
        './src/layouts'
      ],
      resolvers: [HeadlessUiResolver()],
      dts: true
    }),
    AutoImport({
      include: [
        /\.vue$/, /\.vue\?vue/, // .vue
      ],
      imports: ["vue", "@vueuse/head", VueRouterAutoImports],
      vueTemplate: true,
      dirs: [
        './src/layouts',
        './layouts'
      ]
    }),
  ],
  server: {
    host: true,
    port: 5173,
    hmr:{
      clientPort: 5010
    }
  },
});
