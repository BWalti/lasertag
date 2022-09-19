import { createApp } from "vue";
import "./style.css";

import { router } from "./router";
import state from "./state";
import App from "./App.vue";

const app = createApp(App);
app.use(state);
app.use(router);
app.mount("#app");
