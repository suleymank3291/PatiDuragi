using Microsoft.AspNetCore.Mvc.RazorPages;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages.Bolgeler
{
    public class IndexModel : PageModel
    {
        public List<Bolge> Bolgeler { get; set; } = new();

        public void OnGet()
        {
            Bolgeler = BolgeDeposu.Bolgeler;
        }

        public int NoktaSayisi(int bolgeId)
        {
            return NoktaDeposu.Noktalar.Count(n => n.BolgeId == bolgeId);
        }
    }
}
