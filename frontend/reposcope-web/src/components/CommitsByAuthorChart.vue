<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { Chart, registerables } from 'chart.js';
import type { CommitsByAuthor } from '../types/metrics';

Chart.register(...registerables);

const props = defineProps<{
  data: CommitsByAuthor[];
}>();

const chartCanvas = ref<HTMLCanvasElement | null>(null);

onMounted(() => {
  if (!chartCanvas.value) return;

  const top10 = props.data.slice(0, 10);

  new Chart(chartCanvas.value, {
    type: 'bar',
    data: {
      labels: top10.map(d => d.authorName),
      datasets: [{
        label: 'Commits',
        data: top10.map(d => d.commitCount),
        backgroundColor: '#667eea'
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: true,
      indexAxis: 'y',
      plugins: {
        legend: {
          display: false
        },
        tooltip: {
          callbacks: {
            afterLabel: (context) => {
              const author = top10[context.dataIndex];
              return author.authorEmail;
            }
          }
        }
      },
      scales: {
        x: {
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
    <h3>Top Contributors</h3>
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
  max-height: 400px;
}
</style>
