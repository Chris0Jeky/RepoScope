import { reactive, readonly } from 'vue';
import type { RepoMetrics } from '../types/metrics';

interface MetricsState {
  metrics: RepoMetrics | null;
  loading: boolean;
  error: string | null;
}

const state = reactive<MetricsState>({
  metrics: null,
  loading: false,
  error: null
});

export const useMetricsStore = () => {
  const loadFromJson = async (url: string) => {
    state.loading = true;
    state.error = null;

    try {
      const response = await fetch(url);
      if (!response.ok) {
        throw new Error(`Failed to load metrics: ${response.statusText}`);
      }
      state.metrics = await response.json();
    } catch (err) {
      state.error = err instanceof Error ? err.message : 'Unknown error';
      console.error('Failed to load metrics:', err);
    } finally {
      state.loading = false;
    }
  };

  const setMetrics = (metrics: RepoMetrics) => {
    state.metrics = metrics;
  };

  return {
    state: readonly(state),
    loadFromJson,
    setMetrics
  };
};
