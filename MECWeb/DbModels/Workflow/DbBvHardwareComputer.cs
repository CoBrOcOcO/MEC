using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MECWeb.DbModels.Workflow
{
    [Table("mec_bv_hardware_computer")]
    public class DbBvHardwareComputer
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid WorkflowId { get; set; }
        [ForeignKey(nameof(WorkflowId))]
        public DbWorkflow Workflow { get; set; } = null!;
        [MaxLength(500)]
        public string? Description { get; set; } 
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string SerialNumber { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? ContactPerson { get; set; }
        [MaxLength(50)]
        public string? PcType { get; set; }
        [MaxLength(200)]
        public string? PcTypeDetails { get; set; }
        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime LastChange { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public ICollection<DbBvHardwareComponent> HardwareComponents { get; set; } = new List<DbBvHardwareComponent>();
    }
}