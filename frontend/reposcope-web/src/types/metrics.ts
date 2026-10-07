export interface RepoMetrics {
  repoPath: string;
  branch: string | null;
  headCommitId: string;
  totalCommits: number;
  earliestCommitDate: string | null;
  latestCommitDate: string | null;
  uniqueAuthors: number;
  commitsOverTime: readonly CommitsByDay[];
  commitsByAuthor: readonly CommitsByAuthor[];
  commitsByDirectory: readonly CommitsByDirectory[];
  fileHotspots: readonly FileHotspot[];
  codeChurnOverTime: readonly CodeChurnByDay[];
}

export interface CommitsByDay {
  day: string;
  commitCount: number;
}

export interface CommitsByAuthor {
  authorName: string;
  authorEmail: string;
  commitCount: number;
}

export interface CommitsByDirectory {
  directoryPath: string;
  commitCount: number;
}

export interface FileHotspot {
  filePath: string;
  commitCount: number;
  linesAdded: number;
  linesDeleted: number;
  totalChurn: number;
  netChange: number;
}

export interface CodeChurnByDay {
  day: string;
  commitCount: number;
  linesAdded: number;
  linesDeleted: number;
  totalChurn: number;
  netChange: number;
}
