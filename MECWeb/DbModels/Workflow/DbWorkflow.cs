using MECWeb.DbModels.Project;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MECWeb.DbModels.Workflow
{
    public enum WorkflowType
    {
        BDR = 0,  // BDR PC
        BV = 1    // ´BV PC
    }

    
    public enum WorkflowStatus
    {
        Created = 0,
        HardwareInProgress = 1,          // Hardware is being 
        HardwareCompleted = 2,           // Hardware is ready for Purchase
        SoftwareInProgress = 3,          // Software is being installed
        SoftwareCompleted = 4,           //Software is ready for Purchase
        Completed = 5,                   // Ready for purchase
        ForwardedToInstallation = 6,     // Forwarded to installation by purchase
        InstallationInProgress = 7,      // Installation is working on it
        InstallationCompleted = 8,       // Installation completed 
        Archived = 9
    }

    [Table("mec_workflow")]
    public class DbWorkflow
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public DbProject Project { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public WorkflowType WorkflowType { get; set; }

        [Required]
        public WorkflowStatus Status { get; set; } = WorkflowStatus.Created;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime LastChange { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public ICollection<DbHardwareComputer> HardwareComputers { get; set; } = [];
    }
}