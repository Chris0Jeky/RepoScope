<script setup lang="ts">
import { computed } from 'vue';
import { useMetricsStore } from '../store/metricsStore';
import RepoSummaryCard from '../components/RepoSummaryCard.vue';
import CommitsOverTimeChart from '../components/CommitsOverTimeChart.vue';
import CommitsByAuthorChart from '../components/CommitsByAuthorChart.vue';
import CommitsByDirectoryChart from '../components/CommitsByDirectoryChart.vue';
import CodeChurnChart from '../components/CodeChurnChart.vue';
import FileHotspotsTable from '../components/FileHotspotsTable.vue';

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

      <div class="card" v-if="metrics.codeChurnOverTime && metrics.codeChurnOverTime.length > 0">
        <h2>Code Churn Over Time</h2>
        <CodeChurnChart :data="metrics.codeChurnOverTime" />
      </div>

      <div class="card" v-if="metrics.fileHotspots && metrics.fileHotspots.length > 0">
        <h2>File Hotspots</h2>
        <p class="subtitle">Files with the most changes and code churn</p>
        <FileHotspotsTable :data="metrics.fileHotspots" />
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

.card {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 2rem;
}

.card h2 {
  font-size: 1.5rem;
  margin-bottom: 0.5rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.subtitle {
  color: #666;
  font-size: 0.9rem;
  margin-bottom: 1rem;
}

@media (max-width: 768px) {
  .charts-row {
    grid-template-columns: 1fr;
  }
}
</style>
