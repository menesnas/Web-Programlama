using System.ComponentModel.DataAnnotations;

namespace WebApplication7.Models
{
    public class SacModeli
    {
        [Key] // Birincil anahtar
        public int Id { get; set; }
        public string İsim { get; set; }
        public int Ucret { get; set; }
    }
}
