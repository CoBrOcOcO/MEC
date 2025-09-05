using MECWeb.DbModels.Machine;
using MECWeb.DbModels.Project;
using MECWeb.DbModels.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MECWeb.DbModels.Translate
{

    /// <summary>
    /// Übersetzung
    /// </summary>
    [Table("mec_translate_translation")]
    public class DbTranslateTranslation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid EntryId { get; set; }

        [ForeignKey(nameof(EntryId))]
        public DbTranslateEntry Entry { get; set; } = null!;




        [Required]
        [MaxLength(10)]
        public string LanguageCode { get; set; } = string.Empty; // z.B. "de", "en", "fr"

        [MaxLength(200)]
        public string? Value { get; set; }
    }
}
