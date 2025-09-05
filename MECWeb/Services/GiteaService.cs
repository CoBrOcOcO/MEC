using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MECWeb.Models.Gitea;

namespace MECWeb.Services
{
    public class GiteaService
    {
        private readonly HttpClient _httpClient;

        public GiteaService(HttpClient httpClient, IConfiguration config)
        {
            var token = config["Gitea:Token"];
            var baseUrl = config["Gitea:ApiUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Gitea:ApiUrl ist nicht gesetzt.");

            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Gitea:Token ist nicht gesetzt.");

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
        }

        // ===== REPOSITORY METHODS =====

        public async Task<Repository?> GetRepositoryAsync(string owner, string repoName)
        {
            var encodedRepoName = Uri.EscapeDataString(repoName);
            var response = await _httpClient.GetAsync($"repos/{owner}/{encodedRepoName}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Fehler beim Abrufen des Repositories: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var repo = JsonSerializer.Deserialize<Repository>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return repo;
        }

        public async Task<List<Repository>?> GetUserRepositoriesAsync()
        {
            var response = await _httpClient.GetAsync("user/repos");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Fehler beim Abrufen der Repositories: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var repos = JsonSerializer.Deserialize<List<Repository>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return repos;
        }

        public async Task<Repository?> CreateRepositoryAsync(string name, string description = "")
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

                var repository = JsonSerializer.Deserialize<Repository>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (repository != null)
                {
                    Console.WriteLine($"✅ Repository erfolgreich erstellt: {repository.Name}");
                    Console.WriteLine($"  Full Name: {repository.Full_Name}");
                    Console.WriteLine($"  Owner: {repository.Owner?.Login}");
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

        public async Task<string?> GetFileContentAsync(string owner, string repoName, string filePath)
        {
            try
            {
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

        // ===== UPLOAD METHODS =====

        public async Task<bool> UploadFileAsync(string owner, string repoName, string filePath, byte[] fileContent, string commitMessage)
        {
            try
            {
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

        // ===== GIT HISTORY METHODS =====

        /// <summary>
        /// 📚 Git-Commits für eine spezifische Datei abrufen
        /// </summary>
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

        /// <summary>
        /// 📄 Datei-Inhalt zu einem bestimmten Commit abrufen
        /// </summary>
        public async Task<string?> GetFileContentAtCommitAsync(string owner, string repoName, string filePath, string commitSha)
        {
            try
            {
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

        /// <summary>
        /// 📥 Datei zu einem bestimmten Commit als Bytes abrufen (für Download)
        /// </summary>
        public async Task<byte[]?> GetFileContentBytesAtCommitAsync(string owner, string repoName, string filePath, string commitSha)
        {
            try
            {
                Console.WriteLine($"🔍 GET FILE BYTES AT COMMIT DEBUG:");
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
                    Console.WriteLine($"  File Size: {bytes.Length} bytes");
                    return bytes;
                }

                Console.WriteLine($"  No content found");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Get File Bytes At Commit Exception: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 🔍 Einzelnen Commit-Details abrufen
        /// </summary>
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

        /// <summary>
        /// 📊 Repository-Commits abrufen (allgemeine Historie)
        /// </summary>
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

        // ===== FOLDER CREATION (NEW) =====

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

                // Upload über bestehende UploadFileAsync Methode
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