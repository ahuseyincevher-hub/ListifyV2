using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_uyg.Data;

namespace web_uyg.Controllers;

public class IstatistikController : Controller
{
    private readonly AlisverisListesiContext _context;

    public IstatistikController(AlisverisListesiContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new Dictionary<string, object>();

        var tumUrunler = await _context.AlisverisListesi.ToListAsync();
        
        viewModel["ToplamUrun"] = tumUrunler.Count;
        viewModel["ToplamListe"] = await _context.AlisverisListeler.CountAsync();
        viewModel["ToplamKategori"] = await _context.Kategoriler.CountAsync();
        viewModel["ToplamFavori"] = await _context.FavoriUrunler.CountAsync();

        var alinanUrunSayisi = tumUrunler.Count(u => u.AlındiMi);
        viewModel["AlinanUrun"] = alinanUrunSayisi;
        viewModel["AlinanUrunOrani"] = tumUrunler.Count > 0 
            ? (double)alinanUrunSayisi / tumUrunler.Count * 100 
            : 0;

        var toplamTutar = tumUrunler.Where(u => u.Fiyat.HasValue).Sum(u => u.Fiyat ?? 0);
        viewModel["ToplamTutar"] = toplamTutar;

        var ortalimaFiyat = tumUrunler.Where(u => u.Fiyat.HasValue).Any()
            ? tumUrunler.Where(u => u.Fiyat.HasValue).Average(u => u.Fiyat ?? 0)
            : 0;
        viewModel["OrtalamaFiyat"] = ortalimaFiyat;

        viewModel["KategoriBazliUrunler"] = await _context.Kategoriler
            .Where(k => k.Urunler.Any())
            .Select(k => new
            {
                k.Ad,
                UrunSayisi = k.Urunler.Count
            })
            .OrderByDescending(k => k.UrunSayisi)
            .ToListAsync();

        viewModel["EnCokEklenenUrunler"] = tumUrunler
            .GroupBy(u => u.UrunAdi)
            .Select(g => new
            {
                UrunAdi = g.Key,
                Sayi = g.Count()
            })
            .OrderByDescending(x => x.Sayi)
            .Take(5)
            .ToList();

        viewModel["EnPahaliUrunler"] = tumUrunler
            .Where(u => u.Fiyat.HasValue)
            .OrderByDescending(u => u.Fiyat)
            .Take(5)
            .Select(u => new
            {
                u.UrunAdi,
                u.Fiyat
            })
            .ToList();

        viewModel["ListeBazliIstatistikler"] = await _context.AlisverisListeler
            .Select(l => new
            {
                l.Id,
                l.Ad,
                UrunSayisi = l.Urunler.Count,
                AlinanUrunSayisi = l.Urunler.Count(u => u.AlındiMi),
                ToplamTutar = l.Urunler.Where(u => u.Fiyat.HasValue).Sum(u => u.Fiyat ?? 0)
            })
            .OrderByDescending(l => l.UrunSayisi)
            .ToListAsync();

        viewModel["SonEklenenler"] = tumUrunler
            .OrderByDescending(u => u.EklenmeTarihi)
            .Take(5)
            .Select(u => new
            {
                u.UrunAdi,
                u.EklenmeTarihi,
                u.ListeId
            })
            .ToList();

        return View(viewModel);
    }
}
