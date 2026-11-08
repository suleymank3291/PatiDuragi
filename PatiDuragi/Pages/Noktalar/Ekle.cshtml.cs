using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages.Noktalar
{
    public class EkleModel : PageModel
    {
        private readonly PatiDbContext _context;

        public EkleModel(PatiDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BeslemeNoktasi YeniNokta { get; set; } = new();

        // Bölge detayından "Bu bölgeye nokta ekle" ile gelindiyse o bölge formun üstünde gösterilir
        public Bolge? HedefBolge { get; set; }

        public async Task OnGetAsync(int? bolgeId)
        {
            if (bolgeId != null)
            {
                HedefBolge = await _context.Bolgeler.FindAsync(bolgeId.Value);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Bölge elle seçilmez: koordinat hangi mahallenin sınırı içindeyse nokta o bölgeye atanır
            var bolge = await _context.KonumunBolgesi(YeniNokta.Enlem ?? 0, YeniNokta.Boylam ?? 0);
            if (bolge == null)
            {
                ModelState.AddModelError("YeniNokta.Enlem", "Bu konum kayıtlı hiçbir mahallenin içinde değil. Önce mahalleyi Bölgeler sayfasından ekleyin.");
                return Page();
            }

            if (await _context.AyniNoktaAdiVarMi(YeniNokta.Ad, bolge.Id))
            {
                ModelState.AddModelError("YeniNokta.Ad", $"{bolge.Mahalle} içinde aynı adla bir nokta zaten var.");
                return Page();
            }

            YeniNokta.BolgeId = bolge.Id;
            _context.Noktalar.Add(YeniNokta);
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = $"\"{YeniNokta.Ad}\" noktası eklendi ({bolge.TamAd}).";
            return RedirectToPage("Detay", new { id = YeniNokta.Id });
        }
    }
}
