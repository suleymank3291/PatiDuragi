using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages.Noktalar
{
    public class DuzenleModel : PageModel
    {
        private readonly PatiDbContext _context;

        public DuzenleModel(PatiDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BeslemeNoktasi DuzenlenenNokta { get; set; } = new();

        public int Id { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var nokta = await _context.Noktalar.FindAsync(id);
            if (nokta == null)
            {
                return NotFound();
            }

            Id = id;
            DuzenlenenNokta = nokta;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Id = id;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var nokta = await _context.Noktalar.FindAsync(id);
            if (nokta == null)
            {
                return NotFound();
            }

            // Konum değişmiş olabilir; bölge yeni koordinata göre tekrar bulunur
            var bolge = await _context.KonumunBolgesi(DuzenlenenNokta.Enlem ?? 0, DuzenlenenNokta.Boylam ?? 0);
            if (bolge == null)
            {
                ModelState.AddModelError("DuzenlenenNokta.Enlem", "Bu konum kayıtlı hiçbir mahallenin içinde değil. Önce mahalleyi Bölgeler sayfasından ekleyin.");
                return Page();
            }

            if (await _context.AyniNoktaAdiVarMi(DuzenlenenNokta.Ad, bolge.Id, id))
            {
                ModelState.AddModelError("DuzenlenenNokta.Ad", $"{bolge.Mahalle} içinde aynı adla bir nokta zaten var.");
                return Page();
            }

            // Formdan gelen nesne doğrudan kaydedilmez; yalnızca düzenlenebilir alanlar kopyalanır
            nokta.Ad = DuzenlenenNokta.Ad;
            nokta.Aciklama = DuzenlenenNokta.Aciklama;
            nokta.Tur = DuzenlenenNokta.Tur;
            nokta.Enlem = DuzenlenenNokta.Enlem;
            nokta.Boylam = DuzenlenenNokta.Boylam;
            nokta.Onayli = DuzenlenenNokta.Onayli;
            nokta.BolgeId = bolge.Id;
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = $"\"{nokta.Ad}\" noktası güncellendi ({bolge.TamAd}).";
            return RedirectToPage("Detay", new { id });
        }
    }
}
