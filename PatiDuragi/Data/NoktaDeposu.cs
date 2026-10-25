using PatiDuragi.Models;

namespace PatiDuragi.Data
{
    public static class NoktaDeposu
    {
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
                                 Tur = "Mama ve Su", Enlem = 39.9686, Boylam = 32.7310, Onayli = true, BolgeId = 3 }
        };

        public static BeslemeNoktasi? Bul(int id)
        {
            return Noktalar.FirstOrDefault(n => n.Id == id);
        }
    }
}
