using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages.Noktalar
{
    public class SilModel : PageModel
    {
        public BeslemeNoktasi SeciliNokta { get; set; } = new();
        public string BolgeAdi { get; set; } = "";

        public IActionResult OnGet(int id)
        {
            var nokta = NoktaDeposu.Bul(id);
            if (nokta == null)
            {
                return NotFound();
            }

            SeciliNokta = nokta;
            BolgeAdi = BolgeDeposu.Bul(nokta.BolgeId)?.Ad ?? "-";
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            var nokta = NoktaDeposu.Bul(id);
            if (nokta == null)
            {
                return NotFound();
            }

            NoktaDeposu.Sil(nokta);
            TempData["Mesaj"] = $"\"{nokta.Ad}\" noktası silindi.";
            return RedirectToPage("Index");
        }
    }
}
