using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages.Bolgeler
{
    public class IndexModel : PageModel
    {
        private readonly PatiDbContext _context;

        public IndexModel(PatiDbContext context)
        {
            _context = context;
        }

        public List<Bolge> Bolgeler { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Include: her bölgenin noktaları da yüklenir, kartta nokta sayısı buradan okunur
            Bolgeler = await _context.Bolgeler
                .Include(b => b.Noktalar)
                .OrderBy(b => b.Ilce).ThenBy(b => b.Mahalle)
                .ToListAsync();
        }
    }
}
