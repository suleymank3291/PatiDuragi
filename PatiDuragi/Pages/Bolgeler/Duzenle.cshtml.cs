using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatiDuragi.Data;
using PatiDuragi.Models;
using PatiDuragi.Services;

namespace PatiDuragi.Pages.Bolgeler
{
    public class DuzenleModel : PageModel
    {
        private readonly PatiDbContext _context;
        private readonly SinirServisi _sinirServisi;

        public DuzenleModel(PatiDbContext context, SinirServisi sinirServisi)
        {
            _context = context;
            _sinirServisi = sinirServisi;
        }

        [BindProperty]
        public Bolge DuzenlenenBolge { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var bolge = await _context.Bolgeler.FindAsync(id);
            if (bolge == null)
            {
                return NotFound();
            }

            DuzenlenenBolge = bolge;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var bolge = await _context.Bolgeler.Include(b => b.Noktalar).FirstOrDefaultAsync(b => b.Id == id);
            if (bolge == null)
            {
                return NotFound();
            }

            MahalleBilgisi? bilgi;
            try
            {
                bilgi = await _sinirServisi.MahalleBul(DuzenlenenBolge.Mahalle, DuzenlenenBolge.Ilce);
            }
            catch (Exception hata) when (hata is HttpRequestException or TaskCanceledException)
            {
                ModelState.AddModelError("", "Mahalle sınırı alınamadı. İnternet bağlantısını kontrol edip tekrar deneyin.");
                return Page();
            }

            if (bilgi == null)
            {
                ModelState.AddModelError("DuzenlenenBolge.Mahalle", "Bu ilçede böyle bir mahalle bulunamadı. Semt değil, resmî mahalle adını yazın.");
                return Page();
            }

            // Başka bir bölge aynı mahalleyi kullanıyor mu? (bölgenin kendisi sayılmaz)
            if (await _context.Bolgeler.AnyAsync(b => b.OsmId == bilgi.OsmId && b.Id != id))
            {
                ModelState.AddModelError("DuzenlenenBolge.Mahalle", $"{bilgi.Mahalle} zaten ekli.");
                return Page();
            }

            // Bölgedeki noktalar yeni sınırın dışında kalacaksa değişikliğe izin verilmez
            var disaridaKalan = bolge.Noktalar.Count(n => !SinirServisi.IcindeMi(n.Enlem ?? 0, n.Boylam ?? 0, bilgi.Sinir));
            if (disaridaKalan > 0)
            {
                ModelState.AddModelError("DuzenlenenBolge.Mahalle", $"Bu bölgedeki {disaridaKalan} nokta yeni mahallenin sınırı dışında kalıyor.");
                return Page();
            }

            bolge.Mahalle = bilgi.Mahalle;
            bolge.Ilce = bilgi.Ilce;
            bolge.OsmId = bilgi.OsmId;
            bolge.Sinir = bilgi.Sinir;
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = $"\"{bolge.TamAd}\" bölgesi güncellendi.";
            return RedirectToPage("Index");
        }
    }
}
