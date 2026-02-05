using Microsoft.EntityFrameworkCore;
using web_uyg.Data;
using web_uyg.Models;

namespace web_uyg.Services;

public class AlisverisService : IAlisverisService
{
    private readonly AlisverisListesiContext _context;

    public AlisverisService(AlisverisListesiContext context)
    {
        _context = context;
    }

    public async Task<List<AlisverisUrunu>> GetUrunlerByListeId(int listeId)
    {
        return await _context.AlisverisListesi
            .Include(u => u.Kategori)
            .Where(u => u.ListeId == listeId)
            .OrderBy(u => u.SiraNo)
            .ThenByDescending(u => u.EklenmeTarihi)
            .ToListAsync();
    }

    public async Task<AlisverisUrunu?> GetUrunById(int id)
    {
        return await _context.AlisverisListesi
            .Include(u => u.Kategori)
            .Include(u => u.Liste)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<UrunEklemeSonuc> UrunEkle(AlisverisUrunu urun)
    {
        try
        {
            urun.EklenmeTarihi = DateTime.Now;
            urun.AlındiMi = false;

            var maxSira = await _context.AlisverisListesi
                .Where(u => u.ListeId == urun.ListeId)
                .MaxAsync(u => (int?)u.SiraNo) ?? 0;

            urun.SiraNo = maxSira + 1;

            _context.AlisverisListesi.Add(urun);
            await _context.SaveChangesAsync();

            return new UrunEklemeSonuc { BasariliMi = true, Urun = urun };
        }
        catch (Exception ex)
        {
            return new UrunEklemeSonuc { BasariliMi = false, HataMesaji = ex.Message };
        }
    }

    public async Task<bool> UrunGuncelle(AlisverisUrunu urun)
    {
        try
        {
            _context.Update(urun);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UrunSil(int id)
    {
        try
        {
            var urun = await _context.AlisverisListesi.FindAsync(id);
            if (urun == null) return false;

            _context.AlisverisListesi.Remove(urun);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DurumGuncelle(int id)
    {
        try
        {
            var urun = await _context.AlisverisListesi.FindAsync(id);
            if (urun == null) return false;

            urun.AlındiMi = !urun.AlındiMi;
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<ListeyiTemizleSonuc> ListeyiTemizle(int listeId, bool sadeceAlinanlar)
    {
        try
        {
            var urunler = sadeceAlinanlar
                ? _context.AlisverisListesi.Where(u => u.ListeId == listeId && u.AlındiMi)
                : _context.AlisverisListesi.Where(u => u.ListeId == listeId);

            var silinenUrunler = urunler.ToList();
            _context.AlisverisListesi.RemoveRange(silinenUrunler);
            await _context.SaveChangesAsync();

            return new ListeyiTemizleSonuc { BasariliMi = true, SilinenUrunSayisi = silinenUrunler.Count };
        }
        catch
        {
            return new ListeyiTemizleSonuc { BasariliMi = false, SilinenUrunSayisi = 0 };
        }
    }

    public async Task<bool> UrunSiraGuncelle(int id, int yeniSira)
    {
        try
        {
            var urun = await _context.AlisverisListesi.FindAsync(id);
            if (urun == null) return false;

            urun.SiraNo = yeniSira;
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> TopluUrunSil(List<int> urunIdleri)
    {
        try
        {
            var urunler = await _context.AlisverisListesi
                .Where(u => urunIdleri.Contains(u.Id))
                .ToListAsync();

            if (!urunler.Any()) return false;

            _context.AlisverisListesi.RemoveRange(urunler);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> TopluDurumGuncelle(List<int> urunIdleri, bool alinanMi)
    {
        try
        {
            var urunler = await _context.AlisverisListesi
                .Where(u => urunIdleri.Contains(u.Id))
                .ToListAsync();

            if (!urunler.Any()) return false;

            foreach (var urun in urunler)
            {
                urun.AlındiMi = alinanMi;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> TopluListeTasi(List<int> urunIdleri, int hedefListeId)
    {
        try
        {
            var urunler = await _context.AlisverisListesi
                .Where(u => urunIdleri.Contains(u.Id))
                .ToListAsync();

            if (!urunler.Any()) return false;

            var maxSira = await _context.AlisverisListesi
                .Where(u => u.ListeId == hedefListeId)
                .MaxAsync(u => (int?)u.SiraNo) ?? 0;

            foreach (var urun in urunler)
            {
                urun.ListeId = hedefListeId;
                urun.SiraNo = ++maxSira;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
