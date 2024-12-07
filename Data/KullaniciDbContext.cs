using Microsoft.EntityFrameworkCore;
using WebApplication7.Models;

public class KullaniciDbContext : DbContext
{
    public KullaniciDbContext(DbContextOptions<KullaniciDbContext> options)
        : base(options) { }

    public DbSet<Kullanici> Kullanicilar { get; set; }
}
