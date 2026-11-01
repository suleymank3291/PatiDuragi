using System.ComponentModel.DataAnnotations;

namespace PatiDuragi.Models
{
    public class Bolge
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Bölge adı zorunludur.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Bölge adı 2-50 karakter olmalı.")]
        [Display(Name = "Bölge Adı")]
        public string Ad { get; set; } = "";

        [Required(ErrorMessage = "İlçe zorunludur.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "İlçe 2-50 karakter olmalı.")]
        [RegularExpression(@"^[a-zA-ZçğıöşüÇĞİÖŞÜ ]+$", ErrorMessage = "İlçe adı yalnızca harflerden oluşmalı.")]
        [Display(Name = "İlçe")]
        public string Ilce { get; set; } = "";
    }
}
