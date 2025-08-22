import "./assets/styles/main.css";
import "./public/css/audit-panel.css";
import 'vuetify/styles';
import { createVuetify } from 'vuetify';
import * as components from 'vuetify/components';
import * as directives from 'vuetify/directives';
import { aliases, mdi } from 'vuetify/iconsets/mdi'

import { createApp } from "vue";
import { createPinia } from 'pinia';
import App from "./App.vue";
import { useWorkflowStore } from './stores/workflow';

const app = createApp(App);
const pinia = createPinia();
const vuetify = createVuetify({
    components,
    directives,
        icons: {
                defaultSet: 'mdi',
                aliases,
                sets: { mdi }
        },
        theme: {
            defaultTheme: 'light'
        },
});

app.use(pinia);
app.use(vuetify);

// Only mount if the server-rendered host element exists on the current page.
const mountEl = document.getElementById('app');
if (mountEl) {
    app.mount(mountEl);
} else {
    // If the element is not present, do not throw — this allows the same bundle to be used across pages.
    // Developers can mount manually later if needed.
    // eslint-disable-next-line no-console
    console.warn('Vue mount skipped: #app not found on this page.');
}

const workflowStore = useWorkflowStore();
workflowStore.initSignalR();
