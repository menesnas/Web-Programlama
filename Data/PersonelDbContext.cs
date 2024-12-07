using Microsoft.EntityFrameworkCore;
using WebApplication7.Models;

public class PersonelDbContext : DbContext
{
    public PersonelDbContext(DbContextOptions<PersonelDbContext> options)
        : base(options) { }

    public DbSet<Personel> Personeller { get; set; }
    public DbSet<Rezervasyon> Rezervasyonlar { get; set; } // Rezervasyon tablosu

}
