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
- Bootstrap 5 ve Bootstrap Icons (mobil uyumlu arayüz)
- Leaflet + OpenStreetMap (harici harita servisi)
- PostgreSQL + Entity Framework Core (3. haftadan itibaren)

## Çalıştırma

Gereksinim: [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

```bash
git clone https://github.com/suleymank3291/PatiDuragi.git
cd PatiDuragi/PatiDuragi
dotnet run
```

Uygulama `http://localhost:5163` adresinde açılır. Visual Studio ile çalıştırmak için `PatiDuragi.sln` dosyasını açıp F5'e basmanız yeterlidir.

> Şu an veriler bellekte tutulmaktadır (`Data/` klasöründeki depolar). Veritabanı kurulumu ve migration adımları
> 3. haftada bu bölüme eklenecektir.

## Proje yapısı

```
PatiDuragi/
├── Models/        Bolge, BeslemeNoktasi sınıfları
├── Data/          Bellek içi örnek veri depoları (BolgeDeposu, NoktaDeposu)
└── Pages/
    ├── Bolgeler/  Bölge listesi ve detay sayfası
    ├── Noktalar/  Besleme noktası listesi ve detay sayfası
    └── Shared/    Ortak sayfa düzeni (_Layout) ve parçalar (_Harita, _TurRozeti, _OnayDurumu)
wwwroot/js/harita.js   Noktaları haritada işaretleyen fonksiyon
```

## Haftalık ilerleme

| Hafta | Lider | Durum |
|---|---|---|
| 1 | Süleyman Kılınç | Proje iskeleti, ortak düzen ve mobil menü, ana sayfa, bölge ve besleme noktası listeleme/detay sayfaları, noktaların OpenStreetMap haritasında gösterimi |
| 2 | Selman Yasin Zemheri | — |
| 3 | Süleyman Kılınç | — |
| 4 | Selman Yasin Zemheri | — |
| 5 | Süleyman Kılınç | — |
| 6 | Selman Yasin Zemheri | — |
| 7 | Süleyman Kılınç | — |
| 8 | Selman Yasin Zemheri | — |
| 9 | Süleyman Kılınç | — |
