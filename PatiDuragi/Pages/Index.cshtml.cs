using Microsoft.AspNetCore.Mvc.RazorPages;
using PatiDuragi.Data;
using PatiDuragi.Models;

namespace PatiDuragi.Pages
{
    public class IndexModel : PageModel
    {
        public List<BeslemeNoktasi> Noktalar { get; set; } = new();
        public int BolgeSayisi { get; set; }
        public int OnayBekleyenSayisi { get; set; }

        public void OnGet()
        {
            Noktalar = NoktaDeposu.Noktalar;
            BolgeSayisi = BolgeDeposu.Bolgeler.Count;
            OnayBekleyenSayisi = Noktalar.Count(n => !n.Onayli);
        }
    }
}
