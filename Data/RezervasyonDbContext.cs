using Microsoft.EntityFrameworkCore;
using WebApplication7.Models;

public class RezervasyonDbContext : DbContext
{

    public RezervasyonDbContext(DbContextOptions<RezervasyonDbContext> options) : base(options) { }

    public DbSet<Rezervasyon> Rezervasyonlar { get; set; }
    public DbSet<Personel> Personeller { get; set; }


}
