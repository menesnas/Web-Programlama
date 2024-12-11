using WebApplication7.Models;

namespace WebApplication7.Models
{
    public class Rezervasyon
    {
        public int Id { get; set; } // Birincil anahtar
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public DateTime Tarih { get; set; }
        public string Telefon { get; set; }
        public int PersonelId { get; set; } // Personel'i belirlemek için
        public Personel Personel { get; set; } // Personel nesnesi
    }
}