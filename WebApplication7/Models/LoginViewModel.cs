namespace WebApplication7.Models
{
    public class LoginViewModel
    {
        public Admin Admin { get; set; }
        public Kullanici Kullanici { get; set; }
        public string UserType { get; set; } // Kullanıcı türü seçimi için
    }
}
