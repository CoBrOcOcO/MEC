namespace MECWeb.Models.Gitea
{
    public class Release
    {
        public string Name { get; set; } = string.Empty;
        public string TagName { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<ReleaseAsset> Assets { get; set; } = new();
    }

    public class ReleaseAsset
    {
        public string Name { get; set; } = string.Empty;
        public string BrowserDownloadUrl { get; set; } = string.Empty;
    }
}