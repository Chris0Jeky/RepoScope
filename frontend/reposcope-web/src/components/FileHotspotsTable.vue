<template>
  <div class="table-container">
    <table>
      <thead>
        <tr>
          <th>File Path</th>
          <th class="number">Commits</th>
          <th class="number">Lines Added</th>
          <th class="number">Lines Deleted</th>
          <th class="number">Total Churn</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="file in topFiles" :key="file.filePath">
          <td class="file-path">{{ file.filePath }}</td>
          <td class="number">{{ file.commitCount }}</td>
          <td class="number added">{{ file.linesAdded.toLocaleString() }}</td>
          <td class="number deleted">{{ file.linesDeleted.toLocaleString() }}</td>
          <td class="number churn">{{ file.totalChurn.toLocaleString() }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { FileHotspot } from '../types/metrics'

const props = defineProps<{
  data: FileHotspot[]
}>()

const topFiles = computed(() => {
  return props.data.slice(0, 50)
})
</script>

<style scoped>
.table-container {
  overflow-x: auto;
  margin-top: 1rem;
}

table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
}

thead {
  background: #f7fafc;
}

th {
  text-align: left;
  padding: 0.75rem 1rem;
  font-weight: 600;
  color: #4a5568;
  border-bottom: 2px solid #e2e8f0;
}

th.number {
  text-align: right;
}

td {
  padding: 0.75rem 1rem;
  border-bottom: 1px solid #e2e8f0;
}

td.file-path {
  font-family: 'Monaco', 'Menlo', 'Courier New', monospace;
  font-size: 0.85rem;
}

td.number {
  text-align: right;
  font-variant-numeric: tabular-nums;
}

td.added {
  color: #10b981;
}

td.deleted {
  color: #ef4444;
}

td.churn {
  font-weight: 600;
}

tbody tr:hover {
  background: #f7fafc;
}

tbody tr:last-child td {
  border-bottom: none;
}
</style>
