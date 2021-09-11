using AppProductList.Data;
using AppProductList.Data.Entities;
using AppProductList.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppProductList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public EFAppContext _context { get; set; }

        public HomeController(ILogger<HomeController> logger, EFAppContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var model = _context.Products
               .Include(i => i.ProductImages)
               .Select(x => new ProductViewModel
               {
                   Id = x.Id,
                   Name = x.Name,
                   Price = x.Price,
                   Images = x.ProductImages.Select(t => new ProductImageItemVM
                   {
                       Path = "/images/" + t.Name
                   }).ToList()
               });
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductAddViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            Product newProduct = new Product();
            newProduct.Name = model.Name;
            newProduct.Price = model.Price;
            _context.Products.Add(newProduct);
            _context.SaveChanges();

            List<ProductImage> images = new List<ProductImage>();
            foreach (var item in model.Images)
            {
                string ext = Path.GetExtension(item.FileName);
                string fileName = Path.GetRandomFileName() + ext;

                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "products", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    item.CopyTo(stream);
                }
                ProductImage productImage = new ProductImage
                {
                    Name = fileName,
                    Product = newProduct
                };
                images.Add(productImage);
            }
            _context.ProductImages.AddRange(images);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Delete")]
        public IActionResult ConfirmDelete(int id)
        {
            var selectedItem = _context.Products.FirstOrDefault(si => si.Id == id);
            var res = _context.ProductImages.Where(r => r.ProductId == id).ToList();

            DeletedProductImg del = new();
            del.Id = selectedItem.Id;
            del.Name = selectedItem.Name;
            del.Price = selectedItem.Price;
            del.productViewModels = res;

            if (selectedItem != null)
            {
                return View(del);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var deletedItemImage = _context.ProductImages.FirstOrDefault(dii => dii.ProductId == id);
            var deletedItemProduct = _context.Products.FirstOrDefault(dip => dip.Id == id);
            if (deletedItemImage != null && deletedItemProduct != null)
            {
                _context.ProductImages.Remove(deletedItemImage);
                _context.Products.Remove(deletedItemProduct);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {

            var resultItem = _context.Products.FirstOrDefault(x => x.Id == id);
            var resultImageItem = _context.ProductImages.Where(c => c.ProductId == id).ToList();

            EditedProductImg modeledit = new();
            modeledit.Id = resultItem.Id;
            modeledit.Name = resultItem.Name;
            modeledit.Price = resultItem.Price;
            modeledit.productImages = resultImageItem;

            if (resultItem != null)
            {
                return View(modeledit);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditedProductImg editModel)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Не вірно введені дані");
                return View(editModel);
            }

            if (ModelState.IsValid)
            {
                var itemProd = _context.Products.FirstOrDefault(x => x.Id == id);

                itemProd.Name = editModel.Name;
                itemProd.Price = editModel.Price;

                string fileName = string.Empty;
                List<ProductImage> images = new List<ProductImage>();

                if (editModel.Image!=null)
                {
                    foreach (var item in editModel.Image)
                    {
                        string ext = Path.GetExtension(item.FileName);
                        fileName = Path.GetRandomFileName() + ext;

                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "products", fileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await item.CopyToAsync(stream);
                        }

                        ProductImage productImage = new ProductImage
                        {
                            Name = fileName,
                            Product = itemProd
                        };
                        images.Add(productImage);
                    }
                }

                if (editModel.delImage != null)
                {
                    string pathToDel = Path.Combine(Directory.GetCurrentDirectory(), "products");
                    foreach (var item in editModel.delImage)
                    {
                        var itemImage = _context.ProductImages.FirstOrDefault(x => x.Name == item);
                        string fullPath = Path.Combine(pathToDel, itemImage.Name);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        _context.ProductImages.Remove(itemImage);
                    }
                }
                _context.ProductImages.AddRange(images);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
