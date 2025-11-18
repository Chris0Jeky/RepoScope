// Load and render metrics
document.addEventListener('DOMContentLoaded', () => {
    loadMetrics();
});

async function loadMetrics() {
    try {
        const response = await fetch('metrics.json');

        if (!response.ok) {
            throw new Error(`Failed to load metrics.json: ${response.statusText}`);
        }

        const metrics = await response.json();
        renderReport(metrics);

        // Hide loading, show content
        document.getElementById('loading').style.display = 'none';
        document.getElementById('report-content').style.display = 'block';
    } catch (error) {
        console.error('Error loading metrics:', error);
        showError(error.message);
    }
}

function showError(message) {
    document.getElementById('loading').style.display = 'none';
    const errorContainer = document.getElementById('error-container');
    errorContainer.style.display = 'block';
    document.getElementById('error-message').textContent = message;
}

function renderReport(metrics) {
    renderRepoInfo(metrics);
    renderStatsCards(metrics);
    renderCommitsOverTime(metrics.commitsOverTime);
    renderCommitsByAuthor(metrics.commitsByAuthor);
    renderCommitsByDirectory(metrics.commitsByDirectory);
    renderAuthorsTable(metrics.commitsByAuthor, metrics.totalCommits);
    renderDirectoriesTable(metrics.commitsByDirectory, metrics.totalCommits);
}

function renderRepoInfo(metrics) {
    const infoGrid = document.getElementById('repo-info');

    const formatDate = (dateStr) => {
        if (!dateStr) return 'N/A';
        return new Date(dateStr).toLocaleDateString();
    };

    const info = [
        { label: 'Repository Path', value: metrics.repoPath },
        { label: 'Branch', value: metrics.branch || 'N/A' },
        { label: 'Head Commit', value: metrics.headCommitId.substring(0, 8) },
        { label: 'Date Range', value: `${formatDate(metrics.earliestCommitDate)} - ${formatDate(metrics.latestCommitDate)}` }
    ];

    infoGrid.innerHTML = info.map(item => `
        <div class="info-row">
            <span class="info-label">${item.label}:</span>
            <span class="info-value">${item.value}</span>
        </div>
    `).join('');
}

function renderStatsCards(metrics) {
    const statsGrid = document.getElementById('stats-cards');

    const stats = [
        { label: 'Total Commits', value: metrics.totalCommits },
        { label: 'Contributors', value: metrics.uniqueAuthors },
        { label: 'Directories', value: metrics.commitsByDirectory.length },
        { label: 'Active Days', value: metrics.commitsOverTime.length }
    ];

    statsGrid.innerHTML = stats.map(stat => `
        <div class="stat-card">
            <div class="stat-value">${stat.value}</div>
            <div class="stat-label">${stat.label}</div>
        </div>
    `).join('');
}

function renderCommitsOverTime(data) {
    const ctx = document.getElementById('commits-over-time');

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.map(d => d.day),
            datasets: [{
                label: 'Commits',
                data: data.map(d => d.commitCount),
                borderColor: '#667eea',
                backgroundColor: 'rgba(102, 126, 234, 0.1)',
                tension: 0.3,
                fill: true,
                pointRadius: 2,
                pointHoverRadius: 5
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
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
                },
                x: {
                    ticks: {
                        maxRotation: 45,
                        minRotation: 45
                    }
                }
            }
        }
    });
}

function renderCommitsByAuthor(data) {
    const ctx = document.getElementById('commits-by-author');
    const top10 = data.slice(0, 10);

    new Chart(ctx, {
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
            maintainAspectRatio: false,
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
}

function renderCommitsByDirectory(data) {
    const ctx = document.getElementById('commits-by-directory');
    const top10 = data.slice(0, 10);

    new Chart(ctx, {
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
            maintainAspectRatio: false,
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
}

function renderAuthorsTable(data, totalCommits) {
    const tbody = document.querySelector('#authors-table tbody');

    tbody.innerHTML = data.map(author => {
        const percentage = ((author.commitCount / totalCommits) * 100).toFixed(1);
        return `
            <tr>
                <td>${escapeHtml(author.authorName)}</td>
                <td>${escapeHtml(author.authorEmail)}</td>
                <td class="number">${author.commitCount}</td>
                <td class="number percentage">${percentage}%</td>
            </tr>
        `;
    }).join('');
}

function renderDirectoriesTable(data, totalCommits) {
    const tbody = document.querySelector('#directories-table tbody');

    tbody.innerHTML = data.map(dir => {
        const percentage = ((dir.commitCount / totalCommits) * 100).toFixed(1);
        return `
            <tr>
                <td>${escapeHtml(dir.directoryPath)}</td>
                <td class="number">${dir.commitCount}</td>
                <td class="number percentage">${percentage}%</td>
            </tr>
        `;
    }).join('');
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}
