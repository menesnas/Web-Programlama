using System.ComponentModel.DataAnnotations.Schema;
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
        public string? ImageUrl { get; set; }


        [ForeignKey("SacModeliId")]
        public SacModeli Sacmodel { get; set; } // Bağlantılı SacModeli nesnesi
        public int SacModeliId { get; set; } // Foreign key

        [ForeignKey("PersonelId")]
        public Personel Personel { get; set; } // Bağlantılı Personel nesnesi
        
        // PersonelId, personelin kimliğini belirtir
        public int PersonelId { get; set; } // Foreign key

    }

}
