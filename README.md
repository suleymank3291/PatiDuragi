# Pati Durağı — Sokak Hayvanları Besleme Noktası Takip Portalı

BLM4581 Ağ Tabanlı Teknolojiler ve Uygulamaları dönem projesi (2026–2027 Güz).

Mahallelerdeki mama ve su noktaları gönüllüler tarafından birbirinden habersiz doldurulduğu için bazı noktalar
sürekli dolu, bazıları günlerce boş kalabiliyor. Pati Durağı, besleme noktalarını bölgelere göre tek bir portalda
toplayarak gönüllüler arasında koordinasyon sağlamayı amaçlar.

## Ekip

| Ad Soyad | Öğrenci No | GitHub |
|---|---|---|
| Süleyman Kılınç | 23291035 | [suleymank3291](https://github.com/suleymank3291) |
| Selman Yasin Zemheri | 23291092 | [yazzyreformed](https://github.com/yazzyreformed) |

## Teknolojiler

- ASP.NET Core 10 — Razor Pages
- PostgreSQL + Entity Framework Core 10 (Npgsql)
- Bootstrap 5 ve Bootstrap Icons (mobil uyumlu arayüz)
- Leaflet + OpenStreetMap (harici harita servisi); mahalle sınırları OpenStreetMap Nominatim servisinden alınır

## Kurulum ve çalıştırma

Gereksinimler: [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) ve PostgreSQL.

Aşağıdaki adımlar **bir kez** yapılır.

**1. Projeyi indirin** (daha önce klonladıysanız `git pull` yeterli):

```bash
git clone https://github.com/suleymank3291/PatiDuragi.git
```

**2. PostgreSQL'i kurun:**

- Windows: [postgresql.org](https://www.postgresql.org/download/windows/) adresindeki kurulum programı (pgAdmin de birlikte gelir)
- macOS: `brew install postgresql@16` ve ardından `brew services start postgresql@16`
- Linux: `sudo apt install postgresql`

**3. Veritabanı kullanıcısını oluşturun.** psql'de ya da pgAdmin'in sorgu ekranında çalıştırın:

```sql
CREATE USER patiduragi WITH PASSWORD 'patiduragi' CREATEDB;
```

**4. `PatiDuragi/PatiDuragi` klasöründe sırayla çalıştırın:**

```bash
dotnet tool install --global dotnet-ef
dotnet user-secrets set "ConnectionStrings:PatiDb" "Host=localhost;Port=5432;Database=patiduragi;Username=patiduragi;Password=patiduragi"
dotnet ef database update
dotnet run
```

- `user-secrets`: bağlantı dizesi depoya girmez, her geliştiricinin kendi bilgisayarında saklanır.
- `dotnet ef database update`: `patiduragi` veritabanını ve tabloları oluşturur, örnek mahalle ve noktaları ekler.
  Yeni bir migration geldiğinde (`git pull` sonrası) bu komutu tekrar çalıştırmak yeterlidir.

Uygulama `http://localhost:5163` adresinde açılır. Visual Studio ile çalıştırmak için `PatiDuragi.sln` dosyasını açıp
F5'e basabilirsiniz (1–4. adımlar yine bir kez yapılmalıdır).

## Proje yapısı

```
PatiDuragi/
├── Models/        Bolge (mahalle ve sınırı), BeslemeNoktasi
├── Data/          PatiDbContext (tablolar, ilişkiler, örnek veri), OrnekSinirlar (örnek mahalle sınırları)
├── Migrations/    EF Core migration dosyaları
├── Services/      SinirServisi: mahalle sınırını OpenStreetMap'ten bulur, konumun hangi mahallede olduğunu hesaplar
└── Pages/
    ├── Bolgeler/  Mahalle listeleme, detay, ekleme, düzenleme, silme (ortak form: _BolgeFormu)
    ├── Noktalar/  Besleme noktası listeleme (arama/filtre), detay, ekleme, düzenleme, silme (ortak form: _NoktaFormu)
    └── Shared/    Ortak sayfa düzeni (_Layout) ve parçalar (_TurRozeti, _TurIkonu, _OnayDurumu, _Mesaj)
wwwroot/js/harita.js   Harita fonksiyonları: noktaları ve mahalle sınırlarını çizme, formda haritadan konum seçme
```

## Haftalık ilerleme

| Hafta | Lider | Durum |
|---|---|---|
| 1 | Süleyman Kılınç | Proje iskeleti, ortak düzen ve mobil menü, ana sayfa, bölge ve besleme noktası listeleme/detay sayfaları, noktaların OpenStreetMap haritasında gösterimi |
| 2 | Selman Yasin Zemheri | Bölge ve besleme noktası ekleme/düzenleme/silme formları; DataAnnotations ile istemci ve sunucu tarafı doğrulama (zorunlu alanlar, uzunluk, tür listesi, koordinat aralığı) ve depoya bakan ek kurallar (aynı ad, var olmayan bölge, içinde nokta olan bölgenin silinmemesi); model bağlama; işlem sonrası TempData mesajları |
| 3 | Süleyman Kılınç | Entity Framework Core ve PostgreSQL kurulumu; DbContext, bağlantı dizesinin user-secrets ile depo dışında tutulması; ilk migration ile tabloların ve örnek verinin oluşturulması; bellek içi verinin veritabanına taşınması; mahalle sınırlarının OpenStreetMap'ten alınması ve noktanın konumundan mahallesinin otomatik bulunması; nokta formunda haritaya tıklayarak konum seçme (mahalle sınırları ve seçilen konumun hangi mahalleye düştüğü gösterilir); besleme noktalarında ad/açıklama araması, tür ve onay durumu filtresi |
| 4 | Selman Yasin Zemheri | — |
| 5 | Süleyman Kılınç | — |
| 6 | Selman Yasin Zemheri | — |
| 7 | Süleyman Kılınç | — |
| 8 | Selman Yasin Zemheri | — |
| 9 | Süleyman Kılınç | — |
