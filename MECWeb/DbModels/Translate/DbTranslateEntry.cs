using MECWeb.DbModels.Machine;
using MECWeb.DbModels.Project;
using MECWeb.DbModels.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;

namespace MECWeb.DbModels.Translate
{

    /// <summary>
    /// Übersetzungseintrag einer Übersetzung
    /// </summary>
    [Table("mec_translate_entry")]
    public class DbTranslateEntry
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid DictionaryId { get; set; }

        [ForeignKey(nameof(DictionaryId))]
        public DbTranslateDictionary Dictionary { get; set; } = null!;




        [Required]
        [MaxLength(200)]
        public string Key { get; set; } = string.Empty; // z.B. "StartButton"



        public ICollection<DbTranslateTranslation> Translations { get; set; } = new List<DbTranslateTranslation>();





    }
}
