using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages.Noktalar
{
    public class DuzenleModel : PageModel
    {
        [BindProperty]
        public BeslemeNoktasi DuzenlenenNokta { get; set; } = new();

        public int Id { get; set; }

        public IActionResult OnGet(int id)
        {
            var nokta = NoktaDeposu.Bul(id);
            if (nokta == null)
            {
                return NotFound();
            }

            Id = id;
            DuzenlenenNokta = nokta;
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            var nokta = NoktaDeposu.Bul(id);
            if (nokta == null)
            {
                return NotFound();
            }

            Id = id;
            foreach (var (alan, mesaj) in NoktaDeposu.KuralHatalari(DuzenlenenNokta, id))
            {
                ModelState.AddModelError($"DuzenlenenNokta.{alan}", mesaj);
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Formdan gelen nesne doğrudan listeye konmaz; yalnızca düzenlenebilir alanlar kopyalanır
            nokta.Ad = DuzenlenenNokta.Ad;
            nokta.Aciklama = DuzenlenenNokta.Aciklama;
            nokta.Tur = DuzenlenenNokta.Tur;
            nokta.Enlem = DuzenlenenNokta.Enlem;
            nokta.Boylam = DuzenlenenNokta.Boylam;
            nokta.Onayli = DuzenlenenNokta.Onayli;
            nokta.BolgeId = DuzenlenenNokta.BolgeId;

            TempData["Mesaj"] = $"\"{nokta.Ad}\" noktası güncellendi.";
            return RedirectToPage("Detay", new { id });
        }
    }
}
