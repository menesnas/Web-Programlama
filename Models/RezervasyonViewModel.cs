using WebApplication7.Models;


namespace WebApplication7.Models
{
    public class RezervasyonViewModel
    {
        public Rezervasyon Rezervasyon { get; set; }
        public List<Personel> Personeller { get; set; }
        public int SecilenPersonelId { get; set; } // Bu alan seçilen personeli tutar
    }




}


