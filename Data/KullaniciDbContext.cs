using Microsoft.EntityFrameworkCore;
using WebApplication7.Models;
public class KullaniciDbContext : DbContext
{
 
        public DbSet<Kullanici> Kullanicilar { get; set; }

        



    public KullaniciDbContext(DbContextOptions<KullaniciDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Tablo adını belirtmek isterseniz:
        modelBuilder.Entity<Kullanici>().ToTable("Kullanicilar");
        modelBuilder.Entity<Rezervasyon>().HasNoKey(); // Eğer birincil anahtar gerekliyse kaldırmayın

    }

}

