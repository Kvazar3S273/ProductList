﻿using AppProductList.Data;
using AppProductList.Data.Entities;
using AppProductList.Models;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult ConfirmDelete(long id)
        {
            var selectedItem = _context.Products.FirstOrDefault(si => si.Id == id);
            if (selectedItem != null)
            {
                return View(selectedItem);
            }
            return NotFound();
        }

        [HttpPost]

        public IActionResult Delete(long id)
        {
            var deletedItem = _context.Products.FirstOrDefault(di => di.Id == id);
            if (deletedItem != null)
            {
                //var fileName = deletedItem.Image;
                //string dir = "\\images\\";
                //string path = _host.ContentRootPath + dir + fileName;
                //if (System.IO.File.Exists(path))
                //{
                //    System.IO.File.Delete(path);
                //}
                _context.Products.Remove(deletedItem);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return NotFound();
            //return Ok();
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