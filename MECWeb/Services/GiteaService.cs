using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MECWeb.Models.Gitea;
using LibGit2Sharp;

namespace MECWeb.Services
{
    public class GiteaService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        // Binärdateien die über LFS verwaltet werden sollen
        private static readonly HashSet<string> BinaryFileExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".ap17", ".s7p", ".zap", ".tsproj", ".pro", ".acd", // SPS-Dateien
            ".zip", ".rar", ".7z", // Archive
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", // Bilder
            ".mp4", ".avi", ".mov", ".wmv", // Videos
            ".mp3", ".wav", ".flac", // Audio
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", // Office
            ".exe", ".dll", ".so", ".dylib" // Executables
        };

        public GiteaService(HttpClient httpClient, IConfiguration config)
        {
            var token = config["Gitea:Token"];
            var baseUrl = config["Gitea:ApiUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Gitea:ApiUrl ist nicht gesetzt.");

            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Gitea:Token ist nicht gesetzt.");

            _httpClient = httpClient;
            _config = config;
            _httpClient.BaseAddress = new Uri(baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
        }

        // ===== HELPER METHODS =====

        private bool IsBinaryFile(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            return BinaryFileExtensions.Contains(extension);
        }

        private bool IsLargeFile(long fileSize)
        {
            // Dateien über 100 MB sollten über LFS gehen
            return fileSize > (100 * 1024 * 1024);
        }

        private string GetGiteaRepoUrl(string owner, string repoName)
        {
            var baseAddress = _httpClient.BaseAddress?.ToString().TrimEnd('/');
            // Entferne /api/v1/ vom Ende falls vorhanden
            if (baseAddress?.EndsWith("/api/v1") == true)
            {
                baseAddress = baseAddress.Substring(0, baseAddress.Length - 7);
            }
            return $"{baseAddress}/{owner}/{repoName}.git";
        }

        // ===== REPOSITORY METHODS =====

        public async Task<GiteaRepository?> GetRepositoryAsync(string owner, string repoName)
        {
            var encodedRepoName = Uri.EscapeDataString(repoName);
            var response = await _httpClient.GetAsync($"repos/{owner}/{encodedRepoName}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Fehler beim Abrufen des Repositories: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var repo = JsonSerializer.Deserialize<GiteaRepository>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return repo;
        }

        public async Task<List<GiteaRepository>?> GetUserRepositoriesAsync()
        {
            var response = await _httpClient.GetAsync("user/repos");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Fehler beim Abrufen der Repositories: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var repos = JsonSerializer.Deserialize<List<GiteaRepository>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return repos;
        }

        public async Task<GiteaRepository?> CreateRepositoryAsync(string name, string description = "")
        {
            try
            {
                Console.WriteLine($"🚀 CREATE REPOSITORY DEBUG:");
                Console.WriteLine($"  Name: {name}");
                Console.WriteLine($"  Description: {description}");

                var url = "user/repos";
                Console.WriteLine($"  API-URL: {url}");

                var repoData = new
                {
                    name = name,
                    description = description,
                    @private = false,
                    auto_init = true
                };

                var jsonContent = JsonSerializer.Serialize(repoData);
                Console.WriteLine($"  JSON-Content: {jsonContent}");

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                Console.WriteLine($"  Response Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"  Error Response: {errorContent}");
                    Console.WriteLine($"❌ Repository-Erstellung fehlgeschlagen: {response.StatusCode}");
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"  Success Response Length: {responseContent.Length}");

                var repository = JsonSerializer.Deserialize<GiteaRepository>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (repository != null)
                {
                    Console.WriteLine($"✅ Repository erfolgreich erstellt: {repository.Name}");
                    Console.WriteLine($"  Full Name: {repository.Full_Name}");
                    Console.WriteLine($"  Owner: {repository.Owner?.Login}");

                    // Erstelle .gitattributes für LFS
                    await CreateGitAttributesFileAsync(repository.Owner?.Login!, repository.Name);
                }

                return repository;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Repository-Erstellung Exception: {ex.Message}");
                Console.WriteLine($"  StackTrace: {ex.StackTrace}");
                return null;
            }
        }

        private async Task CreateGitAttributesFileAsync(string owner, string repoName)
        {
            try
            {
                var gitAttributesContent = @"# Git LFS Configuration for SPS Project Files
# Behandle diese Dateitypen als Binärdateien und aktiviere LFS

# SPS Project Files
*.ap17 filter=lfs diff=lfs merge=lfs -text binary
*.s7p filter=lfs diff=lfs merge=lfs -text binary  
*.zap filter=lfs diff=lfs merge=lfs -text binary
*.tsproj filter=lfs diff=lfs merge=lfs -text binary
*.pro filter=lfs diff=lfs merge=lfs -text binary
*.acd filter=lfs diff=lfs merge=lfs -text binary

# Large Binary Files
*.zip filter=lfs diff=lfs merge=lfs -text binary
*.rar filter=lfs diff=lfs merge=lfs -text binary
*.7z filter=lfs diff=lfs merge=lfs -text binary
*.pdf filter=lfs diff=lfs merge=lfs -text binary

# Media Files  
*.jpg filter=lfs diff=lfs merge=lfs -text binary
*.jpeg filter=lfs diff=lfs merge=lfs -text binary
*.png filter=lfs diff=lfs merge=lfs -text binary
*.mp4 filter=lfs diff=lfs merge=lfs -text binary
*.avi filter=lfs diff=lfs merge=lfs -text binary
*.mp3 filter=lfs diff=lfs merge=lfs -text binary

# Office Documents
*.doc filter=lfs diff=lfs merge=lfs -text binary
*.docx filter=lfs diff=lfs merge=lfs -text binary
*.xls filter=lfs diff=lfs merge=lfs -text binary
*.xlsx filter=lfs diff=lfs merge=lfs -text binary
";

                var fileBytes = Encoding.UTF8.GetBytes(gitAttributesContent);
                await UploadFileAsync(owner, repoName, ".gitattributes", fileBytes, "🔧 Git LFS Configuration hinzugefügt");
                Console.WriteLine($"✅ .gitattributes für LFS erstellt");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Warnung: .gitattributes konnte nicht erstellt werden: {ex.Message}");
            }
        }

        // ===== CONTENT METHODS =====

        public async Task<List<RepositoryContent>?> GetRepositoryContentsAsync(string owner, string repoName, string path = "")
        {
            var encodedPath = Uri.EscapeDataString(path);
            var url = string.IsNullOrEmpty(path)
                ? $"repos/{owner}/{repoName}/contents"
                : $"repos/{owner}/{repoName}/contents/{encodedPath}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Fehler beim Abrufen des Inhalts: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var contents = JsonSerializer.Deserialize<List<RepositoryContent>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // URL-Fix für localhost -> 10.197.240.9 (falls nötig)
            if (contents != null)
            {
                foreach (var item in contents)
                {
                    if (!string.IsNullOrEmpty(item.Download_Url))
                    {
                        item.Download_Url = item.Download_Url.Replace("localhost", "10.197.240.9");
                    }
                }
            }

            return contents;
        }

        // ===== TEXT FILE METHODS (nur für kleine Textdateien!) =====

        public async Task<string?> GetFileContentAsync(string owner, string repoName, string filePath)
        {
            try
            {
                if (IsBinaryFile(filePath))
                {
                    Console.WriteLine($"⚠️ Warnung: {filePath} ist eine Binärdatei. Verwende DownloadFileStreamAsync() stattdessen.");
                    return null;
                }

                var encodedPath = Uri.EscapeDataString(filePath);
                var url = $"repos/{owner}/{repoName}/contents/{encodedPath}";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Fehler beim Abrufen der Datei: {response.StatusCode}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var fileData = JsonSerializer.Deserialize<RepositoryContent>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (!string.IsNullOrEmpty(fileData?.Content))
                {
                    var bytes = Convert.FromBase64String(fileData.Content);
                    return Encoding.UTF8.GetString(bytes);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Abrufen der Datei: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> GetFileContentAtCommitAsync(string owner, string repoName, string filePath, string commitSha)
        {
            try
            {
                if (IsBinaryFile(filePath))
                {
                    Console.WriteLine($"⚠️ Warnung: {filePath} ist eine Binärdatei. Verwende DownloadFileStreamAsync() stattdessen.");
                    return null;
                }

                Console.WriteLine($"🔍 GET FILE AT COMMIT DEBUG:");
                Console.WriteLine($"  Owner: {owner}");
                Console.WriteLine($"  RepoName: {repoName}");
                Console.WriteLine($"  FilePath: {filePath}");
                Console.WriteLine($"  CommitSha: {commitSha}");

                var encodedPath = Uri.EscapeDataString(filePath);
                var url = $"repos/{owner}/{repoName}/contents/{encodedPath}?ref={commitSha}";

                Console.WriteLine($"  API-URL: {url}");

                var response = await _httpClient.GetAsync(url);

                Console.WriteLine($"  Response Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"  Error Response: {errorContent}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var fileData = JsonSerializer.Deserialize<RepositoryContent>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (!string.IsNullOrEmpty(fileData?.Content))
                {
                    var bytes = Convert.FromBase64String(fileData.Content);
                    var content = Encoding.UTF8.GetString(bytes);
                    Console.WriteLine($"  Content Length: {content.Length}");
                    return content;
                }

                Console.WriteLine($"  No content found");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Get File At Commit Exception: {ex.Message}");
                return null;
            }
        }

        // ===== NEW: LFS DOWNLOAD METHODS =====

        /// <summary>
        /// 📥 Lädt eine Datei performant als Stream herunter (LFS-kompatibel)
        /// </summary>
        public async Task<Stream?> DownloadFileStreamAsync(string owner, string repoName, string filePath, string? commitSha = null)
        {
            try
            {
                Console.WriteLine($"🔍 DOWNLOAD FILE STREAM:");
                Console.WriteLine($"  Owner: {owner}");
                Console.WriteLine($"  RepoName: {repoName}");
                Console.WriteLine($"  FilePath: {filePath}");
                Console.WriteLine($"  CommitSha: {commitSha ?? "latest"}");

                // 1. Metadaten abfragen um Download-URL zu bekommen
                var encodedPath = Uri.EscapeDataString(filePath);
                var url = string.IsNullOrEmpty(commitSha)
                    ? $"repos/{owner}/{repoName}/contents/{encodedPath}"
                    : $"repos/{owner}/{repoName}/contents/{encodedPath}?ref={commitSha}";

                Console.WriteLine($"  Metadata URL: {url}");

                var metaResponse = await _httpClient.GetAsync(url);

                if (!metaResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"  Metadata Error: {metaResponse.StatusCode}");
                    return null;
                }

                var json = await metaResponse.Content.ReadAsStringAsync();
                var fileData = JsonSerializer.Deserialize<RepositoryContent>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (fileData == null)
                {
                    Console.WriteLine($"  No file data found");
                    return null;
                }

                // 2. Prüfe ob LFS Download-URL vorhanden ist
                if (!string.IsNullOrEmpty(fileData.Download_Url))
                {
                    Console.WriteLine($"  Using LFS Download URL: {fileData.Download_Url}");

                    // Direkte LFS-URL für große Dateien
                    var fileResponse = await _httpClient.GetAsync(fileData.Download_Url);

                    if (fileResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"✅ LFS Stream Download successful");
                        return await fileResponse.Content.ReadAsStreamAsync();
                    }
                }

                // 3. Fallback: Base64 Content für kleine Dateien
                if (!string.IsNullOrEmpty(fileData.Content))
                {
                    Console.WriteLine($"  Using Base64 Content (fallback)");
                    var bytes = Convert.FromBase64String(fileData.Content);
                    Console.WriteLine($"  File Size: {bytes.Length} bytes");
                    return new MemoryStream(bytes);
                }

                Console.WriteLine($"  No download method available");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Download Stream Exception: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 📥 Legacy-Methode für Backwards-Kompatibilität (nur für kleine Dateien empfohlen)
        /// </summary>
        public async Task<byte[]?> GetFileContentBytesAtCommitAsync(string owner, string repoName, string filePath, string commitSha)
        {
            try
            {
                using var stream = await DownloadFileStreamAsync(owner, repoName, filePath, commitSha);
                if (stream == null) return null;

                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Get File Bytes At Commit Exception: {ex.Message}");
                return null;
            }
        }

        // ===== UPLOAD METHODS =====

        /// <summary>
        /// 📤 Legacy Upload-Methode (nur für kleine Dateien empfohlen)
        /// </summary>
        public async Task<bool> UploadFileAsync(string owner, string repoName, string filePath, byte[] fileContent, string commitMessage)
        {
            try
            {
                if (IsLargeFile(fileContent.Length))
                {
                    Console.WriteLine($"⚠️ Warnung: Datei {filePath} ist {fileContent.Length / (1024 * 1024)}MB groß.");
                    Console.WriteLine($"    Verwende UploadFileViaLfsAsync() für bessere Performance bei großen Dateien.");
                }

                Console.WriteLine($"🚀 UPLOAD DEBUG:");
                Console.WriteLine($"  Owner: {owner}");
                Console.WriteLine($"  RepoName: {repoName}");
                Console.WriteLine($"  FilePath: {filePath}");
                Console.WriteLine($"  FileSize: {fileContent.Length} bytes");
                Console.WriteLine($"  CommitMessage: {commitMessage}");

                var encodedPath = Uri.EscapeDataString(filePath);
                var url = $"repos/{owner}/{repoName}/contents/{encodedPath}";

                Console.WriteLine($"  API-URL: {url}");
                Console.WriteLine($"  Encoded Path: {encodedPath}");

                var uploadData = new
                {
                    message = commitMessage,
                    content = Convert.ToBase64String(fileContent)
                };

                var jsonContent = JsonSerializer.Serialize(uploadData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                Console.WriteLine($"  JSON Length: {jsonContent.Length}");
                Console.WriteLine($"  Base64 Length: {Convert.ToBase64String(fileContent).Length}");

                var response = await _httpClient.PostAsync(url, content);

                Console.WriteLine($"  Response Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"  Response Content: {responseContent}");
                    Console.WriteLine($"❌ Upload fehlgeschlagen: {response.StatusCode}");
                    return false;
                }

                Console.WriteLine($"✅ Upload erfolgreich!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Upload Exception: {ex.Message}");
                Console.WriteLine($"  StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 📤 Performanter Upload für große Dateien via Git LFS
        /// </summary>
        public async Task<bool> UploadFileViaLfsAsync(string owner, string repoName, string filePath, Stream fileStream, string commitMessage)
        {
            var repoUrl = GetGiteaRepoUrl(owner, repoName);
            var tempClonePath = Path.Combine(Path.GetTempPath(), $"gitea_upload_{Guid.NewGuid():N}");

            try
            {
                Console.WriteLine($"🚀 LFS UPLOAD DEBUG:");
                Console.WriteLine($"  Owner: {owner}");
                Console.WriteLine($"  RepoName: {repoName}");
                Console.WriteLine($"  FilePath: {filePath}");
                Console.WriteLine($"  FileSize: {fileStream.Length} bytes");
                Console.WriteLine($"  RepoUrl: {repoUrl}");
                Console.WriteLine($"  TempPath: {tempClonePath}");

                var cloneOptions = new CloneOptions();
                cloneOptions.FetchOptions.CredentialsProvider = (_url, _user, _cred) =>
                    new UsernamePasswordCredentials
                    {
                        Username = "token",
                        Password = _config["Gitea:Token"]
                    };

                Console.WriteLine($"  Klone Repository...");
                Repository.Clone(repoUrl, tempClonePath, cloneOptions);

                using (var repo = new Repository(tempClonePath))
                {
                    var fullFilePath = Path.Combine(tempClonePath, filePath);
                    var directory = Path.GetDirectoryName(fullFilePath);

                    if (!string.IsNullOrEmpty(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    Console.WriteLine($"  Schreibe Datei: {fullFilePath}");
                    using (var fs = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
                    {
                        await fileStream.CopyToAsync(fs);
                    }

                    Console.WriteLine($"  Stage Datei...");
                    Commands.Stage(repo, filePath);

                    var status = repo.RetrieveStatus();
                    if (!status.IsDirty)
                    {
                        Console.WriteLine($"  Keine Änderungen zu committen");
                        return true; // Datei ist identisch
                    }

                    Console.WriteLine($"  Erstelle Commit...");
                    var author = new Signature("MECWeb", "webapp@schaeffler.com", DateTimeOffset.Now);
                    repo.Commit(commitMessage, author, author);

                    Console.WriteLine($"  Push Änderungen...");
                    var pushOptions = new PushOptions
                    {
                        CredentialsProvider = cloneOptions.FetchOptions.CredentialsProvider
                    };
                    repo.Network.Push(repo.Head, pushOptions);

                    Console.WriteLine($"✅ LFS Upload erfolgreich!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                // VERBESSERTES FEHLER-LOGGING
                Console.WriteLine("===================================================");
                Console.WriteLine("❌ SCHWERWIEGENDER FEHLER BEIM LFS UPLOAD ❌");
                Console.WriteLine($"  Fehlermeldung: {ex.Message}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"  --- Innere Exception (die wahre Ursache) ---");
                    Console.WriteLine($"  Typ: {ex.InnerException.GetType().Name}");
                    Console.WriteLine($"  Meldung: {ex.InnerException.Message}");
                    Console.WriteLine($"  --- Ende Innere Exception ---");
                }

                Console.WriteLine($"  StackTrace: {ex.StackTrace}");
                Console.WriteLine("===================================================");
                return false;
            }
            finally
            {
                if (Directory.Exists(tempClonePath))
                {
                    try
                    {
                        // Workaround für Berechtigungsprobleme unter Windows
                        var dirInfo = new DirectoryInfo(tempClonePath);
                        foreach (var file in dirInfo.GetFiles("*", SearchOption.AllDirectories))
                        {
                            file.Attributes = FileAttributes.Normal;
                        }
                        dirInfo.Delete(true);
                        Console.WriteLine($"  Temporärer Ordner bereinigt");
                    }
                    catch (Exception cleanupEx)
                    {
                        Console.WriteLine($"⚠️ Warnung: Temporärer Ordner konnte nicht gelöscht werden: {cleanupEx.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// 📤 Convenience-Methode für byte[] Upload via LFS
        /// </summary>
        public async Task<bool> UploadFileViaLfsAsync(string owner, string repoName, string filePath, byte[] fileContent, string commitMessage)
        {
            using var stream = new MemoryStream(fileContent);
            return await UploadFileViaLfsAsync(owner, repoName, filePath, stream, commitMessage);
        }

        // ===== GIT HISTORY METHODS =====

        public async Task<List<GitCommit>?> GetFileCommitsAsync(string owner, string repoName, string filePath)
        {
            try
            {
                Console.WriteLine($"🔍 GET FILE COMMITS DEBUG:");
                Console.WriteLine($"  Owner: {owner}");
                Console.WriteLine($"  RepoName: {repoName}");
                Console.WriteLine($"  FilePath: {filePath}");

                var encodedPath = Uri.EscapeDataString(filePath);
                var url = $"repos/{owner}/{repoName}/commits?path={encodedPath}";

                Console.WriteLine($"  API-URL: {url}");

                var response = await _httpClient.GetAsync(url);

                Console.WriteLine($"  Response Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"  Error Response: {errorContent}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"  Response Length: {json.Length}");

                var commits = JsonSerializer.Deserialize<List<GitCommit>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                Console.WriteLine($"  Found {commits?.Count ?? 0} commits");
                return commits;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Get File Commits Exception: {ex.Message}");
                return null;
            }
        }

        public async Task<GitCommitDetail?> GetCommitDetailAsync(string owner, string repoName, string commitSha)
        {
            try
            {
                Console.WriteLine($"🔍 GET COMMIT DETAIL DEBUG:");
                Console.WriteLine($"  Owner: {owner}");
                Console.WriteLine($"  RepoName: {repoName}");
                Console.WriteLine($"  CommitSha: {commitSha}");

                var url = $"repos/{owner}/{repoName}/commits/{commitSha}";
                Console.WriteLine($"  API-URL: {url}");

                var response = await _httpClient.GetAsync(url);
                Console.WriteLine($"  Response Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"  Error Response: {errorContent}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var commitDetail = JsonSerializer.Deserialize<GitCommitDetail>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                Console.WriteLine($"  Commit loaded: {commitDetail?.Commit?.Message}");
                return commitDetail;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Get Commit Detail Exception: {ex.Message}");
                return null;
            }
        }

        public async Task<List<GitCommit>?> GetRepositoryCommitsAsync(string owner, string repoName, int page = 1, int limit = 20)
        {
            try
            {
                var url = $"repos/{owner}/{repoName}/commits?page={page}&limit={limit}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Fehler beim Abrufen der Commits: {response.StatusCode}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var commits = JsonSerializer.Deserialize<List<GitCommit>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return commits;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Get Repository Commits Exception: {ex.Message}");
                return null;
            }
        }

        // ===== RELEASES =====

        public async Task<List<Release>?> GetReleasesAsync(string owner, string repoName)
        {
            var response = await _httpClient.GetAsync($"repos/{owner}/{repoName}/releases");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Fehler beim Abrufen der Releases: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var releases = JsonSerializer.Deserialize<List<Release>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return releases;
        }

        // ===== FOLDER CREATION =====

        /// <summary>
        /// 📁 Ordner erstellen durch Upload einer README.md mit Beschreibung
        /// </summary>
        public async Task<bool> CreateFolderAsync(string owner, string repoName, string folderPath, string description)
        {
            try
            {
                Console.WriteLine($"🚀 CREATE FOLDER DEBUG:");
                Console.WriteLine($"  Owner: {owner}");
                Console.WriteLine($"  RepoName: {repoName}");
                Console.WriteLine($"  FolderPath: {folderPath}");
                Console.WriteLine($"  Description: {description}");

                // Erstelle README.md Pfad im Ordner
                var readmePath = $"{folderPath.TrimEnd('/')}/README.md";

                // Erstelle README-Inhalt
                var readmeContent = $"# {folderPath}\n\n{description}\n\n---\n*Dieser Ordner wurde automatisch erstellt.*";
                var fileBytes = Encoding.UTF8.GetBytes(readmeContent);

                // Upload über bestehende UploadFileAsync Methode (README ist klein)
                var commitMessage = $"📁 Ordner '{folderPath}' erstellt mit Beschreibung";

                return await UploadFileAsync(owner, repoName, readmePath, fileBytes, commitMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Create Folder Exception: {ex.Message}");
                return false;
            }
        }
    }
}

