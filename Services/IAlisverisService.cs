using web_uyg.Models;

namespace web_uyg.Services;

public interface IAlisverisService
{
    Task<List<AlisverisUrunu>> GetUrunlerByListeId(int listeId);
    Task<AlisverisUrunu?> GetUrunById(int id);
    Task<UrunEklemeSonuc> UrunEkle(AlisverisUrunu urun);
    Task<bool> UrunGuncelle(AlisverisUrunu urun);
    Task<bool> UrunSil(int id);
    Task<bool> DurumGuncelle(int id);
    Task<ListeyiTemizleSonuc> ListeyiTemizle(int listeId, bool sadeceAlinanlar);
    Task<bool> UrunSiraGuncelle(int id, int yeniSira);
    Task<bool> TopluUrunSil(List<int> urunIdleri);
    Task<bool> TopluDurumGuncelle(List<int> urunIdleri, bool alinanMi);
    Task<bool> TopluListeTasi(List<int> urunIdleri, int hedefListeId);
}

public class UrunEklemeSonuc
{
    public bool BasariliMi { get; set; }
    public string? HataMesaji { get; set; }
    public AlisverisUrunu? Urun { get; set; }
}

public class ListeyiTemizleSonuc
{
    public bool BasariliMi { get; set; }
    public int SilinenUrunSayisi { get; set; }
}
