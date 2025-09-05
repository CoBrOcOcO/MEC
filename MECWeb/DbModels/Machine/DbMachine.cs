using MECWeb.DbModels.Project;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MECWeb.DbModels.Machine
{
    /// <summary>
    /// Projekt
    /// </summary>
    [Table("mec_machine")]
    public class DbMachine
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public DbProject Project { get; set; } = null!;




        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string SerialNumber { get; set; } = string.Empty;





    }
}
