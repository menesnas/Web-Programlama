using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication7.Models
{
    public class Rezervasyon
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public DateTime Tarih { get; set; }
        public string Telefon { get; set; }

      
    }


}
