using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace StoreApp.Models
{
    public class Product
    {
        [Display(Name ="Ürun ID")]
        public int ProductId { get; set; }

        [Required(ErrorMessage ="Ürün adı boş geçilemez!")]
        [StringLength(30,ErrorMessage ="Ürün adı {1} ila {2} uzunluğunda olmalıdır!",MinimumLength =2)]
        [Display(Name = "Ürun Adı")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Ürün fiyatı boş geçilemez!")]
        [Range(1,100000,ErrorMessage ="Ürün fiyatı {1} ila {2} TL arasında olmalıdır!")]
        [Display(Name = "Fiyat")]
        public decimal? Price { get; set; }

        [Display(Name = "Resim")]
        public string Image { get; set; }= string.Empty;

        [Display(Name = "Aktif mi?")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Ürün kategorisi boş geçilemez!")]
        [Display(Name = "Kategori")]
        public int? CategoryId { get; set; }
    }
}
