using web_uyg.Models;

namespace web_uyg.Services;

public interface IListeService
{
    Task<List<AlisverisListesi>> TumListeler();
    Task<AlisverisListesi?> GetListeById(int id);
    Task<AlisverisListesi?> GetVarsayilanListe();
    Task<bool> ListeEkle(AlisverisListesi liste);
    Task<bool> ListeGuncelle(AlisverisListesi liste);
    Task<bool> ListeSil(int id);
    Task<bool> VarsayilanListeAyarla(int listeId);
    Task<ListeIstatistik> GetListeIstatistik(int listeId);
    Task<int> ListeKopyala(int listeId, string yeniAd, string? yeniAciklama, string? yeniRenk);
}

public class ListeIstatistik
{
    public int ToplamUrun { get; set; }
    public int AlinanUrun { get; set; }
    public int AlinmayanUrun { get; set; }
    public decimal? ToplamTutar { get; set; }
    public double TamamlanmaOrani { get; set; }
}
