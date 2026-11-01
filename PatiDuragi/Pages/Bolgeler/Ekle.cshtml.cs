using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages.Bolgeler
{
    public class EkleModel : PageModel
    {
        [BindProperty]
        public Bolge YeniBolge { get; set; } = new();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            BolgeDeposu.Ekle(YeniBolge);
            TempData["Mesaj"] = $"\"{YeniBolge.Ad}\" bölgesi eklendi.";
            return RedirectToPage("Index");
        }
    }
}
