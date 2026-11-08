using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages.Bolgeler
{
    public class SilModel : PageModel
    {
        private readonly PatiDbContext _context;

        public SilModel(PatiDbContext context)
        {
            _context = context;
        }

        public Bolge SeciliBolge { get; set; } = new();
        public bool Silinebilir { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var bolge = await _context.Bolgeler.FindAsync(id);
            if (bolge == null)
            {
                return NotFound();
            }

            SeciliBolge = bolge;
            Silinebilir = await SilinebilirMi(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var bolge = await _context.Bolgeler.FindAsync(id);
            if (bolge == null)
            {
                return NotFound();
            }

            // Form atlanıp doğrudan istek gönderilse bile kural sunucuda tekrar kontrol edilir
            if (!await SilinebilirMi(id))
            {
                return RedirectToPage("Sil", new { id });
            }

            _context.Bolgeler.Remove(bolge);
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = $"\"{bolge.TamAd}\" bölgesi silindi.";
            return RedirectToPage("Index");
        }

        // İçinde besleme noktası olan bölge silinemez (noktalar sahipsiz kalmasın)
        private async Task<bool> SilinebilirMi(int id)
        {
            return !await _context.Noktalar.AnyAsync(n => n.BolgeId == id);
        }
    }
}
