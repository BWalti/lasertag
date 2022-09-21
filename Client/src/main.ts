import { createApp } from "vue";
import { createHead } from "@vueuse/head";
import { createRouter, createWebHistory } from "vue-router/auto";

import App from "./App.vue";
import "./style.css";
import state from "./state";

const app = createApp(App);
const head = createHead();
const router = createRouter({
  history: createWebHistory(),
});

app.use(state);
app.use(router);
app.use(head);
app.mount("#app");
