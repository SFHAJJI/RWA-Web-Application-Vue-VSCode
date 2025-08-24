<template>
  <div>
    <h2>BDD Manager</h2>
    <div class="card">
      <div class="flex justify-content-center mb-4">
        <v-btn-toggle v-model="selectedView" divided>
          <v-btn value="Successful">Successful</v-btn>
          <v-btn value="Failed">Failed</v-btn>
        </v-btn-toggle>
      </div>

      <v-container fluid>
        <v-row dense>
          <v-col
            v-for="item in datatableData"
            :key="item.Identifiant"
            cols="12"
            md="6"
            lg="4"
          >
            <v-card class="mb-4">
              <v-card-title>
                {{ item.Nom }}
              </v-card-title>
              <v-card-subtitle>
                {{ item.Identifiant }}
              </v-card-subtitle>
              <v-card-text>
                <div><strong>Num Ligne:</strong> {{ item.NumLigne }}</div>
                <div><strong>RAF Enrichi:</strong> {{ item.Rafenrichi }}</div>
                <div><strong>Date Fin Contrat:</strong> {{ item.DateFinContrat }}</div>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';

const props = defineProps({
  step: {
    type: Object,
    required: true
  }
});

const selectedView = ref('Successful');

const datatableData = computed(() => {
  try {
    const payload = JSON.parse(props.step.DataPayload);
    return selectedView.value === 'Successful' ? payload.SuccessfulMatches : payload.FailedMatches;
  } catch (e) {
    return [];
  }
});
</script>

<style scoped>
.card {
  background: #ffffff;
  padding: 2rem;
  border-radius: 10px;
  margin-bottom: 1rem;
}
</style>
