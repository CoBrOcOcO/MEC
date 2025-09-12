using MECWeb.DbModels.User;
using MECWeb.DbModels.Workflow;
using System.ComponentModel.DataAnnotations;

namespace MECWeb.DbModels.Project
{
    public class DbProject
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ProjectNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public DateTime LastChange { get; set; } = DateTime.UtcNow;

        // ✅ NEUE GIT-INTEGRATION PROPERTIES
        /// <summary>
        /// Git-Integration für dieses Projekt aktiviert
        /// </summary>
        public bool GitEnabled { get; set; } = false;

        /// <summary>
        /// Name des Git-Repository (meist identisch mit ProjectNumber)
        /// </summary>
        [StringLength(200)]
        public string? GitRepositoryName { get; set; }

        /// <summary>
        /// Git-Repository URL für externe Links
        /// </summary>
        [StringLength(500)]
        public string? GitRepositoryUrl { get; set; }

        /// <summary>
        /// Git-Repository Besitzer/Organization
        /// </summary>
        [StringLength(100)]
        public string? GitOwner { get; set; } = "barwiaex";

        /// <summary>
        /// Git-Repository Beschreibung
        /// </summary>
        [StringLength(1000)]
        public string? GitDescription { get; set; }

        /// <summary>
        /// Timestamp wann Git-Repository erstellt wurde
        /// </summary>
        public DateTime? GitCreatedAt { get; set; }

        /// <summary>
        /// Private/Public Repository Einstellung
        /// </summary>
        public bool GitIsPrivate { get; set; } = false;
        // ✅ KORRIGIERT: DbWorkflow statt ProjectWorkflow verwenden
        public ICollection<DbWorkflow> Workflows { get; set; } = new List<DbWorkflow>();

        
        public ICollection<DbUserProject> UserProjects { get; set; } = new List<DbUserProject>();
        public ICollection<DbUserProjectFavorite> UserProjectFavorites { get; set; } = new List<DbUserProjectFavorite>();

        // Bestehende Properties (falls vorhanden)...
        // Weitere Properties hier hinzufügen je nach deinem bestehenden Model
    }
}