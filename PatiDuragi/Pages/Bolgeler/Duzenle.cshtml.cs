using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages.Bolgeler
{
    public class DuzenleModel : PageModel
    {
        [BindProperty]
        public Bolge DuzenlenenBolge { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            var bolge = BolgeDeposu.Bul(id);
            if (bolge == null)
            {
                return NotFound();
            }

            DuzenlenenBolge = bolge;
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            if (BolgeDeposu.AyniAdVarMi(DuzenlenenBolge.Ad, id))
            {
                ModelState.AddModelError("DuzenlenenBolge.Ad", "Bu adla bir bölge zaten var.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var bolge = BolgeDeposu.Bul(id);
            if (bolge == null)
            {
                return NotFound();
            }

            bolge.Ad = DuzenlenenBolge.Ad;
            bolge.Ilce = DuzenlenenBolge.Ilce;

            TempData["Mesaj"] = $"\"{bolge.Ad}\" bölgesi güncellendi.";
            return RedirectToPage("Index");
        }
    }
}
