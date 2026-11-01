using System.Globalization;
using PatiDuragi.Models;

namespace PatiDuragi.Data
{
    public static class BolgeDeposu
    {
        private static int sonId = 3;

        public static List<Bolge> Bolgeler { get; } = new()
        {
            new Bolge { Id = 1, Ad = "Bahçelievler", Ilce = "Çankaya" },
            new Bolge { Id = 2, Ad = "Emek", Ilce = "Çankaya" },
            new Bolge { Id = 3, Ad = "Batıkent", Ilce = "Yenimahalle" }
        };

        public static Bolge? Bul(int id)
        {
            return Bolgeler.FirstOrDefault(b => b.Id == id);
        }

        public static void Ekle(Bolge bolge)
        {
            sonId++;
            bolge.Id = sonId;
            Bolgeler.Add(bolge);
        }

        public static void Sil(Bolge bolge)
        {
            Bolgeler.Remove(bolge);
        }

        // Aynı adla başka bölge var mı? Düzenlemede bölgenin kendisi (haricId) sayılmaz.
        public static bool AyniAdVarMi(string ad, int haricId = 0)
        {
            var turkce = new CultureInfo("tr-TR");
            return Bolgeler.Any(b => b.Id != haricId &&
                string.Compare(b.Ad.Trim(), ad.Trim(), turkce, CompareOptions.IgnoreCase) == 0);
        }

        // İçinde besleme noktası olan bölge silinemez (noktalar sahipsiz kalmasın)
        public static bool SilinebilirMi(int id)
        {
            return !NoktaDeposu.Noktalar.Any(n => n.BolgeId == id);
        }
    }
}
