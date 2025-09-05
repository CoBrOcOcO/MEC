using MECWeb.DbModels;
using MECWeb.DbModels.User;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Graph;

using Microsoft.Identity.Web;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MECWeb.Services
{
    /// <summary>
    /// Service class for user profile information. Graph ML API is used to retrieve user profile data.
    /// </summary>
    public class UserProfileService
    {
        private readonly GraphServiceClient _graphClient;
        private readonly MicrosoftIdentityConsentAndConditionalAccessHandler _consentHandler;
        private readonly ApplicationDbContext _dbContext;


        /// <summary>
        /// User Daten des aktuell angemeldeten Benutzers.
        /// </summary>
        public DbUser User { get; private set; } = new DbUser();


        /// <summary>
        /// Konstruktor für den UserProfileService.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="consentHandler"></param>
        public UserProfileService(GraphServiceClient graphClient, MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler, ApplicationDbContext dbContext)
        {
            _graphClient = graphClient;
            _consentHandler = consentHandler;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Lädt das Benutzerprofil asynchron und aktualisiert die Eigenschaften des DbUser-Objekts.
        /// </summary>
        /// <returns></returns>
        public async Task LoadUserProfileAsync()
        {
            try
            {
                this.User = new DbUser(); // Initialisiere User-Objekt

                // Userdaten abrufen
                var user = await _graphClient.Me.Request().GetAsync();
                if (user == null)
                {
                    throw new ServiceException(new Error
                    {
                        Code = "UserNotFound",
                        Message = "User could not found in GraphMl"
                    });
                }

                this.User = DbUser.ParseGraphMLUser(user);

                // Benutzerfoto abrufen
                try
                {
                    var photoStream = await _graphClient.Me.Photo.Content.Request().GetAsync(); // Use the Request() method before calling GetAsync()
                    if (photoStream != null)
                    {
                        using var ms = new MemoryStream();
                        await photoStream.CopyToAsync(ms);
                        var bytes = ms.ToArray();
                        this.User.UserPhoto = $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";
                    }
                }
                catch (ServiceException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Kein Foto vorhanden, ignoriere
                    this.User.UserPhoto = null;
                }


                // Prüfe, ob der User bereits in der Datenbank existiert, andernfalls füge ihn hinzu oder aktualisiere ihn
                var existingUser = await _dbContext.User.FindAsync(this.User.UId);
                if (existingUser == null)
                {
                    _dbContext.User.Add(this.User); // Neuer User
                }
                else
                {
                    _dbContext.Entry(existingUser).CurrentValues.SetValues(this.User); // Update bestehender User
                }
                await _dbContext.SaveChangesAsync();
                await Task.CompletedTask;
            }
            catch (ServiceException ex)
            {
                _consentHandler.HandleException(ex);
            }
        }
    }
}
