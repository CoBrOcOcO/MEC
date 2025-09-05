using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MECWeb.DbModels.Workflow
{
    public enum HardwareType
    {
        MIC = 0,
        BDR230V15 = 1,
        BDR24V15 = 2,
        MPG215 = 3,
        MPG210 = 4,
        SiemensPanelPC = 5,
        SiemensBoxPC = 6,
        OPCSlotIn = 7,
        TQBoxLowCost = 8
    }

    [Table("mec_hardware_computer")]
    public class DbHardwareComputer
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid WorkflowId { get; set; }

        [ForeignKey(nameof(WorkflowId))]
        public DbWorkflow Workflow { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string SerialNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Location { get; set; }

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        [MaxLength(200)]
        public string? HardwareSpecs { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime LastChange { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<DbSoftwareComponent> SoftwareComponents { get; set; } = new List<DbSoftwareComponent>();

        public ICollection<DbHardwareField> HardwareFields { get; set; } = new List<DbHardwareField>();
    }
}