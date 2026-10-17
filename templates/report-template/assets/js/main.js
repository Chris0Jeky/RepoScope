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
    renderCodeChurnOverTime(metrics.codeChurnOverTime || []);
    renderAuthorsTable(metrics.commitsByAuthor, metrics.totalCommits);
    renderDirectoriesTable(metrics.commitsByDirectory, metrics.totalCommits);
    renderFileHotspotsTable(metrics.fileHotspots || []);
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

    infoGrid.replaceChildren(...info.map(item => {
        const row = el('div', 'info-row');
        row.append(el('span', 'info-label', `${item.label}:`), el('span', 'info-value', item.value));
        return row;
    }));
}

function renderStatsCards(metrics) {
    const statsGrid = document.getElementById('stats-cards');

    const stats = [
        { label: 'Total Commits', value: metrics.totalCommits },
        { label: 'Contributors', value: metrics.uniqueAuthors },
        { label: 'Directories', value: metrics.commitsByDirectory.length },
        { label: 'Active Days', value: metrics.commitsOverTime.length }
    ];

    statsGrid.replaceChildren(...stats.map(stat => {
        const card = el('div', 'stat-card');
        card.append(el('div', 'stat-value', stat.value), el('div', 'stat-label', stat.label));
        return card;
    }));
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

    tbody.replaceChildren(...data.map(author => {
        const percentage = ((author.commitCount / totalCommits) * 100).toFixed(1);
        return row([
            [author.authorName],
            [author.authorEmail],
            [author.commitCount, 'number'],
            [`${percentage}%`, 'number percentage']
        ]);
    }));
}

function renderDirectoriesTable(data, totalCommits) {
    const tbody = document.querySelector('#directories-table tbody');

    tbody.replaceChildren(...data.map(dir => {
        const percentage = ((dir.commitCount / totalCommits) * 100).toFixed(1);
        return row([
            [dir.directoryPath],
            [dir.commitCount, 'number'],
            [`${percentage}%`, 'number percentage']
        ]);
    }));
}

function renderCodeChurnOverTime(data) {
    if (!data || data.length === 0) return;

    const ctx = document.getElementById('code-churn-over-time');

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: data.map(d => d.day),
            datasets: [
                {
                    label: 'Lines Added',
                    data: data.map(d => d.linesAdded),
                    backgroundColor: 'rgba(16, 185, 129, 0.7)',
                    borderColor: '#10b981',
                    borderWidth: 1
                },
                {
                    label: 'Lines Deleted',
                    data: data.map(d => -d.linesDeleted),
                    backgroundColor: 'rgba(239, 68, 68, 0.7)',
                    borderColor: '#ef4444',
                    borderWidth: 1
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: true,
                    position: 'top'
                },
                tooltip: {
                    mode: 'index',
                    intersect: false,
                    callbacks: {
                        label: function(context) {
                            let label = context.dataset.label || '';
                            if (label) {
                                label += ': ';
                            }
                            // Show absolute value for deleted lines
                            const value = Math.abs(context.parsed.y);
                            label += value.toLocaleString() + ' lines';
                            return label;
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function(value) {
                            return Math.abs(value).toLocaleString();
                        }
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

function renderFileHotspotsTable(data) {
    if (!data || data.length === 0) return;

    const tbody = document.querySelector('#file-hotspots-table tbody');

    // Show top 50 files
    const top50 = data.slice(0, 50);

    tbody.replaceChildren(...top50.map(file => {
        return row([
            [file.filePath],
            [file.commitCount, 'number'],
            [file.linesAdded.toLocaleString(), 'number', { color: '#10b981' }],
            [file.linesDeleted.toLocaleString(), 'number', { color: '#ef4444' }],
            [file.totalChurn.toLocaleString(), 'number', { fontWeight: '600' }]
        ]);
    }));
}

// Every value in metrics.json comes from the analysed repository (paths, branch, author names, ...) and is
// attacker-controlled. Build DOM nodes and assign text only through textContent; never build markup from strings.
function el(tag, className, text) {
    const node = document.createElement(tag);
    if (className) node.className = className;
    if (text !== undefined) node.textContent = String(text);
    return node;
}

// cells: [[text, className?, styleProps?], ...]
function row(cells) {
    const tr = document.createElement('tr');
    for (const [text, className, style] of cells) {
        const td = el('td', className, text);
        if (style) Object.assign(td.style, style);
        tr.appendChild(td);
    }
    return tr;
}
