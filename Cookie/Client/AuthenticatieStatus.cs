using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Cookie.Shared.Models;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Cookie.Client
{
    public class AuthenticatieStatus : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;

        public AuthenticatieStatus(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // Het object kan gevuld worden als succesvol is ingelogd
                Lid huidiglid = await _httpClient.GetFromJsonAsync<Lid>("/api/Profiel/huidiglid");
                if (huidiglid != null && huidiglid.ID != 0)
                {
                    //creëer claims uit het geretourneerde object
                    var claimID = new Claim(ClaimTypes.NameIdentifier, Convert.ToString(huidiglid.ID));
                    var claimNaam = new Claim(ClaimTypes.Name, huidiglid.Voornaam + " " + huidiglid.Achternaam);
                    var claimExpired = new Claim(ClaimTypes.Expired, Convert.ToString(huidiglid.Expired));

                    //create claimsIdentity
                    var claimsIdentity = new ClaimsIdentity(new[] { claimID, claimNaam, claimExpired }, "lidSportschool");

                    //retourneer de claimsPrincipal
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    return new AuthenticationState(claimsPrincipal);
                }
                else
                    // het lid kan niet gevonden 
                    // retourneer een lege ClaimsPrincipal
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            catch
            {
                // er is wat fout gegaan, retourneer een lege ClaimsPrincipal
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

        }
    }
}
