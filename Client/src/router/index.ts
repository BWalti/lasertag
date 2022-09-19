import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";

import Home from "../views/Home.vue";
import About from "../views/About.vue";

const routes : RouteRecordRaw[] = [
  {
    name: "Home",
    path: "/",
    component: Home,
  },
  {
    name: "About",
    path: "/about",
    component: About
  },
];

export const router = createRouter({
  history: createWebHistory(),
  routes,
});
