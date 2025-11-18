<script setup lang="ts">
import type { RepoMetrics } from '../types/metrics';

defineProps<{
  metrics: RepoMetrics;
}>();

const formatDate = (date: string | null) => {
  if (!date) return 'N/A';
  return new Date(date).toLocaleDateString();
};
</script>

<template>
  <div class="summary-card">
    <div class="repo-info">
      <h2>Repository Information</h2>
      <div class="info-row">
        <span class="label">Path:</span>
        <span class="value">{{ metrics.repoPath }}</span>
      </div>
      <div class="info-row">
        <span class="label">Branch:</span>
        <span class="value">{{ metrics.branch || 'N/A' }}</span>
      </div>
      <div class="info-row">
        <span class="label">Head:</span>
        <span class="value">{{ metrics.headCommitId.substring(0, 8) }}</span>
      </div>
      <div class="info-row">
        <span class="label">Date Range:</span>
        <span class="value">{{ formatDate(metrics.earliestCommitDate) }} - {{ formatDate(metrics.latestCommitDate) }}</span>
      </div>
    </div>

    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-value">{{ metrics.totalCommits }}</div>
        <div class="stat-label">Total Commits</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ metrics.uniqueAuthors }}</div>
        <div class="stat-label">Contributors</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ metrics.commitsByDirectory.length }}</div>
        <div class="stat-label">Directories</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ metrics.commitsOverTime.length }}</div>
        <div class="stat-label">Active Days</div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.summary-card {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  margin-bottom: 2rem;
}

.repo-info {
  margin-bottom: 1.5rem;
}

.info-row {
  display: flex;
  padding: 0.5rem 0;
  border-bottom: 1px solid #f0f0f0;
}

.info-row:last-child {
  border-bottom: none;
}

.label {
  font-weight: 600;
  width: 120px;
  color: #666;
}

.value {
  flex: 1;
  color: #333;
  font-family: 'Monaco', 'Menlo', monospace;
  font-size: 0.9rem;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
  margin-top: 1.5rem;
}

.stat-card {
  text-align: center;
  padding: 1rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 8px;
  color: white;
}

.stat-value {
  font-size: 2rem;
  font-weight: bold;
  margin-bottom: 0.25rem;
}

.stat-label {
  font-size: 0.9rem;
  opacity: 0.9;
}
</style>
