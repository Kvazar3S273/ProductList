using AppProductList.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppProductList.ViewComponents
{
    public class ThingsViewComponent : ViewComponent
    {
        public EFAppContext _context { get; set; }
        public ThingsViewComponent(EFAppContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var list = await Task.Run(() => { return _context.Products.Include(x => x.ProductImages).ToList(); });
            return View("_Things", list);
        }
    }
}
