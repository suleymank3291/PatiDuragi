using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages.Bolgeler
{
    public class DetayModel : PageModel
    {
        private readonly PatiDbContext _context;

        public DetayModel(PatiDbContext context)
        {
            _context = context;
        }

        public Bolge SeciliBolge { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var bolge = await _context.Bolgeler
                .Include(b => b.Noktalar)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (bolge == null)
            {
                return NotFound();
            }

            SeciliBolge = bolge;
            return Page();
        }
    }
}
