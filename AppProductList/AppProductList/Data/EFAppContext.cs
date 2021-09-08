using AppProductList.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppProductList.Data
{
    public class EFAppContext : DbContext
    {
        public EFAppContext(DbContextOptions<EFAppContext> options) :
            base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }


    }
}
