using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages.Noktalar
{
    public class DetayModel : PageModel
    {
        private readonly PatiDbContext _context;

        public DetayModel(PatiDbContext context)
        {
            _context = context;
        }

        public BeslemeNoktasi SeciliNokta { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Include: noktanın bölgesi (adı ve sınırı) de birlikte yüklenir
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
    }
}
