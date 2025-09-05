using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MECWeb.DbModels.Workflow
{
    [Table("mec_hardware_field")]
    public class DbHardwareField
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid HardwareComputerId { get; set; }

        [ForeignKey(nameof(HardwareComputerId))]
        public DbHardwareComputer HardwareComputer { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string FieldName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FieldType { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? FieldValue { get; set; }

        [MaxLength(500)]
        public string? DefaultValue { get; set; }

        [MaxLength(500)]
        public string? ValidationRules { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsRequired { get; set; } = false;

        public bool IsReadOnly { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime LastChange { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public bool IsActive { get; set; } = true;
    }
}