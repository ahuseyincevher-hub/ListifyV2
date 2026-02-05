using Microsoft.EntityFrameworkCore;
using web_uyg.Data;
using web_uyg.Models;

namespace web_uyg.Services;

public class ListeService : IListeService
{
    private readonly AlisverisListesiContext _context;

    public ListeService(AlisverisListesiContext context)
    {
        _context = context;
    }

    public async Task<List<AlisverisListesi>> TumListeler()
    {
        return await _context.AlisverisListeler
            .Include(l => l.Urunler)
            .OrderByDescending(l => l.VarsayilanMi)
            .ThenByDescending(l => l.SonDegistirilmeTarihi)
            .ThenByDescending(l => l.OlusturulmaTarihi)
            .ToListAsync();
    }

    public async Task<AlisverisListesi?> GetListeById(int id)
    {
        return await _context.AlisverisListeler
            .Include(l => l.Urunler)
            .ThenInclude(u => u.Kategori)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<AlisverisListesi?> GetVarsayilanListe()
    {
        return await _context.AlisverisListeler
            .Include(l => l.Urunler)
            .ThenInclude(u => u.Kategori)
            .FirstOrDefaultAsync(l => l.VarsayilanMi);
    }

    public async Task<bool> ListeEkle(AlisverisListesi liste)
    {
        try
        {
            liste.OlusturulmaTarihi = DateTime.Now;
            liste.SonDegistirilmeTarihi = DateTime.Now;

            if (!await _context.AlisverisListeler.AnyAsync())
            {
                liste.VarsayilanMi = true;
            }

            _context.AlisverisListeler.Add(liste);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ListeGuncelle(AlisverisListesi liste)
    {
        try
        {
            liste.SonDegistirilmeTarihi = DateTime.Now;
            
            // Eğer bu liste varsayılan yapılıyorsa, diğerlerinin varsayılan özelliğini kaldır
            if (liste.VarsayilanMi)
            {
                var digerListeler = await _context.AlisverisListeler
                    .Where(l => l.Id != liste.Id)
                    .ToListAsync();
                    
                foreach (var digerListe in digerListeler)
                {
                    digerListe.VarsayilanMi = false;
                }
            }
            
            _context.Update(liste);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ListeSil(int id)
    {
        try
        {
            var liste = await _context.AlisverisListeler.FindAsync(id);
            if (liste == null) return false;

            if (liste.VarsayilanMi)
            {
                var baskaListeVar = await _context.AlisverisListeler
                    .AnyAsync(l => l.Id != id);
                if (baskaListeVar)
                {
                    var yeniVarsayilan = await _context.AlisverisListeler
                        .FirstOrDefaultAsync(l => l.Id != id);
                    if (yeniVarsayilan != null)
                        yeniVarsayilan.VarsayilanMi = true;
                }
            }

            _context.AlisverisListeler.Remove(liste);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> VarsayilanListeAyarla(int listeId)
    {
        try
        {
            var tumListeler = await _context.AlisverisListeler.ToListAsync();
            foreach (var l in tumListeler)
            {
                l.VarsayilanMi = l.Id == listeId;
            }
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<ListeIstatistik> GetListeIstatistik(int listeId)
    {
        var urunler = await _context.AlisverisListesi
            .Where(u => u.ListeId == listeId)
            .ToListAsync();

        var toplamUrun = urunler.Count;
        var alinanUrun = urunler.Count(u => u.AlındiMi);
        var alinmayanUrun = toplamUrun - alinanUrun;
        var toplamTutar = urunler.Where(u => u.Fiyat.HasValue).Sum(u => u.Fiyat!.Value * u.Miktar);

        return new ListeIstatistik
        {
            ToplamUrun = toplamUrun,
            AlinanUrun = alinanUrun,
            AlinmayanUrun = alinmayanUrun,
            ToplamTutar = toplamTutar,
            TamamlanmaOrani = toplamUrun > 0 ? (double)alinanUrun / toplamUrun * 100 : 0
        };
    }

    public async Task<int> ListeKopyala(int listeId, string yeniAd, string? yeniAciklama, string? yeniRenk)
    {
        try
        {
            var originalListe = await _context.AlisverisListeler
                .Include(l => l.Urunler)
                .FirstOrDefaultAsync(l => l.Id == listeId);

            if (originalListe == null) return 0;

            var yeniListe = new AlisverisListesi
            {
                Ad = yeniAd,
                Aciklama = yeniAciklama,
                Renk = yeniRenk ?? originalListe.Renk,
                VarsayilanMi = false,
                OlusturulmaTarihi = DateTime.Now,
                SonDegistirilmeTarihi = DateTime.Now,
                Urunler = new List<AlisverisUrunu>()
            };

            _context.AlisverisListeler.Add(yeniListe);
            await _context.SaveChangesAsync();

            foreach (var urun in originalListe.Urunler)
            {
                var kopyaUrun = new AlisverisUrunu
                {
                    UrunAdi = urun.UrunAdi,
                    Miktar = urun.Miktar,
                    MiktarBirimi = urun.MiktarBirimi,
                    AlındiMi = false,
                    KategoriId = urun.KategoriId,
                    ListeId = yeniListe.Id,
                    ResimUrl = urun.ResimUrl,
                    Fiyat = urun.Fiyat,
                    Not = urun.Not,
                    SiraNo = urun.SiraNo,
                    EklenmeTarihi = DateTime.Now
                };
                _context.AlisverisListesi.Add(kopyaUrun);
            }

            await _context.SaveChangesAsync();
            return yeniListe.Id;
        }
        catch
        {
            return 0;
        }
    }
}
