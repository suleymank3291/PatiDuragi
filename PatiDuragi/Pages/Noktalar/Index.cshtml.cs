using Microsoft.AspNetCore.Mvc;
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

        // Arama ve filtre değerleri adres çubuğundan gelir (?Arama=park&Tur=Su&Onay=bekleyen).
        // SupportsGet = true olmadan [BindProperty] yalnızca POST isteklerinde doldurulur.
        [BindProperty(SupportsGet = true)]
        public string? Arama { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Tur { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Onay { get; set; }   // "onayli", "bekleyen" ya da boş (hepsi)

        public bool FiltreVar => !string.IsNullOrWhiteSpace(Arama) || !string.IsNullOrEmpty(Tur) || !string.IsNullOrEmpty(Onay);

        // Noktalar mahallelerine göre gruplu gösterilir
        public List<Bolge> Bolgeler { get; set; } = new();

        // Filtreye uyan noktalar (filtre yoksa hepsi)
        public List<BeslemeNoktasi> Noktalar { get; set; } = new();

        // Üstteki özet; filtreden bağımsız olarak tüm noktaları sayar
        public int ToplamSayisi { get; set; }
        public int OnayliSayisi { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<BeslemeNoktasi> sorgu = _context.Noktalar.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(Arama))
            {
                // ILIKE: PostgreSQL'de büyük-küçük harf ayırmadan arama ("park" = "Park")
                var desen = $"%{AramaDeseni(Arama.Trim())}%";
                sorgu = sorgu.Where(n => EF.Functions.ILike(n.Ad, desen, "\\") || EF.Functions.ILike(n.Aciklama, desen, "\\"));
            }

            // Listede olmayan bir tür adresten elle yazılırsa filtre yok sayılır
            if (!string.IsNullOrEmpty(Tur) && BeslemeNoktasi.Turler.Contains(Tur))
            {
                sorgu = sorgu.Where(n => n.Tur == Tur);
            }

            if (Onay == "onayli")
            {
                sorgu = sorgu.Where(n => n.Onayli);
            }
            else if (Onay == "bekleyen")
            {
                sorgu = sorgu.Where(n => !n.Onayli);
            }

            Noktalar = await sorgu.OrderBy(n => n.Ad).ToListAsync();

            Bolgeler = await _context.Bolgeler
                .AsNoTracking()
                .OrderBy(b => b.Ilce).ThenBy(b => b.Mahalle)
                .ToListAsync();

            ToplamSayisi = await _context.Noktalar.CountAsync();
            OnayliSayisi = await _context.Noktalar.CountAsync(n => n.Onayli);
        }

        // Kullanıcının yazdığı metni ILIKE desenine çevirir.
        // 1) % ve _ ILIKE'ta özel karakter olduğu için kaçış karakteriyle düz harfe çevrilir.
        // 2) PostgreSQL "I" harfini "i" yapar, Türkçedeki "ı" ile eşleştiremez ("PARKI" ≠ "Parkı").
        //    Bu yüzden i, ı, I, İ harfleri "tek bir harf" anlamındaki _ jokerine çevrilir.
        private static string AramaDeseni(string metin)
        {
            var desen = metin.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
            foreach (var harf in new[] { 'i', 'ı', 'I', 'İ' })
            {
                desen = desen.Replace(harf, '_');
            }
            return desen;
        }

        // Bir mahallenin filtreye uyan noktaları
        public List<BeslemeNoktasi> BolgeninNoktalari(int bolgeId)
        {
            return Noktalar.Where(n => n.BolgeId == bolgeId).ToList();
        }
    }
}
