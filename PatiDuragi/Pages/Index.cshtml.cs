using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages
{
    public class IndexModel : PageModel
    {
        private readonly PatiDbContext _context;

        public IndexModel(PatiDbContext context)
        {
            _context = context;
        }

        public List<BeslemeNoktasi> Noktalar { get; set; } = new();
        public int BolgeSayisi { get; set; }
        public int OnayBekleyenSayisi { get; set; }

        public async Task OnGetAsync()
        {
            Noktalar = await _context.Noktalar.ToListAsync();
            BolgeSayisi = await _context.Bolgeler.CountAsync();
            OnayBekleyenSayisi = Noktalar.Count(n => !n.Onayli);
        }
    }
}
