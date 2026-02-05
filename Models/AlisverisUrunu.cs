using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace web_uyg.Models
{
    public class AlisverisUrunu
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur")]
        [StringLength(100, ErrorMessage = "Ürün adı en fazla 100 karakter olabilir")]
        [Display(Name = "Ürün adı")]
        public string UrunAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Miktar zorunludur")]
        [Range(1, 1000, ErrorMessage = "Miktar 1-1000 arasında olmalıdır")]
        [Display(Name = "Miktar")]
        public int Miktar { get; set; }

        [Required(ErrorMessage = "Miktar birimi zorunludur")]
        [Display(Name = "Miktar birimi")]
        public string MiktarBirimi { get; set; } = "Adet";

        [Display(Name = "Alındı mı?")]
        public bool AlındiMi { get; set; }

        public int? KategoriId { get; set; }
        public Kategori? Kategori { get; set; }

        [Required]
        public int ListeId { get; set; }
        
        public AlisverisListesi? Liste { get; set; }

        public string? ResimUrl { get; set; }

        public decimal? Fiyat { get; set; }

        public string? Not { get; set; }

        public int SiraNo { get; set; }

        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
    }
}
