[Table("mec_bv_hardware_computer")]
public class DbBvHardwareComputer
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid WorkflowId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? SerialNumber { get; set; }

    [MaxLength(100)]
    public string? ContactPerson { get; set; }

    [MaxLength(50)]
    public string? PcType { get; set; }

    [MaxLength(500)]
    public string? PcRemarks { get; set; }  // Neues Feld für PC-Bemerkungen

    [MaxLength(500)]
    public string? RequirementsRemarks { get; set; }  // Neues Feld für Anforderungs-Bemerkungen

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public DateTime CreationDate { get; set; }

    [Required]
    public DateTime LastChange { get; set; }

    public bool IsActive { get; set; } = true;
}