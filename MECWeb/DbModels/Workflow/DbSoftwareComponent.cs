using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MECWeb.DbModels.Workflow
{
    [Table("mec_software_component")]
    public class DbSoftwareComponent
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid HardwareComputerId { get; set; }

        [ForeignKey(nameof(HardwareComputerId))]
        public DbHardwareComputer HardwareComputer { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Version { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? ComponentType { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? ConfigurationPath { get; set; }

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime LastChange { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public bool IsConfigured { get; set; } = false;

        public ICollection<DbConfigurationEntry> ConfigurationEntries { get; set; } = new List<DbConfigurationEntry>();
    }
}