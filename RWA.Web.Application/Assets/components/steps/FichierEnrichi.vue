<template>
    <div>
        <h2>Fichier Enrichi</h2>
        <div class="card">
            <v-container fluid>
                <v-card>
                    <v-card-title class="data-table-title success-title">
                        <v-icon color="white" class="mr-2">mdi-file-excel</v-icon>
                        Export Fichier Enrichi
                    </v-card-title>
                    <v-card-subtitle>Download the enriched file in Excel format.</v-card-subtitle>
                    <v-card-actions>
                        <v-spacer></v-spacer>
                        <v-btn color="primary" @click="downloadEnrichi">Download Enrichi</v-btn>
                    </v-card-actions>
                </v-card>
            </v-container>
        </div>
    </div>
</template>

<script setup lang="ts">
const downloadEnrichi = async () => {
    try {
        const response = await fetch('/api/export/inventaire');
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'inventaire.xlsx';
        document.body.appendChild(a);
        a.click();
        a.remove();
    } catch (error) {
        console.error('There was an error with the download:', error);
    }
};
</script>

<style scoped>
.card {
    background: #ffffff;
    padding: 2rem;
    border-radius: 10px;
    margin-bottom: 1rem;
}

.data-table-title {
    font-weight: 500;
    color: white;
    display: flex;
    align-items: center;
}

.success-title {
    background-color: #388e3c; /* Vuetify Success Color */
}
</style>
