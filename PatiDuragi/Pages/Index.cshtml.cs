using Microsoft.AspNetCore.Mvc.RazorPages;
using PatiDuragi.Data;

namespace PatiDuragi.Pages
{
    public class IndexModel : PageModel
    {
        public int BolgeSayisi { get; set; }
        public int NoktaSayisi { get; set; }
        public int OnayBekleyenSayisi { get; set; }

        public void OnGet()
        {
            BolgeSayisi = BolgeDeposu.Bolgeler.Count;
            NoktaSayisi = NoktaDeposu.Noktalar.Count;
            OnayBekleyenSayisi = NoktaDeposu.Noktalar.Count(n => !n.Onayli);
        }
    }
}
