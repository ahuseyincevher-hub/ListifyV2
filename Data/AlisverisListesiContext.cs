using Microsoft.EntityFrameworkCore;
using web_uyg.Models;

namespace web_uyg.Data;

public class AlisverisListesiContext : DbContext
{
    public AlisverisListesiContext(DbContextOptions<AlisverisListesiContext> options)
        : base(options)
    {
    }

    public DbSet<AlisverisUrunu> AlisverisListesi { get; set; }
    public DbSet<AlisverisListesi> AlisverisListeler { get; set; }
    public DbSet<Kategori> Kategoriler { get; set; }
    public DbSet<FavoriUrun> FavoriUrunler { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AlisverisUrunu>()
            .Property(p => p.UrunAdi)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<AlisverisUrunu>()
            .Property(p => p.Miktar)
            .IsRequired();

        modelBuilder.Entity<AlisverisUrunu>()
            .HasOne(u => u.Kategori)
            .WithMany(k => k.Urunler)
            .HasForeignKey(u => u.KategoriId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<AlisverisUrunu>()
            .HasOne(u => u.Liste)
            .WithMany(l => l.Urunler)
            .HasForeignKey(u => u.ListeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AlisverisListesi>()
            .HasIndex(l => l.VarsayilanMi);

        modelBuilder.Entity<AlisverisUrunu>()
            .HasIndex(u => new { u.ListeId, u.SiraNo });

        modelBuilder.Entity<Kategori>().HasData(
            new Kategori { Id = 1, Ad = "Meyve & Sebze", Aciklama = "Taze meyve ve sebzeler" },
            new Kategori { Id = 2, Ad = "Süt & Süt Ürünleri", Aciklama = "Süt, peynir, yoğurt vb." },
            new Kategori { Id = 3, Ad = "Temel Gıda", Aciklama = "Bakliyat, un, şeker vb." },
            new Kategori { Id = 4, Ad = "Temizlik", Aciklama = "Temizlik malzemeleri" },
            new Kategori { Id = 5, Ad = "İçecek", Aciklama = "Sıcak ve soğuk içecekler" },
            new Kategori { Id = 6, Ad = "Tatlı", Aciklama = "Tatlı ve tatlı ürünler" },
            new Kategori { Id = 7, Ad = "Kahvaltı", Aciklama = "Kahvaltı için gerekli ürünler" },
            new Kategori { Id = 8, Ad = "Yiyecek", Aciklama = "Yiyecek ürünleri" },
            new Kategori { Id = 9, Ad = "Süs Eşyası", Aciklama = "Süs eşyaları" }
        );
    }
}