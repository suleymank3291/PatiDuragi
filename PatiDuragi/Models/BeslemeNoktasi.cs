using System.ComponentModel.DataAnnotations;

namespace PatiDuragi.Models
{
    public class BeslemeNoktasi
    {
        // Formdaki tür seçenekleri ve doğrulama bu listeden beslenir
        public static readonly string[] Turler = { "Mama", "Su", "Mama ve Su" };

        public int Id { get; set; }

        [Required(ErrorMessage = "Nokta adı zorunludur.")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "Nokta adı 3-80 karakter olmalı.")]
        [Display(Name = "Nokta Adı")]
        public string Ad { get; set; } = "";

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [StringLength(300, MinimumLength = 10, ErrorMessage = "Açıklama 10-300 karakter olmalı.")]
        [Display(Name = "Açıklama")]
        public string Aciklama { get; set; } = "";

        [Required(ErrorMessage = "Tür seçmelisiniz.")]
        [RegularExpression("^(Mama|Su|Mama ve Su)$", ErrorMessage = "Tür Mama, Su ya da Mama ve Su olmalı.")]
        [Display(Name = "Tür")]
        public string Tur { get; set; } = "";

        // double? + Required: boş bırakılan alan "0" sayılmasın, hata versin
        [Required(ErrorMessage = "Enlem zorunludur.")]
        [Range(-90, 90, ErrorMessage = "Enlem -90 ile 90 arasında olmalı.")]
        [Display(Name = "Enlem")]
        public double? Enlem { get; set; }

        [Required(ErrorMessage = "Boylam zorunludur.")]
        [Range(-180, 180, ErrorMessage = "Boylam -180 ile 180 arasında olmalı.")]
        [Display(Name = "Boylam")]
        public double? Boylam { get; set; }

        [Display(Name = "Onaylı")]
        public bool Onayli { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Bölge seçmelisiniz.")]
        [Display(Name = "Bölge")]
        public int BolgeId { get; set; }
    }
}
