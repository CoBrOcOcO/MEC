using MECWeb.DbModels.Project;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MECWeb.DbModels.User
{
    /// <summary>
    /// User-Project-Zuordnung in der Datenbank
    /// </summary>
    [Table("mec_user_project")]
    public class DbUserProject
    {
        public Guid DbUserId { get; set; }
        public DbUser User { get; set; }

        public Guid DbProjectId { get; set; }
        public DbProject Project { get; set; }
    }

}