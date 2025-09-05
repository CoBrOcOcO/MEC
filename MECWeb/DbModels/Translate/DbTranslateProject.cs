using MECWeb.DbModels.Machine;
using MECWeb.DbModels.Project;
using MECWeb.DbModels.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MECWeb.DbModels.Translate
{
    public enum TranslationServiceType
    {
        Google = 0,
        Microsoft = 1,
    }

    public enum TranslationFileType
    {
        TIA = 0,
        STEP7 = 1,
        CUSTOM = 2,
    }

    /// <summary>
    /// Übersetzungsprojekt in der Datenbank
    /// </summary>
    [Table("mec_translate_project")]
    public class DbTranslateProject
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
        public TranslationFileType TranslationFileType { get; set; } = TranslationFileType.CUSTOM;

        [Required]
        public TranslationServiceType TranslationService { get; set; } = TranslationServiceType.Microsoft;

        [Required]
        public int TranslatedItemsCount { get; set; } = 0;

        [Required]
        public int TotalItemsCount { get; set; } = 0;

        [Required]
        public DateTime CreationDate { get; private set; }








    }
}
