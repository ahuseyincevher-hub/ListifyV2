using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_uyg.Data;
using web_uyg.Models;
using System.ComponentModel.DataAnnotations;

namespace web_uyg.Controllers;

public class FavoriController : Controller
{
    private readonly AlisverisListesiContext _context;

    public FavoriController(AlisverisListesiContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var favoriler = await _context.FavoriUrunler
            .OrderByDescending(f => f.EklenmeTarihi)
            .ToListAsync();
        return View(favoriler);
    }

    public IActionResult Ekle()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Ekle(FavoriUrun favoriUrun)
    {
        if (ModelState.IsValid)
        {
            favoriUrun.EklenmeTarihi = DateTime.Now;
            _context.Add(favoriUrun);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Favori başarıyla eklendi";
            return RedirectToAction(nameof(Index));
        }
        return View(favoriUrun);
    }

    public async Task<IActionResult> Duzenle(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var favoriUrun = await _context.FavoriUrunler.FindAsync(id);
        if (favoriUrun == null)
        {
            return NotFound();
        }
        return View(favoriUrun);
    }

    [HttpPost]
    public async Task<IActionResult> Duzenle(int id, FavoriUrun favoriUrun)
    {
        if (id != favoriUrun.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(favoriUrun);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Favori başarıyla güncellendi";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.FavoriUrunler.AnyAsync(f => f.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(favoriUrun);
    }

    [HttpPost]
    public async Task<IActionResult> Sil(int id)
    {
        var favoriUrun = await _context.FavoriUrunler.FindAsync(id);
        if (favoriUrun != null)
        {
            _context.FavoriUrunler.Remove(favoriUrun);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Favori başarıyla silindi";
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ListeyeEkle(int id)
    {
        var favoriUrun = await _context.FavoriUrunler.FindAsync(id);
        if (favoriUrun != null)
        {
            var varsayilanListe = await _context.AlisverisListeler.FirstOrDefaultAsync(l => l.VarsayilanMi);
            if (varsayilanListe == null)
            {
                varsayilanListe = await _context.AlisverisListeler.FirstOrDefaultAsync();
            }

            if (varsayilanListe != null)
            {
                var alisverisUrunu = new AlisverisUrunu
                {
                    UrunAdi = favoriUrun.UrunAdi,
                    Miktar = favoriUrun.VarsayilanMiktar ?? 1,
                    AlındiMi = false,
                    ListeId = varsayilanListe.Id,
                    EklenmeTarihi = DateTime.Now
                };

                _context.AlisverisListesi.Add(alisverisUrunu);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"{favoriUrun.UrunAdi} listeye eklendi";
            }
            else
            {
                TempData["Error"] = "Önce bir liste oluşturun";
            }
        }
        return RedirectToAction(nameof(Index));
    }
}
