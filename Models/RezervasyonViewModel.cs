using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using WebApplication7.Models;


namespace WebApplication7.Models
{
    public class RezervasyonViewModel
    {
        public Rezervasyon Rezervasyon { get; set; }
        public List<Personel> Personeller { get; set; }
        public List<SacModeli> SacModelleri { get; set; }
        public string? ImageUrl { get; set; }
        public string Prompt {  get; set; }

    }
}


