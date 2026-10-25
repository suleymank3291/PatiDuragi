namespace PatiDuragi.Models
{
    public class BeslemeNoktasi
    {
        public int Id { get; set; }
        public string Ad { get; set; } = "";
        public string Aciklama { get; set; } = "";
        public string Tur { get; set; } = "";   // "Mama", "Su" veya "Mama ve Su"
        public double Enlem { get; set; }
        public double Boylam { get; set; }
        public bool Onayli { get; set; }
        public int BolgeId { get; set; }
    }
}
