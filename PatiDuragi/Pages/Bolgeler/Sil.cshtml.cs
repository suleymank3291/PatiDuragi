using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages.Bolgeler
{
    public class SilModel : PageModel
    {
        public Bolge SeciliBolge { get; set; } = new();
        public bool Silinebilir { get; set; }

        public IActionResult OnGet(int id)
        {
            var bolge = BolgeDeposu.Bul(id);
            if (bolge == null)
            {
                return NotFound();
            }

            SeciliBolge = bolge;
            Silinebilir = BolgeDeposu.SilinebilirMi(id);
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            var bolge = BolgeDeposu.Bul(id);
            if (bolge == null)
            {
                return NotFound();
            }

            // Form atlanıp doğrudan istek gönderilse bile kural sunucuda tekrar kontrol edilir
            if (!BolgeDeposu.SilinebilirMi(id))
            {
                return RedirectToPage("Sil", new { id });
            }

            BolgeDeposu.Sil(bolge);
            TempData["Mesaj"] = $"\"{bolge.Ad}\" bölgesi silindi.";
            return RedirectToPage("Index");
        }
    }
}
