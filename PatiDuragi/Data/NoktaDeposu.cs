using System.Globalization;
using PatiDuragi.Models;

namespace PatiDuragi.Data
{
    public static class NoktaDeposu
    {
        private static int sonId = 8;

        public static List<BeslemeNoktasi> Noktalar { get; } = new()
        {
            new BeslemeNoktasi { Id = 1, Ad = "7. Cadde Parkı", Aciklama = "Parkın girişindeki bankın yanında.",
                                 Tur = "Mama ve Su", Enlem = 39.9213, Boylam = 32.8235, Onayli = true, BolgeId = 1 },
            new BeslemeNoktasi { Id = 2, Ad = "Aşkabat Caddesi Durağı", Aciklama = "Otobüs durağının arkasındaki ağacın dibinde.",
                                 Tur = "Su", Enlem = 39.9188, Boylam = 32.8262, Onayli = true, BolgeId = 1 },
            new BeslemeNoktasi { Id = 3, Ad = "Emek Pazar Yeri", Aciklama = "Pazar yerinin otopark tarafında.",
                                 Tur = "Mama", Enlem = 39.9271, Boylam = 32.8124, Onayli = true, BolgeId = 2 },
            new BeslemeNoktasi { Id = 4, Ad = "Bişkek Caddesi Okul Önü", Aciklama = "Okul bahçe duvarının köşesinde.",
                                 Tur = "Mama ve Su", Enlem = 39.9292, Boylam = 32.8090, Onayli = false, BolgeId = 2 },
            new BeslemeNoktasi { Id = 5, Ad = "Batıkent Metro Çıkışı", Aciklama = "Metro çıkışının sağındaki yeşil alanda.",
                                 Tur = "Mama ve Su", Enlem = 39.9686, Boylam = 32.7310, Onayli = true, BolgeId = 3 },
            new BeslemeNoktasi { Id = 6, Ad = "Emek Sağlık Ocağı Arkası", Aciklama = "Bahçe çitinin dibinde, iki kap yan yana.",
                                 Tur = "Mama ve Su", Enlem = 39.9248, Boylam = 32.8158, Onayli = true, BolgeId = 2 },
            new BeslemeNoktasi { Id = 7, Ad = "Bahçelievler 3. Cadde", Aciklama = "Eczanenin yanındaki ağaçlık alanda.",
                                 Tur = "Mama", Enlem = 39.9232, Boylam = 32.8198, Onayli = true, BolgeId = 1 },
            new BeslemeNoktasi { Id = 8, Ad = "Batıkent Göl Parkı", Aciklama = "Gölün kuzey ucundaki çardağın yanında.",
                                 Tur = "Su", Enlem = 39.9712, Boylam = 32.7265, Onayli = false, BolgeId = 3 }
        };

        public static BeslemeNoktasi? Bul(int id)
        {
            return Noktalar.FirstOrDefault(n => n.Id == id);
        }

        public static void Ekle(BeslemeNoktasi nokta)
        {
            sonId++;
            nokta.Id = sonId;
            Noktalar.Add(nokta);
        }

        public static void Sil(BeslemeNoktasi nokta)
        {
            Noktalar.Remove(nokta);
        }

        // DataAnnotations'ın kontrol edemediği, depoya bakmayı gerektiren kurallar.
        // Ekle ve Düzenle sayfaları aynı kuralları kullanır; dönen her hata (alan adı, mesaj) çiftidir.
        public static List<(string Alan, string Mesaj)> KuralHatalari(BeslemeNoktasi nokta, int haricId = 0)
        {
            var hatalar = new List<(string, string)>();

            if (nokta.BolgeId > 0 && BolgeDeposu.Bul(nokta.BolgeId) == null)
            {
                hatalar.Add(("BolgeId", "Seçilen bölge bulunamadı."));
            }

            if (!string.IsNullOrWhiteSpace(nokta.Ad) && AyniAdVarMi(nokta.Ad, nokta.BolgeId, haricId))
            {
                hatalar.Add(("Ad", "Bu bölgede aynı adla bir nokta zaten var."));
            }

            return hatalar;
        }

        // Aynı bölgede aynı adla başka nokta var mı? Düzenlemede noktanın kendisi (haricId) sayılmaz.
        public static bool AyniAdVarMi(string ad, int bolgeId, int haricId = 0)
        {
            var turkce = new CultureInfo("tr-TR");
            return Noktalar.Any(n => n.Id != haricId && n.BolgeId == bolgeId &&
                string.Compare(n.Ad.Trim(), ad.Trim(), turkce, CompareOptions.IgnoreCase) == 0);
        }
    }
}
