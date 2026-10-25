using PatiDuragi.Models;

namespace PatiDuragi.Data
{
    public static class BolgeDeposu
    {
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
    }
}
