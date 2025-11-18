<script setup lang="ts">
import { onMounted } from 'vue';
import DashboardView from './views/DashboardView.vue';
import { useMetricsStore } from './store/metricsStore';

const metricsStore = useMetricsStore();

onMounted(() => {
  // Load metrics from metrics.json in the same directory
  // This assumes the app is deployed alongside metrics.json
  metricsStore.loadFromJson('./metrics.json');
});
</script>

<template>
  <div class="app-container">
    <header class="app-header">
      <div class="container">
        <h1>RepoScope</h1>
        <p class="tagline">Git Repository Analyzer & Visualizer</p>
      </div>
    </header>

    <main class="app-main">
      <div v-if="metricsStore.state.loading" class="loading">
        Loading metrics...
      </div>
      <div v-else-if="metricsStore.state.error" class="error">
        Error: {{ metricsStore.state.error }}
      </div>
      <DashboardView v-else-if="metricsStore.state.metrics" />
      <div v-else class="empty">
        No metrics loaded
      </div>
    </main>
  </div>
</template>

<style scoped>
.app-container {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.app-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 2rem 1rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.container {
  max-width: 1200px;
  margin: 0 auto;
}

.tagline {
  opacity: 0.9;
  font-size: 1.1rem;
  margin-top: 0.25rem;
}

.app-main {
  flex: 1;
  padding: 2rem 1rem;
}

.loading,
.error,
.empty {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
  text-align: center;
  font-size: 1.1rem;
}

.error {
  color: #e53e3e;
  background: #fff5f5;
  border-radius: 8px;
}
</style>
