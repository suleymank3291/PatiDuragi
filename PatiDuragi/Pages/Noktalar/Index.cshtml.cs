using Microsoft.AspNetCore.Mvc.RazorPages;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages.Noktalar
{
    public class IndexModel : PageModel
    {
        public List<BeslemeNoktasi> Noktalar { get; set; } = new();

        public void OnGet()
        {
            Noktalar = NoktaDeposu.Noktalar;
        }

        public string BolgeAdi(int bolgeId)
        {
            return BolgeDeposu.Bul(bolgeId)?.Ad ?? "-";
        }
    }
}
