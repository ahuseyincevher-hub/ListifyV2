using System.ComponentModel.DataAnnotations;

namespace web_uyg.Models;

public class FavoriUrun
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ürün adı zorunludur")]
    [StringLength(100, ErrorMessage = "Ürün adı en fazla 100 karakter olabilir")]
    public string UrunAdi { get; set; } = string.Empty;

    public int? VarsayilanMiktar { get; set; }

    public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
}
