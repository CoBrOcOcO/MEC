using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MECWeb.DbModels.Workflow
{
    [Table("mec_installation_configuration")]
    public class DbInstallationConfiguration
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid WorkflowId { get; set; }

        [ForeignKey(nameof(WorkflowId))]
        public DbWorkflow Workflow { get; set; } = null!;

        [MaxLength(17)]
        public string? MacAddress { get; set; }

        [MaxLength(15)]
        public string? IpAddress { get; set; }

        [MaxLength(100)]
        public string? ComputerName { get; set; }

        public bool IsNetworkConfigured { get; set; } = false;

        [MaxLength(500)]
        public string? NetworkNotes { get; set; }

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime LastChange { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public bool IsActive { get; set; } = true;
    }
}