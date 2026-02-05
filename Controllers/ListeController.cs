using Microsoft.AspNetCore.Mvc;
using web_uyg.Services;
using web_uyg.Models;

namespace web_uyg.Controllers;

public class ListeController : Controller
{
    private readonly IListeService _listeService;
    private readonly ILogger<ListeController> _logger;

    public ListeController(IListeService listeService, ILogger<ListeController> logger)
    {
        _listeService = listeService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var listeler = await _listeService.TumListeler();
        return View(listeler);
    }

    public IActionResult Ekle()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ekle([FromForm] AlisverisListesi liste)
    {
        if (!ModelState.IsValid) return View(liste);

        var basarili = await _listeService.ListeEkle(liste);
        if (basarili)
        {
            _logger.LogInformation("Yeni liste oluşturuldu: {ListeAdi}", liste.Ad);
            TempData["Success"] = "Liste başarıyla oluşturuldu";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "Liste oluşturulurken hata oluştu");
        return View(liste);
    }

    public async Task<IActionResult> Duzenle(int id)
    {
        var liste = await _listeService.GetListeById(id);
        if (liste == null) return NotFound();

        return View(liste);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(int id, [FromForm] AlisverisListesi liste)
    {
        if (id != liste.Id) return NotFound();

        if (!ModelState.IsValid) return View(liste);

        var basarili = await _listeService.ListeGuncelle(liste);
        if (basarili)
        {
            _logger.LogInformation("Liste güncellendi: {ListeAdi}", liste.Ad);
            TempData["Success"] = "Liste başarıyla güncellendi";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "Güncelleme başarısız");
        return View(liste);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(int id)
    {
        var basarili = await _listeService.ListeSil(id);
        if (basarili)
        {
            _logger.LogInformation("Liste silindi: ID {ListeId}", id);
            TempData["Success"] = "Liste başarıyla silindi";
            return Json(new { success = true, message = "Liste başarıyla silindi" });
        }

        TempData["Error"] = "Liste silinirken hata oluştu";
        return Json(new { success = false, message = "Liste silinirken hata oluştu" });
    }

    [HttpPost]
    public async Task<IActionResult> VarsayilanAyarla(int id)
    {
        var basarili = await _listeService.VarsayilanListeAyarla(id);
        if (basarili)
        {
            _logger.LogInformation("Varsayılan liste değiştirildi: ID {ListeId}", id);
            return Json(new { success = true, message = "Varsayılan liste ayarlandı" });
        }

        return Json(new { success = false, message = "İşlem başarısız" });
    }

    public async Task<IActionResult> Detay(int id)
    {
        var liste = await _listeService.GetListeById(id);
        if (liste == null) return NotFound();

        var istatistik = await _listeService.GetListeIstatistik(id);
        ViewBag.Istatistik = istatistik;

        return View(liste);
    }

    public async Task<IActionResult> Kopyala(int id)
    {
        var liste = await _listeService.GetListeById(id);
        if (liste == null) return NotFound();

        ViewData["Title"] = "Liste Kopyala";
        ViewData["OriginalListe"] = liste;

        var yeniListe = new AlisverisListesi
        {
            Ad = $"{liste.Ad} (Kopya)",
            Aciklama = liste.Aciklama,
            Renk = liste.Renk,
            VarsayilanMi = false
        };

        return View(yeniListe);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Kopyala(int id, [FromForm] AlisverisListesi liste)
    {
        var originalListe = await _listeService.GetListeById(id);
        if (originalListe == null) return NotFound();

        var yeniListeId = await _listeService.ListeKopyala(id, liste.Ad, liste.Aciklama, liste.Renk);
        
        if (yeniListeId > 0)
        {
            _logger.LogInformation("Liste kopyalandı: {OriginalListeAd} -> {YeniListeAd}", originalListe.Ad, liste.Ad);
            TempData["Success"] = "Liste başarıyla kopyalandı";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "Liste kopyalanırken hata oluştu");
        return View(liste);
    }
}
