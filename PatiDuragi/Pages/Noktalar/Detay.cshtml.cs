using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages.Noktalar
{
    public class DetayModel : PageModel
    {
        public BeslemeNoktasi SeciliNokta { get; set; } = new();
        public Bolge? NoktaninBolgesi { get; set; }

        public IActionResult OnGet(int id)
        {
            var nokta = NoktaDeposu.Bul(id);
            if (nokta == null)
            {
                return NotFound();
            }

            SeciliNokta = nokta;
            NoktaninBolgesi = BolgeDeposu.Bul(nokta.BolgeId);
            return Page();
        }
    }
}
