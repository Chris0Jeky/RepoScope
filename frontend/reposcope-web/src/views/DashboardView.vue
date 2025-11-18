<script setup lang="ts">
import { computed } from 'vue';
import { useMetricsStore } from '../store/metricsStore';
import RepoSummaryCard from '../components/RepoSummaryCard.vue';
import CommitsOverTimeChart from '../components/CommitsOverTimeChart.vue';
import CommitsByAuthorChart from '../components/CommitsByAuthorChart.vue';
import CommitsByDirectoryChart from '../components/CommitsByDirectoryChart.vue';

const metricsStore = useMetricsStore();
const metrics = computed(() => metricsStore.state.metrics);
</script>

<template>
  <div class="dashboard" v-if="metrics">
    <div class="container">
      <RepoSummaryCard :metrics="metrics" />

      <CommitsOverTimeChart
        v-if="metrics.commitsOverTime.length > 0"
        :data="metrics.commitsOverTime"
      />

      <div class="charts-row">
        <CommitsByAuthorChart
          v-if="metrics.commitsByAuthor.length > 0"
          :data="metrics.commitsByAuthor"
        />

        <CommitsByDirectoryChart
          v-if="metrics.commitsByDirectory.length > 0"
          :data="metrics.commitsByDirectory"
        />
      </div>
    </div>
  </div>
</template>

<style scoped>
.dashboard {
  width: 100%;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
}

.charts-row {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(500px, 1fr));
  gap: 2rem;
}

@media (max-width: 768px) {
  .charts-row {
    grid-template-columns: 1fr;
  }
}
</style>
