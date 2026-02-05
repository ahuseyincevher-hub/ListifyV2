using System.ComponentModel.DataAnnotations;

namespace web_uyg.Models;

public class AlisverisListesi
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Liste adı zorunludur")]
    [StringLength(100, ErrorMessage = "Liste adı en fazla 100 karakter olabilir")]
    public string Ad { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
    public string? Aciklama { get; set; }

    public string Renk { get; set; } = "#0d6efd";

    public bool VarsayilanMi { get; set; }

    public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;

    public DateTime? SonDegistirilmeTarihi { get; set; }

    public ICollection<AlisverisUrunu> Urunler { get; set; } = new List<AlisverisUrunu>();
}
