<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { Chart, registerables } from 'chart.js';
import type { CommitsByDirectory } from '../types/metrics';

Chart.register(...registerables);

const props = defineProps<{
  data: CommitsByDirectory[];
}>();

const chartCanvas = ref<HTMLCanvasElement | null>(null);

onMounted(() => {
  if (!chartCanvas.value) return;

  const top10 = props.data.slice(0, 10);

  new Chart(chartCanvas.value, {
    type: 'bar',
    data: {
      labels: top10.map(d => d.directoryPath),
      datasets: [{
        label: 'Commits',
        data: top10.map(d => d.commitCount),
        backgroundColor: '#10b981'
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: true,
      plugins: {
        legend: {
          display: false
        }
      },
      scales: {
        y: {
          beginAtZero: true,
          ticks: {
            precision: 0
          }
        }
      }
    }
  });
});
</script>

<template>
  <div class="chart-container">
    <h3>Most Active Directories</h3>
    <canvas ref="chartCanvas"></canvas>
  </div>
</template>

<style scoped>
.chart-container {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  margin-bottom: 2rem;
}

canvas {
  max-height: 300px;
}
</style>
