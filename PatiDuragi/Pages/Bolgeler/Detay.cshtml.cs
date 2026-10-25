using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages.Bolgeler
{
    public class DetayModel : PageModel
    {
        public Bolge SeciliBolge { get; set; } = new();
        public int NoktaSayisi { get; set; }

        public IActionResult OnGet(int id)
        {
            var bolge = BolgeDeposu.Bul(id);
            if (bolge == null)
            {
                return NotFound();
            }

            SeciliBolge = bolge;
            NoktaSayisi = NoktaDeposu.Noktalar.Count(n => n.BolgeId == id);
            return Page();
        }
    }
}
