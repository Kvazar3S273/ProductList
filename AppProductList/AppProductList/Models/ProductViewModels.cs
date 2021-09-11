using AppProductList.Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppProductList.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<ProductImageItemVM> Images { get; set; }
    }
    public class ProductImageItemVM
    {
        public string Path { get; set; }
    }

    public class DeletedProductImg
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<ProductImage> productViewModels { get; set; }
    }

    public class EditedProductImg
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<ProductImage> productImages { get; set; }
        public List<IFormFile> Image { get; set; }
        public List<string> delImage { get; set; }
    }

    public class ProductAddViewModel
    {
        [Display(Name="Назва")]
        public string Name { get; set; }
        [Display(Name="Ціна")]
        public decimal Price { get; set; }
        [Display(Name="Зображення")]
        public List<IFormFile> Images { get; set; }
    }
}
