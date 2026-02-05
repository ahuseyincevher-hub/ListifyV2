using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_uyg.Models;
using web_uyg.Data;

namespace web_uyg.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AlisverisListesiContext _context;

    public HomeController(ILogger<HomeController> logger, AlisverisListesiContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var varsayilanListe = await _context.AlisverisListeler
            .FirstOrDefaultAsync(l => l.VarsayilanMi);
        
        var tumListeler = await _context.AlisverisListeler
            .OrderByDescending(l => l.VarsayilanMi)
            .ThenByDescending(l => l.SonDegistirilmeTarihi)
            .Take(5)
            .ToListAsync();
        
        var aktifListeId = varsayilanListe?.Id ?? 0;
        
        var aktifListeUrunleri = await _context.AlisverisListesi
            .Include(u => u.Kategori)
            .Where(u => u.ListeId == aktifListeId)
            .OrderBy(u => u.SiraNo)
            .ThenByDescending(u => u.EklenmeTarihi)
            .Take(10)
            .ToListAsync();
        
        var istatistik = new Services.ListeIstatistik();
        
        if (aktifListeId > 0)
        {
            var tumUrunler = await _context.AlisverisListesi
                .Where(u => u.ListeId == aktifListeId)
                .ToListAsync();
            
            istatistik = new Services.ListeIstatistik
            {
                ToplamUrun = tumUrunler.Count,
                AlinanUrun = tumUrunler.Count(u => u.AlındiMi),
                AlinmayanUrun = tumUrunler.Count(u => !u.AlındiMi),
                ToplamTutar = tumUrunler.Where(u => u.Fiyat.HasValue).Sum(u => (decimal)(u.Fiyat ?? 0m)),
                TamamlanmaOrani = tumUrunler.Count > 0 
                    ? ((double)tumUrunler.Count(u => u.AlındiMi) / tumUrunler.Count * 100) 
                    : 0
            };
        }
        
        var favoriUrunler = await _context.FavoriUrunler
            .OrderByDescending(f => f.EklenmeTarihi)
            .Take(6)
            .ToListAsync();
        
        ViewBag.VarsayilanListe = varsayilanListe;
        ViewBag.TumListeler = tumListeler;
        ViewBag.AktifListeId = aktifListeId;
        ViewBag.Istatistik = istatistik;
        ViewBag.FavoriUrunler = favoriUrunler;
        
        return View(aktifListeUrunleri);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
