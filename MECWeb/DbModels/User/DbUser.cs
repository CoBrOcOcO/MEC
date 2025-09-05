using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MECWeb.DbModels.User
{

    /// <summary>
    /// User in der Datenbank
    /// </summary>
    [Table("mec_user")]
    public class DbUser
    {
        [Key]
        public Guid UId { get; set; }




        [Required]
        [MaxLength(50)]
        public string GivenName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string SurName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string EMail { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Department { get; set; }

        public string? UserPhoto { get; set; }



        /// <summary>
        /// Favoriten Projekte des Users
        /// </summary>
        public ICollection<DbUserProjectFavorite> UserProjectFavorites { get; set; } = new List<DbUserProjectFavorite>();

        /// <summary>
        /// Projekte des Users
        /// </summary>
        public ICollection<DbUserProject> UserProjects { get; set; } = new List<DbUserProject>();




        /// <summary>
        /// Parse einen Microsoft Graph User in ein DbUser Objekt
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static DbUser ParseGraphMLUser(Microsoft.Graph.User user)
        {
            return new DbUser
            {
                UId = new Guid(user.Id),
                GivenName = user.GivenName ?? string.Empty,
                SurName = user.Surname ?? string.Empty,
                DisplayName = user.DisplayName ?? string.Empty,
                EMail = user.Mail ?? string.Empty,
                Department = user.Department ?? string.Empty,
                UserPhoto = null // UserPhoto wird separat behandelt
            };
        }




    }
}
