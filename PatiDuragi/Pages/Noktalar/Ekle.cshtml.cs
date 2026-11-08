using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
        // ve konum haritası o mahalleye odaklanır
        public Bolge? HedefBolge { get; set; }

        // Konum haritasında sınırları çizilecek mahalleler
        public List<Bolge> Bolgeler { get; set; } = new();

        public async Task OnGetAsync(int? bolgeId)
        {
            await SayfaVerisiniYukle(bolgeId);
        }

        // Form bölgeId'li adrese gönderildiği için (?bolgeId=3) hata durumunda da aynı mahalle korunur
        public async Task<IActionResult> OnPostAsync(int? bolgeId)
        {
            await SayfaVerisiniYukle(bolgeId);

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

        private async Task SayfaVerisiniYukle(int? bolgeId)
        {
            Bolgeler = await _context.Bolgeler.OrderBy(b => b.Mahalle).ToListAsync();
            HedefBolge = Bolgeler.FirstOrDefault(b => b.Id == bolgeId);
        }
    }
}
