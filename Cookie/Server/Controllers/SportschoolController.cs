using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Cookie.Shared.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Cookie.Server.Model;

namespace Cookie.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SportschoolController : ControllerBase
    {
        private readonly IModel _model;

        public SportschoolController(IModel model)
        {
            _model = model;
        }

        [HttpPost("aanmelden")]
        public async Task<ActionResult<Lid>> Aanmelden(Lid lid)
        {
            try
            {
                //---
                // Een throw om de foutafhandeling te testen
                // throw new Exception("Unable to sign in.");
                //---

                // MaakHash password
                lid.Password = Utility.MaakHash(lid.Password);

                // Lid aangemeldLid = new Lid();
                Lid aangemeldLid = await _model.HaalopLid(lid.Email, lid.Password);

                // Evalueren dat correct is ingelogd
                if(aangemeldLid != null)
                {
                    //------------------------------
                    // using System.Security.Claims;
                    //------------------------------
                    //create claims
                    var claimID = new Claim(ClaimTypes.NameIdentifier, Convert.ToString(aangemeldLid.ID));
                    var claimNaam = new Claim(ClaimTypes.Name, aangemeldLid.Voornaam + " " + aangemeldLid.Achternaam);

                    //create claimsIdentity
                    var claimsIdentity = new ClaimsIdentity(new[] { claimID, claimNaam }, "lidSportschool");

                    //create claimsPrincipal
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // AuthenticationProperties
                    AuthenticationProperties authenticationProperties = new AuthenticationProperties()
                    {
                        IsPersistent = false,
                        ExpiresUtc = DateTime.Now.AddSeconds(30),
                        RedirectUri = "/"
                    };

                    //------------------------------------------
                    //using Microsoft.AspNetCore.Authentication;
                    //------------------------------------------
                    //Sign In User
                    await HttpContext.SignInAsync(claimsPrincipal, authenticationProperties);
                }

                // Retourneer
                // Bij een succevolle aanmelding is:
                // a) het object gevuld
                // b) een ClaimsPrincipal aangemaakt met een claim
                // c) een sign in gedaan met - await HttpContext.SignInAsync(claimsPrincipal);
                // Een leeg object wordt geretourneerd indien het aanmelden niet is gelukt
                return await Task.FromResult(aangemeldLid);
            }
            catch
            {
                // Leeg object retourneren als om één of andere reden
                // bij het ophalen van de gegevens toch wat fout gaat
                return await Task.FromResult(new Lid());
            }
        }

        [HttpGet("afmelden")]
        public async Task<ActionResult<bool>> Afmelden()
        {
            try
            {
                await Task.Delay(0);

                //---
                // Een throw om de foutafhandeling te testen
                // throw new Exception("Error on signing out.");
                //---
                await HttpContext.SignOutAsync();
                return Ok(true);
            }
            catch (Exception e)
            {
                return Problem(
                    statusCode: (int)HttpStatusCode.InternalServerError,
                    detail: "Er is iets fout gegaan met action method " + "Afmelden van SportschoolController - " + e.Message);
            }
        }

        [HttpGet]
        [Route("ongeautoriseerd")]
        public ActionResult Ongeautoriseerd()
        {
            return Unauthorized();
        }
    }
}
