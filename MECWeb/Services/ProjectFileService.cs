namespace MECWeb.Services
{
    /// <summary>
    /// FileService Klasse für die Verwaltung von Dateien in der Anwendung.
    /// </summary>
    public class ProjectFileService
    {
        public const string TranslationPath = "translate";



        private readonly string _basePath;

        public ProjectFileService(string? basePath)
        {
            if (basePath == null)
            {
                throw new ArgumentNullException(nameof(basePath), "Base path cannot be null.");
            }
            _basePath = basePath;
        }

        /// <summary>
        /// Erstellt einen Projektordner anhand der Projekt-ID.
        /// </summary>
        public async Task<string> CreateProjectFolderAsync(Guid projectId)
        {
            var projectFolder = Path.Combine(_basePath, projectId.ToString());
            if (!Directory.Exists(projectFolder))
            {
                Directory.CreateDirectory(projectFolder);
            }
            // Async-API für Kompatibilität, auch wenn Directory.CreateDirectory synchron ist
            await Task.CompletedTask;
            return projectFolder;
        }

        /// <summary>
        /// Erstellt einen Unterordner im Projektordner.
        /// </summary>
        /// <param name="projectId">Projekt-ID</param>
        /// <param name="relativePath">Relativer Pfad innerhalb des Projektordners, z.B. "Dokumente/2024"</param>
        public async Task<string> CreateSubFolderAsync(Guid projectId, string relativePath)
        {
            var projectFolder = Path.Combine(_basePath, projectId.ToString());
            var subFolder = Path.Combine(projectFolder, relativePath);

            if (!Directory.Exists(subFolder))
            {
                Directory.CreateDirectory(subFolder);
            }
            await Task.CompletedTask;
            return subFolder;
        }

        /// <summary>
        /// Speichert eine Datei im Projektordner unter dem angegebenen relativen Pfad.
        /// </summary>
        /// <param name="projectId">Projekt-ID</param>
        /// <param name="relativePath">Relativer Pfad inkl. Dateiname, z.B. "Dokumente/2024/Datei.txt"</param>
        /// <param name="fileStream">Dateiinhalt als Stream</param>
        /// <returns>Vollständiger Pfad zur gespeicherten Datei</returns>
        public async Task<string> SaveFileAsync(Guid projectId, string relativePath, Stream fileStream)
        {
            var projectFolder = Path.Combine(_basePath, projectId.ToString());
            var filePath = Path.Combine(projectFolder, relativePath);

            // Sicherstellen, dass der Zielordner existiert
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            // Datei speichern
            using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.CopyToAsync(file);
            }

            return filePath;
        }










    }
}
