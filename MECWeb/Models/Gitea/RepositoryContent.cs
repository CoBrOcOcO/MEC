namespace MECWeb.Models.Gitea
{
    public class RepositoryContent
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "file" oder "dir"
        public string Download_Url { get; set; } = string.Empty;
        public string Html_Url { get; set; } = string.Empty;
        public string Git_Url { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty; // Base64 encoded content
        public int Size { get; set; }
        public string Sha { get; set; } = string.Empty;
        public string Encoding { get; set; } = string.Empty; // meist "base64"

        // 🔧 Hilfsmethoden
        public bool IsFile => Type == "file";
        public bool IsDirectory => Type == "dir";

        // Content als String dekodieren
        public string GetDecodedContent()
        {
            if (string.IsNullOrEmpty(Content))
                return string.Empty;

            try
            {
                var bytes = Convert.FromBase64String(Content);
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        // Prüfen ob Content verfügbar ist
        public bool HasContent => !string.IsNullOrEmpty(Content);
    }
}