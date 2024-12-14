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

        // PersonelId, personelin kimliğini belirtir
        public int PersonelId { get; set; } // Foreign key

        // Personel nesnesi, referans olarak eklenir
        public Personel Personel { get; set; } // Navigation property
    }

}
