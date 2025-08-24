<template>
    <v-sheet class="pa-4">
        <v-skeleton-loader v-if="loading" type="list-item-avatar-three-line@3"></v-skeleton-loader>
        <div v-else class="details-grid">
            <div v-for="(item, index) in details" :key="index" class="detail-item">
                <div v-for="header in headers" :key="header.value" class="detail-field">
                    <strong class="field-title">{{ header.title }}:</strong>
                    <span class="field-value">{{ item[header.value] }}</span>
                </div>
            </div>
        </div>
    </v-sheet>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

const props = defineProps({
    numLignes: {
        type: Array as () => number[],
        required: true
    }
})

const details = ref<any[]>([])
const loading = ref(false)

const headers = [
    { title: 'PeriodeCloture', value: 'periodeCloture' },
    { title: 'Source', value: 'source' },
    { title: 'Identifiant', value: 'identifiant' },
    { title: 'Nom', value: 'nom' },
    { title: 'ValeurDeMarche', value: 'valeurDeMarche' },
    { title: 'Categorie1', value: 'categorie1' },
    { title: 'Categorie2', value: 'categorie2' },
    { title: 'DeviseDeCotation', value: 'deviseDeCotation' },
    { title: 'TauxObligation', value: 'tauxObligation' },
    { title: 'DateMaturite', value: 'dateMaturite' },
    { title: 'DateExpiration', value: 'dateExpiration' },
    { title: 'Tiers', value: 'tiers' },
    { title: 'Raf', value: 'raf' },
    { title: 'BoaSj', value: 'boaSj' },
    { title: 'BoaContrepartie', value: 'boaContrepartie' },
    { title: 'BoaDefaut', value: 'boaDefaut' },
    { title: 'IdentifiantOrigine', value: 'identifiantOrigine' },
    { title: 'RefCategorieRwa', value: 'refCategorieRwa' },
    { title: 'IdentifiantUniqueRetenu', value: 'identifiantUniqueRetenu' },
    { title: 'Rafenrichi', value: 'rafenrichi' },
    { title: 'LibelleOrigine', value: 'libelleOrigine' },
    { title: 'DateFinContrat', value: 'dateFinContrat' },
    { title: 'Commentaires', value: 'commentaires' },
    { title: 'Bloomberg', value: 'bloomberg' },
    { title: 'RefTypeDepot', value: 'refTypeDepot' },
    { title: 'RefTypeResultat', value: 'refTypeResultat' },
    { title: 'CodeResultat', value: 'codeResultat' },
]

async function fetchDetails() {
    loading.value = true
    console.log('Fetching details for numLignes:', props.numLignes)
    try {
        const response = await fetch('/api/workflow/get-inventaire-normalise-by-numlignes', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(props.numLignes)
        })

        if (!response.ok) {
            throw new Error('Failed to fetch expanded data')
        }

        const data = await response.json()
        console.log('Fetched details:', data)
        details.value = data
    } finally {
        loading.value = false
    }
}

onMounted(() => {
    fetchDetails()
})
</script>

<style scoped>
.details-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
    gap: 16px;
}

.detail-item {
    border: 1px solid #e0e0e0;
    border-radius: 8px;
    padding: 16px;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.detail-field {
    display: flex;
    justify-content: space-between;
    padding: 4px 0;
    border-bottom: 1px solid #f0f0f0;
}

.detail-field:last-child {
    border-bottom: none;
}

.field-title {
    font-weight: bold;
    margin-right: 8px;
}

.field-value {
    text-align: right;
}
</style>
