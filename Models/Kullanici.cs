using System.ComponentModel.DataAnnotations;

namespace WebApplication7.Models
{
    public class Kullanici
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad gereklidir.")]
        public string Ad { get; set; }

        [Required(ErrorMessage = "Soyad gereklidir.")]
        public string Soyad { get; set; }

        [Required(ErrorMessage = "E-posta gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        public string Mail { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Sifre { get; set; }
    }
}
