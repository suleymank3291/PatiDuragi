using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatiDuragi.Data;
using PatiDuragi.Models;
using PatiDuragi.Services;

namespace PatiDuragi.Pages.Bolgeler
{
    public class EkleModel : PageModel
    {
        private readonly PatiDbContext _context;
        private readonly SinirServisi _sinirServisi;

        public EkleModel(PatiDbContext context, SinirServisi sinirServisi)
        {
            _context = context;
            _sinirServisi = sinirServisi;
        }

        [BindProperty]
        public Bolge YeniBolge { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Mahalle OpenStreetMap'te aranır; bulunamazsa ya da servise ulaşılamazsa form hatayla geri döner
            MahalleBilgisi? bilgi;
            try
            {
                bilgi = await _sinirServisi.MahalleBul(YeniBolge.Mahalle, YeniBolge.Ilce);
            }
            catch (Exception hata) when (hata is HttpRequestException or TaskCanceledException)
            {
                ModelState.AddModelError("", "Mahalle sınırı alınamadı. İnternet bağlantısını kontrol edip tekrar deneyin.");
                return Page();
            }

            if (bilgi == null)
            {
                ModelState.AddModelError("YeniBolge.Mahalle", "Bu ilçede böyle bir mahalle bulunamadı. Semt değil, resmî mahalle adını yazın.");
                return Page();
            }

            if (await _context.Bolgeler.AnyAsync(b => b.OsmId == bilgi.OsmId))
            {
                ModelState.AddModelError("YeniBolge.Mahalle", $"{bilgi.Mahalle} zaten ekli.");
                return Page();
            }

            // Ad ve ilçe, yazım farkı olmasın diye OpenStreetMap'teki resmî hâliyle kaydedilir
            YeniBolge.Mahalle = bilgi.Mahalle;
            YeniBolge.Ilce = bilgi.Ilce;
            YeniBolge.OsmId = bilgi.OsmId;
            YeniBolge.Sinir = bilgi.Sinir;

            _context.Bolgeler.Add(YeniBolge);
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = $"\"{YeniBolge.TamAd}\" bölgesi eklendi.";
            return RedirectToPage("Index");
        }
    }
}
