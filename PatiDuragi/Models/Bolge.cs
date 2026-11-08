using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PatiDuragi.Models
{
    // Bölge = Ankara'daki resmî bir mahalle. Sınırı OpenStreetMap'ten alınır.
    public class Bolge
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İlçe zorunludur.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "İlçe 2-50 karakter olmalı.")]
        [RegularExpression(@"^[a-zA-ZçğıöşüÇĞİÖŞÜ ]+$", ErrorMessage = "İlçe adı yalnızca harflerden oluşmalı.")]
        [Display(Name = "İlçe")]
        public string Ilce { get; set; } = "";

        [Required(ErrorMessage = "Mahalle zorunludur.")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "Mahalle 2-60 karakter olmalı.")]
        [Display(Name = "Mahalle")]
        public string Mahalle { get; set; } = "";

        // Aşağıdaki iki alan formdan gelmez; sunucu OpenStreetMap'ten doldurur
        [BindNever, ValidateNever]
        public long OsmId { get; set; }

        [BindNever, ValidateNever]
        public string Sinir { get; set; } = "";   // mahalle sınırı, GeoJSON metni

        // Bire-çok ilişki: bir bölgede birden çok besleme noktası olur
        public List<BeslemeNoktasi> Noktalar { get; set; } = new();

        // Ekranda gösterilecek tam ad; veritabanında sütun değildir (yalnızca okunur)
        public string TamAd => $"{Ilce} / {Mahalle}";
    }
}
