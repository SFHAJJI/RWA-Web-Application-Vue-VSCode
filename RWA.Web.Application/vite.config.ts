/**
 * Name: vite.config.ts
 * Description: Vite configuration file
 */

import { UserConfig, defineConfig } from 'vite';
import { spawn, spawnSync } from 'child_process';
import fs from 'fs';
import path from 'path';
import vue from "@vitejs/plugin-vue";
import Components from "unplugin-vue-components/vite";
import { PrimeVueResolver } from "@primevue/auto-import-resolver";

// Get base folder for certificates.
const baseFolder =
    process.env.APPDATA !== undefined && process.env.APPDATA !== ''
        ? `${process.env.APPDATA}/ASP.NET/https`
        : `${process.env.HOME}/.aspnet/https`;

// Generate the certificate name using the NPM package name
const certificateName = process.env.npm_package_name;

// Define certificate filepath
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
// Define key filepath
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

// Export Vite configuration
export default defineConfig(async () => {
    // Ensure the certificate and key exist
    if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
        // Wait for the certificate to be generated
        await new Promise<void>((resolve) => {
            spawn('dotnet', [
                'dev-certs',
                'https',
                '--export-path',
                certFilePath,
                '--format',
                'Pem',
                '--no-password',
            ], { stdio: 'inherit', })
                .on('exit', (code) => {
                    resolve();
                    if (code) {
                        process.exit(code);
                    }
                });
        });
    };

    // Define Vite configuration
    const config: UserConfig = {
        plugins: [
            vue(),
            Components({
                resolvers: [PrimeVueResolver()],
            })
        ],
        appType: 'custom',
        root: 'Assets',
        publicDir: 'public',
        build: {
            emptyOutDir: true,
            manifest: true,
            outDir: '../wwwroot',
            assetsDir: '',
            rollupOptions: {
                input: 'Assets/main.ts',
                output: {
                    manualChunks(id) {
                        if (id.includes('node_modules')) {
                            if (id.includes('node_modules/primevue')) return 'primevue-vendor';
                            if (id.includes('node_modules/vuetify')) return 'vuetify-vendor';
                            if (id.includes('node_modules/chart.js')) return 'chartjs-vendor';
                            if (id.includes('node_modules/@microsoft/signalr')) return 'signalr-vendor';
                            return 'vendor';
                        }
                    }
                }
            },
        },
        server: {
            strictPort: true,
            https: {
                cert: certFilePath,
                key: keyFilePath
            }
        },
        optimizeDeps: {
            include: []
        },
        css: {
            preprocessorOptions: {
                scss: {
                    api: "modern-compiler",
                },
            },
        },
    }

    return config;
});
