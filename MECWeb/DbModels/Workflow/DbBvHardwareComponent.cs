using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MECWeb.DbModels.Workflow
{
    [Table("mec_bv_hardware_component")]
    public class DbBvHardwareComponent
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid BvHardwareComputerId { get; set; }

        [ForeignKey(nameof(BvHardwareComputerId))]
        public DbBvHardwareComputer BvHardwareComputer { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string ComponentName { get; set; } = string.Empty;

        [Required]
        public bool IsSelected { get; set; } = false;

        [MaxLength(100)]
        public string? Quantity { get; set; }

        [MaxLength(200)]
        public string? ComponentType { get; set; }

        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}