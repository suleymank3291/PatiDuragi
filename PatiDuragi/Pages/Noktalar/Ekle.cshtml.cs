using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages.Noktalar
{
    public class EkleModel : PageModel
    {
        [BindProperty]
        public BeslemeNoktasi YeniNokta { get; set; } = new();

        // Bölge detayından "Bu bölgeye nokta ekle" ile gelindiyse bölge önceden seçili gelir
        public void OnGet(int? bolgeId)
        {
            if (bolgeId != null && BolgeDeposu.Bul(bolgeId.Value) != null)
            {
                YeniNokta.BolgeId = bolgeId.Value;
            }
        }

        public IActionResult OnPost()
        {
            foreach (var (alan, mesaj) in NoktaDeposu.KuralHatalari(YeniNokta))
            {
                ModelState.AddModelError($"YeniNokta.{alan}", mesaj);
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            NoktaDeposu.Ekle(YeniNokta);
            TempData["Mesaj"] = $"\"{YeniNokta.Ad}\" noktası eklendi.";
            return RedirectToPage("Detay", new { id = YeniNokta.Id });
        }
    }
}
