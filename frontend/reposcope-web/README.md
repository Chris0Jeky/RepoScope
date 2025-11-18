# RepoScope Web Dashboard

Modern Vue 3 + TypeScript dashboard for visualizing Git repository metrics.

## Features

- 📊 Interactive charts using Chart.js
- 📱 Responsive design
- 🎨 Modern gradient UI
- 📦 Standalone deployment (loads metrics.json)

## Development

```bash
# Install dependencies
npm install

# Start dev server (http://localhost:5173)
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview
```

## Project Structure

```
src/
├── components/       # Vue components
│   ├── RepoSummaryCard.vue
│   ├── CommitsOverTimeChart.vue
│   ├── CommitsByAuthorChart.vue
│   └── CommitsByDirectoryChart.vue
├── views/           # Page views
│   └── DashboardView.vue
├── store/           # State management
│   └── metricsStore.ts
├── types/           # TypeScript types
│   └── metrics.ts
├── App.vue          # Root component
├── main.ts          # Entry point
└── style.css        # Global styles
```

## Deployment

The dashboard is designed to work as a static site:

1. Build the project: `npm run build`
2. Copy `dist/` contents to your web server
3. Place `metrics.json` in the same directory
4. Open `index.html`

The dashboard will automatically load and visualize the metrics.

## Technologies

- **Vue 3** - Progressive JavaScript framework
- **TypeScript** - Type-safe development
- **Vite** - Lightning-fast build tool
- **Chart.js** - Beautiful charts

## Customization

To customize the charts, edit the chart components in `src/components/`.
Each chart is a self-contained Vue component with Chart.js configuration.
