using System.Globalization;
using System.Text.Json;

namespace PatiDuragi.Services
{
    // OpenStreetMap'ten bulunan bir mahallenin bilgileri
    public record MahalleBilgisi(long OsmId, string Mahalle, string Ilce, string Sinir);

    // OpenStreetMap'in adres servisi (Nominatim, harici API) ile mahalle sınırını bulur
    // ve bir konumun bir sınırın içinde olup olmadığını hesaplar.
    public class SinirServisi
    {
        private readonly HttpClient _http;

        public SinirServisi(HttpClient http)
        {
            _http = http;
            _http.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
            _http.Timeout = TimeSpan.FromSeconds(10);
            // Nominatim'in kullanım kuralı: istek, uygulamayı tanıtan bir ad taşımalı
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("PatiDuragi-ders-projesi/1.0");
        }

        // Ankara'da verilen ilçedeki mahalleyi arar; bulamazsa null döner.
        // Servise ulaşılamazsa HttpRequestException ya da TaskCanceledException fırlatır.
        public async Task<MahalleBilgisi?> MahalleBul(string mahalle, string ilce)
        {
            var sorgu = Uri.EscapeDataString($"{mahalle}, {ilce}, Ankara");
            var json = await _http.GetStringAsync(
                $"search?q={sorgu}&format=jsonv2&addressdetails=1&polygon_geojson=1&limit=5");

            foreach (var sonuc in JsonDocument.Parse(json).RootElement.EnumerateArray())
            {
                var tur = sonuc.GetProperty("addresstype").GetString();
                var sekil = sonuc.GetProperty("geojson");
                var sekilTuru = sekil.GetProperty("type").GetString();
                var bulunanIlce = sonuc.GetProperty("address").TryGetProperty("town", out var town)
                    ? town.GetString() ?? "" : "";

                // Yalnızca gerçek mahalle: türü "suburb", sınırı çokgen ve ilçesi yazılanla aynı
                if (tur == "suburb" && (sekilTuru == "Polygon" || sekilTuru == "MultiPolygon") && AyniMi(bulunanIlce, ilce))
                {
                    return new MahalleBilgisi(
                        sonuc.GetProperty("osm_id").GetInt64(),
                        sonuc.GetProperty("name").GetString() ?? mahalle,
                        bulunanIlce,
                        sekil.GetRawText());
                }
            }

            return null;
        }

        // Konum sınırın içinde mi? Işın yöntemi: noktadan bir yöne çekilen çizgi
        // sınır çizgilerini tek sayıda kesiyorsa nokta içeridedir.
        public static bool IcindeMi(double enlem, double boylam, string sinir)
        {
            var sekil = JsonDocument.Parse(sinir).RootElement;
            var cokgenler = sekil.GetProperty("type").GetString() == "MultiPolygon"
                ? sekil.GetProperty("coordinates").EnumerateArray().ToList()
                : new List<JsonElement> { sekil.GetProperty("coordinates") };

            var icinde = false;
            foreach (var cokgen in cokgenler)
            {
                foreach (var halka in cokgen.EnumerateArray())
                {
                    // GeoJSON'da her köşe [boylam, enlem] sırasıyla yazılır
                    var k = halka.EnumerateArray()
                        .Select(n => (Boylam: n[0].GetDouble(), Enlem: n[1].GetDouble()))
                        .ToList();

                    for (int i = 0, j = k.Count - 1; i < k.Count; j = i++)
                    {
                        if ((k[i].Enlem > enlem) != (k[j].Enlem > enlem) &&
                            boylam < (k[j].Boylam - k[i].Boylam) * (enlem - k[i].Enlem) / (k[j].Enlem - k[i].Enlem) + k[i].Boylam)
                        {
                            icinde = !icinde;
                        }
                    }
                }
            }

            return icinde;
        }

        // Büyük-küçük harf ve Türkçe harf farkını saymayan karşılaştırma ("cankaya" = "Çankaya")
        private static bool AyniMi(string a, string b)
        {
            return Sadelestir(a) == Sadelestir(b);
        }

        private static string Sadelestir(string metin)
        {
            return metin.Trim().ToLower(new CultureInfo("tr-TR"))
                .Replace('ç', 'c').Replace('ğ', 'g').Replace('ı', 'i')
                .Replace('ö', 'o').Replace('ş', 's').Replace('ü', 'u');
        }
    }
}
