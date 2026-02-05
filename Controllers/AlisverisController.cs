using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using web_uyg.Data;
using web_uyg.Models;
using web_uyg.Services;

namespace web_uyg.Controllers;

public class AlisverisController : Controller
{
    private readonly IAlisverisService _alisverisService;
    private readonly IListeService _listeService;
    private readonly ILogger<AlisverisController> _logger;
    private readonly AlisverisListesiContext _context;

    public AlisverisController(
        IAlisverisService alisverisService,
        IListeService listeService,
        ILogger<AlisverisController> logger,
        AlisverisListesiContext context)
    {
        _alisverisService = alisverisService;
        _listeService = listeService;
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index(int? listeId, string aramaMetni, string filtre, int? kategoriId, string siralama = "tarih")
    {
        try
        {
            var aktifListeId = listeId ?? (await _listeService.GetVarsayilanListe())?.Id ?? 0;

            if (aktifListeId == 0)
            {
                _logger.LogWarning("Hiç liste bulunamadı");
                return View("ListeYok");
            }

            var urunler = await _alisverisService.GetUrunlerByListeId(aktifListeId);

            if (!string.IsNullOrEmpty(aramaMetni))
            {
                urunler = urunler.Where(u => u.UrunAdi.Contains(aramaMetni, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (kategoriId.HasValue)
            {
                urunler = urunler.Where(u => u.KategoriId == kategoriId.Value).ToList();
            }

            filtre = filtre?.ToLower();
            urunler = filtre switch
            {
                "alinanlar" => urunler.Where(u => u.AlındiMi).ToList(),
                "alinmayanlar" => urunler.Where(u => !u.AlındiMi).ToList(),
                _ => urunler
            };

            urunler = siralama.ToLower() switch
            {
                "ada" => urunler.OrderBy(u => u.UrunAdi).ToList(),
                "adaz" => urunler.OrderByDescending(u => u.UrunAdi).ToList(),
                "tarih" => urunler.OrderByDescending(u => u.EklenmeTarihi).ToList(),
                "tariheski" => urunler.OrderBy(u => u.EklenmeTarihi).ToList(),
                "fiyat" => urunler.OrderBy(u => u.Fiyat ?? 0).ToList(),
                "fiyatyuksek" => urunler.OrderByDescending(u => u.Fiyat ?? 0).ToList(),
                _ => urunler
            };

            var tumListeler = await _listeService.TumListeler();
            var aktifListe = tumListeler.FirstOrDefault(l => l.Id == aktifListeId);
            var istatistik = await _listeService.GetListeIstatistik(aktifListeId);
            var kategoriler = await _context.Kategoriler.OrderBy(k => k.Ad).ToListAsync();

            ViewBag.TumListeler = tumListeler;
            ViewBag.AktifListe = aktifListe;
            ViewBag.AktifListeId = aktifListeId;
            ViewBag.Istatistik = istatistik;
            ViewBag.Kategoriler = kategoriler;
            ViewBag.AramaMetni = aramaMetni;
            ViewBag.Filtre = filtre;
            ViewBag.KategoriId = kategoriId;
            ViewBag.Siralama = siralama;

            return View(urunler);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ürünler listelenirken hata oluştu");
            return StatusCode(500, "Bir hata oluştu");
        }
    }

    public IActionResult Ekle(int listeId)
    {
        _logger.LogInformation("Ürün Ekle sayfası açılıyor. ListeId: {ListeId}", listeId);
        
        if (listeId == 0)
        {
            _logger.LogWarning("ListeId 0 geldi, varsayılan liste aranıyor...");
            var varsayilanListe = _listeService.GetVarsayilanListe().Result;
            if (varsayilanListe != null)
            {
                listeId = varsayilanListe.Id;
                _logger.LogInformation("Varsayılan liste bulundu: {ListeId}", listeId);
            }
        }
        
        ViewBag.ListeId = listeId;
        ViewBag.Kategoriler = _context.Kategoriler.OrderBy(k => k.Ad).Select(k => new SelectListItem
        {
            Value = k.Id.ToString(),
            Text = k.Ad
        }).ToList();
        
        var model = new AlisverisUrunu { ListeId = listeId };
        _logger.LogInformation("Model oluşturuldu. ListeId: {ListeId}", model.ListeId);
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ekle(AlisverisUrunu urun)
    {
        _logger.LogInformation("POST Ürün Ekle. ListeId: {ListeId}, UrunAdi: {UrunAdi}", urun.ListeId, urun.UrunAdi);
        
        // Formdan gelen veriyi logla
        foreach (var key in Request.Form.Keys)
        {
            _logger.LogInformation("Form {Key}: {Value}", key, Request.Form[key]);
        }
        
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState geçersiz. Hatalar: {Hatalar}", 
                string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            
            ViewBag.ListeId = urun.ListeId;
            ViewBag.Kategoriler = _context.Kategoriler.OrderBy(k => k.Ad).Select(k => new SelectListItem
            {
                Value = k.Id.ToString(),
                Text = k.Ad
            }).ToList();
            return View(urun);
        }

        var sonuc = await _alisverisService.UrunEkle(urun);

        if (sonuc.BasariliMi)
        {
            _logger.LogInformation("Ürün eklendi: {UrunAdi}", urun.UrunAdi);
            return RedirectToAction(nameof(Index), new { listeId = urun.ListeId });
        }

        ModelState.AddModelError("", sonuc.HataMesaji ?? "Ürün eklenirken hata oluştu");
        ViewBag.ListeId = urun.ListeId;
        ViewBag.Kategoriler = _context.Kategoriler.OrderBy(k => k.Ad).Select(k => new SelectListItem
        {
            Value = k.Id.ToString(),
            Text = k.Ad
        }).ToList();
        return View(urun);
    }

    public async Task<IActionResult> Duzenle(int id)
    {
        var urun = await _alisverisService.GetUrunById(id);
        if (urun == null) return NotFound();

        return View(urun);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(int id, [FromForm] AlisverisUrunu urun)
    {
        if (id != urun.Id) return NotFound();

        if (!ModelState.IsValid) return View(urun);

        var basarili = await _alisverisService.UrunGuncelle(urun);
        if (basarili)
        {
            _logger.LogInformation("Ürün güncellendi: {UrunAdi}", urun.UrunAdi);
            return RedirectToAction(nameof(Index), new { listeId = urun.ListeId });
        }

        ModelState.AddModelError("", "Güncelleme başarısız");
        return View(urun);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(int id, int listeId)
    {
        var basarili = await _alisverisService.UrunSil(id);
        if (basarili)
        {
            _logger.LogInformation("Ürün silindi: ID {UrunId}", id);
        }

        return RedirectToAction(nameof(Index), new { listeId });
    }

    [HttpPost]
    public async Task<IActionResult> DurumGuncelle(int id, int listeId)
    {
        var basarili = await _alisverisService.DurumGuncelle(id);
        if (!basarili)
        {
            return Json(new { success = false, message = "Durum güncellenemedi" });
        }

        var istatistik = await _listeService.GetListeIstatistik(listeId);
        return Json(new { success = true, istatistik });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ListeyiTemizle(int listeId, bool sadeceAlinanlar = false)
    {
        var sonuc = await _alisverisService.ListeyiTemizle(listeId, sadeceAlinanlar);

        if (sonuc.BasariliMi)
        {
            _logger.LogInformation("Liste temizlendi: {ListeId}, Silinen: {Sayi}", listeId, sonuc.SilinenUrunSayisi);
        }

        return RedirectToAction(nameof(Index), new { listeId });
    }

    [HttpPost]
    public async Task<IActionResult> UrunSiraGuncelle(int id, int yeniSira)
    {
        var basarili = await _alisverisService.UrunSiraGuncelle(id, yeniSira);
        return Json(new { success = basarili });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TopluSil([FromBody] List<int> urunIdleri, int listeId)
    {
        if (urunIdleri == null || !urunIdleri.Any())
        {
            return Json(new { success = false, message = "Seçili ürün yok" });
        }

        var basarili = await _alisverisService.TopluUrunSil(urunIdleri);
        if (basarili)
        {
            _logger.LogInformation("Toplu ürün silindi: {Sayi} ürün", urunIdleri.Count);
            return Json(new { success = true, message = $"{urunIdleri.Count} ürün başarıyla silindi" });
        }

        return Json(new { success = false, message = "Ürünler silinirken hata oluştu" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TopluAlinanYap([FromBody] List<int> urunIdleri, bool alinanMi)
    {
        if (urunIdleri == null || !urunIdleri.Any())
        {
            return Json(new { success = false, message = "Seçili ürün yok" });
        }

        var basarili = await _alisverisService.TopluDurumGuncelle(urunIdleri, alinanMi);
        if (basarili)
        {
            var durum = alinanMi ? "alındı" : "alınmadı";
            return Json(new { success = true, message = $"{urunIdleri.Count} ürün {durum} olarak işaretlendi" });
        }

        return Json(new { success = false, message = "İşlem başarısız" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TopluTasi([FromBody] List<int> urunIdleri, int hedefListeId)
    {
        if (urunIdleri == null || !urunIdleri.Any())
        {
            return Json(new { success = false, message = "Seçili ürün yok" });
        }

        var basarili = await _alisverisService.TopluListeTasi(urunIdleri, hedefListeId);
        if (basarili)
        {
            return Json(new { success = true, message = $"{urunIdleri.Count} ürün taşındı" });
        }

        return Json(new { success = false, message = "Ürünler taşınırken hata oluştu" });
    }

    public async Task<IActionResult> Yazdir(int listeId)
    {
        int aktifListeId = listeId > 0 ? listeId : (await _listeService.GetVarsayilanListe())?.Id ?? 0;
        var urunler = await _alisverisService.GetUrunlerByListeId(aktifListeId);
        var liste = await _listeService.GetListeById(aktifListeId);
        
        ViewData["Title"] = "Yazdır - " + liste?.Ad;
        ViewData["Urunler"] = urunler;
        ViewData["Liste"] = liste;
        
        return View();
    }

    public async Task<IActionResult> CSVExport(int listeId)
    {
        int aktifListeId = listeId > 0 ? listeId : (await _listeService.GetVarsayilanListe())?.Id ?? 0;
        var urunler = await _alisverisService.GetUrunlerByListeId(aktifListeId);
        var liste = await _listeService.GetListeById(aktifListeId);

        var csv = new System.Text.StringBuilder();
        csv.AppendLine("Ürün Adı,Miktar,Birim,Kategori,Fiyat,Alındı mı?,Not");
        
        foreach (var urun in urunler)
        {
            csv.AppendLine($"\"{urun.UrunAdi}\",{urun.Miktar},{urun.MiktarBirimi},\"{urun.Kategori?.Ad ?? ""}\",{urun.Fiyat?.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) ?? ""},{(urun.AlındiMi ? "Evet" : "Hayır")},\"{urun.Not ?? ""}\"");
        }

        return File(System.Text.Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"{liste?.Ad ?? "liste"}.csv");
    }
}
