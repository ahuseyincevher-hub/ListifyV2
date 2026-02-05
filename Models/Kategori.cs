using System.ComponentModel.DataAnnotations;

namespace web_uyg.Models;

public class Kategori
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kategori adı zorunludur")]
    [StringLength(50, ErrorMessage = "Kategori adı en fazla 50 karakter olabilir")]
    public string Ad { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir")]
    public string? Aciklama { get; set; }

    public ICollection<AlisverisUrunu> Urunler { get; set; } = new List<AlisverisUrunu>();
}
