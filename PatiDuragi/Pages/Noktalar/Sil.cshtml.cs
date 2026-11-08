using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages.Noktalar
{
    public class SilModel : PageModel
    {
        private readonly PatiDbContext _context;

        public SilModel(PatiDbContext context)
        {
            _context = context;
        }

        public BeslemeNoktasi SeciliNokta { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var nokta = await _context.Noktalar
                .Include(n => n.Bolge)
                .FirstOrDefaultAsync(n => n.Id == id);
            if (nokta == null)
            {
                return NotFound();
            }

            SeciliNokta = nokta;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var nokta = await _context.Noktalar.FindAsync(id);
            if (nokta == null)
            {
                return NotFound();
            }

            _context.Noktalar.Remove(nokta);
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = $"\"{nokta.Ad}\" noktası silindi.";
            return RedirectToPage("Index");
        }
    }
}
