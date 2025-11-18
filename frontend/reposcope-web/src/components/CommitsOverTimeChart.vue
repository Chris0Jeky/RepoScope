<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { Chart, registerables } from 'chart.js';
import type { CommitsByDay } from '../types/metrics';

Chart.register(...registerables);

const props = defineProps<{
  data: CommitsByDay[];
}>();

const chartCanvas = ref<HTMLCanvasElement | null>(null);

onMounted(() => {
  if (!chartCanvas.value) return;

  new Chart(chartCanvas.value, {
    type: 'line',
    data: {
      labels: props.data.map(d => d.day),
      datasets: [{
        label: 'Commits',
        data: props.data.map(d => d.commitCount),
        borderColor: '#667eea',
        backgroundColor: 'rgba(102, 126, 234, 0.1)',
        tension: 0.3,
        fill: true
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: true,
      plugins: {
        legend: {
          display: false
        },
        tooltip: {
          mode: 'index',
          intersect: false
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
    <h3>Commits Over Time</h3>
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
