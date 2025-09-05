using MECWeb.DbModels.Machine;
using MECWeb.DbModels.Project;
using MECWeb.DbModels.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MECWeb.DbModels.Translate
{

    /// <summary>
    /// Wörterbuch für Übersetzungen in der Datenbank
    /// </summary>
    [Table("mec_translate_dictionary")]
    public class DbTranslateDictionary
    {
        [Key]
        public Guid Id { get; set; }




        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime LastChange { get; set; }

        [Required]
        public DateTime CreationDate { get; private set; }



        public ICollection<DbTranslateEntry> Entries { get; set; } = new List<DbTranslateEntry>();





    }
}
