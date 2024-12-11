using System.ComponentModel.DataAnnotations;

namespace WebApplication7.Models
{
    public class Personel
    {
        public int Id { get; set; } // Otomatik artan birincil anahtar

        [Required]
        public string Ad { get; set; }

        [Required]
        public string Soyad { get; set; }

        [Required]
        public string CalistigiSaat { get; set; }

        [Required]
        public string GunlukKazandirdigiPara { get; set; }

    }
}
