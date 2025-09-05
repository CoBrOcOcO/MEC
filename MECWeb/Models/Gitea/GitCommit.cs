// Models/Gitea/GitCommit.cs - KOMPLETTES MODEL für korrekte Timestamps

namespace MECWeb.Models.Gitea
{
    public class GitCommit
    {
        public string Sha { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Html_Url { get; set; } = string.Empty;

        // Nested Commit-Struktur (das ist der wichtige Teil!)
        public GitCommitInfo? Commit { get; set; }

        // Gitea-User-Info
        public GitAuthorInfo? Author { get; set; }
        public GitAuthorInfo? Committer { get; set; }

        // LEGACY - für Abwärtskompatibilität
        public DateTime Date { get; set; }
        public GitCommitTree? Tree { get; set; }
        public List<GitCommitParent>? Parents { get; set; }
    }

    public class GitCommitInfo
    {
        public string Message { get; set; } = string.Empty;
        public GitAuthor? Author { get; set; }
        public GitAuthor? Committer { get; set; }
        public GitCommitTree? Tree { get; set; }
        public GitCommitVerification? Verification { get; set; }
    }

    public class GitAuthor
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    public class GitAuthorInfo
    {
        public string Login { get; set; } = string.Empty;
        public string Avatar_Url { get; set; } = string.Empty;
        public string Html_Url { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Full_Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // ✅ KORRIGIERT: Name-Property für bessere Kompatibilität
        public string Name => string.IsNullOrEmpty(Full_Name) ? Login : Full_Name;
    }

    public class GitCommitTree
    {
        public string Sha { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public class GitCommitParent
    {
        public string Sha { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public class GitCommitVerification
    {
        public bool Verified { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Signature { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
    }

    // Für detaillierte Commit-Informationen
    public class GitCommitDetail
    {
        public string Sha { get; set; } = string.Empty;
        public GitCommitInfo? Commit { get; set; }
        public GitAuthorInfo? Author { get; set; }
        public GitAuthorInfo? Committer { get; set; }
        public List<GitCommitFile>? Files { get; set; }
        public GitCommitStats? Stats { get; set; }
    }

    public class GitCommitFile
    {
        public string Filename { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // "added", "modified", "deleted"
        public int Additions { get; set; }
        public int Deletions { get; set; }
        public int Changes { get; set; }
        public string Blob_Url { get; set; } = string.Empty;
        public string Raw_Url { get; set; } = string.Empty;
        public string Contents_Url { get; set; } = string.Empty;
        public string Patch { get; set; } = string.Empty;
    }

    public class GitCommitStats
    {
        public int Total { get; set; }
        public int Additions { get; set; }
        public int Deletions { get; set; }
    }
}