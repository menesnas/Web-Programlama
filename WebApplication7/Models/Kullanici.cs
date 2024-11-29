using System.ComponentModel.DataAnnotations;

namespace WebApplication7.Models
{
    public class Kullanici
    {
        [Required(ErrorMessage = "Lütfen İsminizi Giriniz")]
        public string Ad { get; set; }


        [Required(ErrorMessage = "Lütfen Soyisminizi Giriniz")]
        public string Soyad { get; set; }

        [EmailAddress(ErrorMessage = "Geçerli Mail Adresi Giriniz")]
        public string Email { get; set; }

        [MinLength(8)]
        public string Sifre { get; set; }
    }
}
