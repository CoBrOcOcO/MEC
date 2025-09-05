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
    /// Base class for UserClaims component.
    /// Retrieves claims present in the ID Token issued by Azure AD.
    /// </summary>
    public class AuthService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        private AuthenticationState? _authState;


        /// <summary>
        /// Konstruktor für die AuthService-Klasse.
        /// </summary>
        /// <param name="authenticationStateProvider"></param>
        /// <param name="graphClient"></param>
        /// <param name="microsoftIdentityConsentAndConditionalAccessHandler"></param>
        public AuthService(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }


        /// <summary>
        /// Retrieves user claims for the signed-in user.
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            // Gets an AuthenticationState that describes the current user.
            _authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        }

        /// <summary>
        /// Gibt zurück, ob der aktuelle Benutzer authentifiziert ist.
        /// </summary>
        public bool IsUserAuthenticated()
        {
            if (_authState == null)
                return false;
            else
                return _authState.User.Identity?.IsAuthenticated ?? false;
        }

        /// <summary>
        /// Gibt den aktuellen ClaimsPrincipal zurück.
        /// </summary>
        public ClaimsPrincipal GetUser()
        {
            if (_authState == null)
                return new ClaimsPrincipal();
            else
                return _authState.User;
        }

        /// <summary>
        /// Prüft, ob der aktuelle Benutzer eine bestimmte Rolle hat.
        /// </summary>
        public bool IsInRole(string role)
        {
            if (_authState == null)
                return false;
            else
                return _authState.User.IsInRole(role);
        }

        /// <summary>
        /// Holt einen bestimmten Claim-Wert.
        /// </summary>
        public string? GetClaim(string claimType)
        {
            if (_authState == null)
                return null;
            else
                // Return the value of the claim
                return _authState.User.FindFirst(claimType)?.Value;
        }
    

    }
}
