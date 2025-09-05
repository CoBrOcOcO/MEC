using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MECWeb.DbModels.Workflow
{
    [Table("mec_configuration_entry")]
    public class DbConfigurationEntry
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid SoftwareComponentId { get; set; }

        [ForeignKey(nameof(SoftwareComponentId))]
        public DbSoftwareComponent SoftwareComponent { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Key { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Value { get; set; }

        [MaxLength(200)]
        public string? Category { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime LastChange { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public DbBvHardwareComputer BvHardwareComputer { get; set; } = null!;
    }
}