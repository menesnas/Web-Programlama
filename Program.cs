using Microsoft.EntityFrameworkCore;

namespace WebApplication7
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Personel DbContext ekle
            builder.Services.AddDbContext<PersonelDbContext>(options =>
                  options.UseSqlServer(builder.Configuration.GetConnectionString("PersonelConnection")));

            builder.Services.AddDbContext<KullaniciDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("KullaniciConnection")));


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
