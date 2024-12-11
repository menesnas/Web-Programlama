using WebApplication7.Models;


namespace WebApplication7.Models
{
    public class RezervasyonViewModel
    {
        public int? SecilenPersonelId { get; set; }
        public Rezervasyon Rezervasyon { get; set; }

        public List<Personel> Personeller { get; set; }

        public bool IsValidPersonelId => SecilenPersonelId.HasValue;
    }

}


