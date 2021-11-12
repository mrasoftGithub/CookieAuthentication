using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cookie.Shared.Models
{
    public partial class Lid
    {
        public int ID { get; set; }

        public string Achternaam { get; set; }

        public string Voornaam { get; set; }

        [Required(ErrorMessage = "Geef je e-mailadres op.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Geef je wachtwoord op.")]
        public string Password { get; set; }

        public string Abonnement { get; set; }

        public bool Expired { get; set; }

    }
}
