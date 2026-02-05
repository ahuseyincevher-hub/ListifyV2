using Microsoft.EntityFrameworkCore;
using Serilog;
using web_uyg.Data;
using web_uyg.Models;
using web_uyg.Services;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/listify-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Listify başlatılıyor...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllersWithViews();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<AlisverisListesiContext>(options =>
        options.UseSqlite(connectionString));

    builder.Services.AddScoped<IAlisverisService, AlisverisService>();
    builder.Services.AddScoped<IListeService, ListeService>();

    builder.Services.AddHttpClient();

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AlisverisListesiContext>();
            context.Database.Migrate();

            var listeler = context.AlisverisListeler.ToList();
            var varsayilanVar = listeler.Any(l => l.Ad == "Varsayılan Liste");
            var haftalikVar = listeler.Any(l => l.Ad == "Haftalık Pazar");

            if (!varsayilanVar)
            {
                context.AlisverisListeler.Add(new AlisverisListesi
                {
                    Ad = "Varsayılan Liste",
                    Aciklama = "Ana alışveriş listem",
                    Renk = "#0d6efd",
                    VarsayilanMi = true,
                    OlusturulmaTarihi = DateTime.Now
                });
            }
            if (!haftalikVar)
            {
                context.AlisverisListeler.Add(new AlisverisListesi
                {
                    Ad = "Haftalık Pazar",
                    Aciklama = "Bu hafta marketten alınacaklar",
                    Renk = "#198754",
                    VarsayilanMi = false,
                    OlusturulmaTarihi = DateTime.Now
                });
            }
            if (!varsayilanVar || !haftalikVar)
                context.SaveChangesAsync().Wait();

            if (!context.AlisverisListesi.AnyAsync().Result)
            {
                listeler = context.AlisverisListeler.OrderBy(l => l.Id).ToList();
                var varsayilanListeId = listeler.FirstOrDefault(l => l.VarsayilanMi)?.Id ?? listeler.First().Id;
                var digerListeId = listeler.FirstOrDefault(l => !l.VarsayilanMi)?.Id ?? varsayilanListeId;

                var mockUrunler = new List<AlisverisUrunu>
                {
                    new() { UrunAdi = "Süt", Miktar = 2, MiktarBirimi = "Litre", AlındiMi = false, ListeId = varsayilanListeId, KategoriId = 2, Fiyat = 45.50m, SiraNo = 1 },
                    new() { UrunAdi = "Ekmek", Miktar = 2, MiktarBirimi = "Adet", AlındiMi = true, ListeId = varsayilanListeId, KategoriId = 3, Fiyat = 15m, SiraNo = 2 },
                    new() { UrunAdi = "Yumurta", Miktar = 1, MiktarBirimi = "Koli", AlındiMi = false, ListeId = varsayilanListeId, KategoriId = 7, Fiyat = 120m, SiraNo = 3 },
                    new() { UrunAdi = "Domates", Miktar = 1, MiktarBirimi = "Kg", AlındiMi = true, ListeId = varsayilanListeId, KategoriId = 1, Fiyat = 35m, SiraNo = 4 },
                    new() { UrunAdi = "Peynir", Miktar = 500, MiktarBirimi = "Gram", AlındiMi = false, ListeId = varsayilanListeId, KategoriId = 2, Fiyat = 180m, SiraNo = 5 },
                    new() { UrunAdi = "Deterjan", Miktar = 1, MiktarBirimi = "Adet", AlındiMi = false, ListeId = varsayilanListeId, KategoriId = 4, Fiyat = 89.90m, SiraNo = 6 },
                    new() { UrunAdi = "Su", Miktar = 6, MiktarBirimi = "Adet", AlındiMi = true, ListeId = digerListeId, KategoriId = 5, Fiyat = 45m, SiraNo = 1 },
                    new() { UrunAdi = "Meyve", Miktar = 2, MiktarBirimi = "Kg", AlındiMi = false, ListeId = digerListeId, KategoriId = 1, Not = "Elma veya armut", SiraNo = 2 },
                    new() { UrunAdi = "Yoğurt", Miktar = 4, MiktarBirimi = "Adet", AlındiMi = false, ListeId = digerListeId, KategoriId = 2, Fiyat = 28m, SiraNo = 3 },
                    new() { UrunAdi = "Çikolata", Miktar = 1, MiktarBirimi = "Paket", AlındiMi = false, ListeId = digerListeId, KategoriId = 6, Fiyat = 25m, SiraNo = 4 }
                };

                foreach (var urun in mockUrunler)
                    context.AlisverisListesi.Add(urun);
                context.SaveChangesAsync().Wait();
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Veritabanı başlatılırken hata oluştu");
        }
    }

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Alisveris}/{action=Index}/{id?}");

    Log.Information("Listify başlatıldı");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Uygulama başlatılamadı");
}
finally
{
    Log.CloseAndFlush();
}
