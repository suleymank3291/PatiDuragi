using System.Globalization;
using Microsoft.EntityFrameworkCore;
using PatiDuragi.Models;
using PatiDuragi.Services;

namespace PatiDuragi.Data
{
    public class PatiDbContext : DbContext
    {
        public PatiDbContext(DbContextOptions<PatiDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bolge> Bolgeler { get; set; } = default!;
        public DbSet<BeslemeNoktasi> Noktalar { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // İçinde nokta olan bölge veritabanı seviyesinde de silinemesin
            modelBuilder.Entity<BeslemeNoktasi>()
                .HasOne(n => n.Bolge)
                .WithMany(b => b.Noktalar)
                .HasForeignKey(n => n.BolgeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Aynı mahalle iki kez eklenemesin (OpenStreetMap numarası benzersiz)
            modelBuilder.Entity<Bolge>().HasIndex(b => b.OsmId).IsUnique();

            // Örnek veriler 1'den başlayan id'leri kullandığı için yeni kayıtların id sayacı onlardan sonra başlar
            modelBuilder.Entity<Bolge>().Property(b => b.Id).HasIdentityOptions(startValue: 4);
            modelBuilder.Entity<BeslemeNoktasi>().Property(n => n.Id).HasIdentityOptions(startValue: 9);

            // İlk migration ile tablolara eklenen örnek veriler (noktalar gerçekten mahallelerinin içinde)
            modelBuilder.Entity<Bolge>().HasData(
                new Bolge { Id = 1, Ilce = "Çankaya", Mahalle = "Bahçelievler Mahallesi", OsmId = 6779580, Sinir = OrnekSinirlar.Bahcelievler },
                new Bolge { Id = 2, Ilce = "Çankaya", Mahalle = "Emek Mahallesi", OsmId = 6779678, Sinir = OrnekSinirlar.Emek },
                new Bolge { Id = 3, Ilce = "Yenimahalle", Mahalle = "İlkyerleşim Mahallesi", OsmId = 5623804, Sinir = OrnekSinirlar.Ilkyerlesim }
            );

            modelBuilder.Entity<BeslemeNoktasi>().HasData(
                new BeslemeNoktasi { Id = 1, Ad = "Metin Oktay Parkı", Aciklama = "Parkın girişindeki bankın yanında.",
                                     Tur = "Mama ve Su", Enlem = 39.92626, Boylam = 32.82470, Onayli = true, BolgeId = 1 },
                new BeslemeNoktasi { Id = 2, Ad = "Nebahat Keskin İlkokulu Önü", Aciklama = "Okul bahçe duvarının köşesinde.",
                                     Tur = "Su", Enlem = 39.92842, Boylam = 32.82369, Onayli = true, BolgeId = 1 },
                new BeslemeNoktasi { Id = 3, Ad = "Emek Pazarı", Aciklama = "Pazar yerinin otopark tarafında.",
                                     Tur = "Mama", Enlem = 39.92567, Boylam = 32.81703, Onayli = true, BolgeId = 2 },
                new BeslemeNoktasi { Id = 4, Ad = "Yunus Emre Parkı", Aciklama = "Çocuk parkının arkasındaki ağaçların dibinde.",
                                     Tur = "Mama ve Su", Enlem = 39.92897, Boylam = 32.81908, Onayli = false, BolgeId = 2 },
                new BeslemeNoktasi { Id = 5, Ad = "Batıkent Pazar Yeri", Aciklama = "Pazar yerinin metroya bakan köşesinde.",
                                     Tur = "Mama ve Su", Enlem = 39.97050, Boylam = 32.72437, Onayli = true, BolgeId = 3 },
                new BeslemeNoktasi { Id = 6, Ad = "Emek Aile Sağlığı Merkezi Arkası", Aciklama = "Bahçe çitinin dibinde, iki kap yan yana.",
                                     Tur = "Mama ve Su", Enlem = 39.92415, Boylam = 32.81600, Onayli = true, BolgeId = 2 },
                new BeslemeNoktasi { Id = 7, Ad = "Bahriye Üçok Parkı", Aciklama = "Parkın kuzey girişindeki çeşmenin yanında.",
                                     Tur = "Mama", Enlem = 39.93233, Boylam = 32.82690, Onayli = true, BolgeId = 1 },
                new BeslemeNoktasi { Id = 8, Ad = "Halil Tunç Parkı", Aciklama = "Parkın ortasındaki çardağın yanında.",
                                     Tur = "Su", Enlem = 39.97820, Boylam = 32.72319, Onayli = false, BolgeId = 3 }
            );
        }

        // Koordinatı sınırının içine alan bölgeyi bulur; hiçbir bölgenin içinde değilse null döner.
        // Noktanın bölgesi elle seçilmez, bu metotla konumdan bulunur.
        public async Task<Bolge?> KonumunBolgesi(double enlem, double boylam)
        {
            var bolgeler = await Bolgeler.ToListAsync();
            return bolgeler.FirstOrDefault(b => SinirServisi.IcindeMi(enlem, boylam, b.Sinir));
        }

        // Aynı bölgede aynı adla başka nokta var mı? Düzenlemede noktanın kendisi (haricId) sayılmaz.
        // Türkçe harfler doğru karşılaştırılsın diye adlar C# tarafında karşılaştırılır.
        public async Task<bool> AyniNoktaAdiVarMi(string ad, int bolgeId, int haricId = 0)
        {
            var adlar = await Noktalar.Where(n => n.BolgeId == bolgeId && n.Id != haricId).Select(n => n.Ad).ToListAsync();
            var turkce = new CultureInfo("tr-TR");
            return adlar.Any(a => string.Compare(a.Trim(), ad.Trim(), turkce, CompareOptions.IgnoreCase) == 0);
        }
    }
}
