using MECWeb.DbModels.Project;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MECWeb.DbModels.User
{
    /// <summary>
    /// User-Project-Favoriten Zuordnung in der Datenbank
    /// </summary>
    [Table("mec_user_project_favorite")]
    public class DbUserProjectFavorite
    {
        public Guid DbUserId { get; set; }
        public DbUser User { get; set; }

        public Guid DbProjectId { get; set; }
        public DbProject Project { get; set; }
    }

}