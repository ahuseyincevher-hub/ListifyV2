using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_uyg.Data;
using web_uyg.Models;

namespace web_uyg.Controllers;

public class KategoriController : Controller
{
    private readonly AlisverisListesiContext _context;

    public KategoriController(AlisverisListesiContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var kategoriler = await _context.Kategoriler
            .Include(k => k.Urunler)
            .ToListAsync();
        return View(kategoriler);
    }

    public IActionResult Ekle()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Ekle(Kategori kategori)
    {
        if (ModelState.IsValid)
        {
            _context.Add(kategori);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Kategori başarıyla eklendi";
            return RedirectToAction(nameof(Index));
        }
        return View(kategori);
    }

    public async Task<IActionResult> Duzenle(int? id)
    {
        if (id == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var kategori = await _context.Kategoriler.FindAsync(id);
        if (kategori == null)
        {
            TempData["Error"] = "Kategori bulunamadı.";
            return RedirectToAction(nameof(Index));
        }
        return View(kategori);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(int id, Kategori kategori)
    {
        if (id != kategori.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(kategori);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Kategori başarıyla güncellendi";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Kategoriler.AnyAsync(k => k.Id == id))
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
        return View(kategori);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(int id)
    {
        var kategori = await _context.Kategoriler.FindAsync(id);
        if (kategori == null)
        {
            TempData["Error"] = "Kategori bulunamadı";
            return RedirectToAction(nameof(Index));
        }

        var urunSayisi = await _context.AlisverisListesi.CountAsync(u => u.KategoriId == id);
        if (urunSayisi > 0)
        {
            TempData["Error"] = $"Bu kategoride {urunSayisi} ürün var. Önce ürünleri başka kategoriye taşıyın.";
            return RedirectToAction(nameof(Index));
        }

        _context.Kategoriler.Remove(kategori);
        await _context.SaveChangesAsync();
        
        TempData["Success"] = "Kategori başarıyla silindi";
        return RedirectToAction(nameof(Index));
    }
}
