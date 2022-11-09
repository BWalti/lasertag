import { createApp } from "vue";
import { createHead } from "@vueuse/head";
import { createRouter, createWebHistory } from "vue-router/auto";
import { createPinia } from "pinia";

import App from "./App.vue";
import "./style.css";

const app = createApp(App);
const head = createHead();
const router = createRouter({
  history: createWebHistory(),
});

app.use(createPinia());
app.use(router);
app.use(head);
app.mount("#app");
