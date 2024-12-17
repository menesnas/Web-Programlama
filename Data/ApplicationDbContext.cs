using Microsoft.EntityFrameworkCore;
using WebApplication7.Models;

namespace WebApplication7.Data
{
    public class MyCustomDbContext : DbContext
    {
        public MyCustomDbContext(DbContextOptions<MyCustomDbContext> options)
            : base(options)
        {
        }
        public DbSet<Personel> Personeller { get; set; }
        public DbSet<Rezervasyon> Rezervasyonlar { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
    }
}
