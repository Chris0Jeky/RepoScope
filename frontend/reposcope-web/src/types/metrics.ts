export interface RepoMetrics {
  repoPath: string;
  branch: string | null;
  headCommitId: string;
  totalCommits: number;
  earliestCommitDate: string | null;
  latestCommitDate: string | null;
  uniqueAuthors: number;
  commitsOverTime: CommitsByDay[];
  commitsByAuthor: CommitsByAuthor[];
  commitsByDirectory: CommitsByDirectory[];
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
