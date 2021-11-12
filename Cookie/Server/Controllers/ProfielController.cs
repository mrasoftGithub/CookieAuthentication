using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cookie.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Cookie.Server.Model;

namespace Cookie.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfielController : ControllerBase
    {
        private readonly IModel _model;

        public ProfielController(IModel model)
        {
            _model = model;
        }

        [HttpGet]
        [Route("gegevenslid/{ID}")]
        public async Task<ActionResult<Lid>> gegevenslid(int ID)
        {
            Lid lid = await _model.HaalopLid(ID);
            return Ok(lid);
        }

        [HttpGet("huidiglid")]
        public async Task<ActionResult<Lid>> HuidigLid()
        {
            try
            {
                //---
                // Een throw om de foutafhandeling te testen
                // throw new Exception("Unable to get the data of the current member.");
                //---

                // Leeg object retourneren indien niet Authenticated
                // Anders object vullen aan de hand van de claim
                Lid huidigLid = new Lid();

                // Ben je al ingelogd?
                if (User.Identity.IsAuthenticated)
                {
                    // Er is een claimID in de cookie bij een succesvolle aanmelding.
                    var claimID = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    // De claimID mogen we gebruiken voor het ophalen van gegevens omdat we 
                    // die hebben gekregen als gevolg van een succesvolle aanmelding  
                    huidigLid = await _model.HaalopLid(Convert.ToInt32(claimID));
                }

                // object leeg laten als de user niet authenticated is
                // anders beschouwt de programmatuur je alsnog als authenticated
                return await Task.FromResult(huidigLid);
            }
            catch
            {
                // Leeg object retourneren als om één of andere reden
                // bij het ophalen van de gegevens toch wat fout gaat
                return await Task.FromResult(new Lid());
            }
        }
    }
}
