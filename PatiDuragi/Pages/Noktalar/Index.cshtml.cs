using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages.Noktalar
{
    public class IndexModel : PageModel
    {
        private readonly PatiDbContext _context;

        public IndexModel(PatiDbContext context)
        {
            _context = context;
        }

        // Noktalar bölgelerine göre gruplu gösterilir: her bölge kendi noktalarıyla birlikte yüklenir
        public List<Bolge> Bolgeler { get; set; } = new();
        public List<BeslemeNoktasi> Noktalar { get; set; } = new();

        public async Task OnGetAsync()
        {
            Bolgeler = await _context.Bolgeler
                .Include(b => b.Noktalar)
                .OrderBy(b => b.Ilce).ThenBy(b => b.Mahalle)
                .ToListAsync();

            Noktalar = Bolgeler.SelectMany(b => b.Noktalar).ToList();
        }
    }
}
