using AppProductList.Data;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppProductList
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddControllersWithViews()
                .AddViewLocalization();// добавляем локализацию представлений;
            services.AddDbContext<EFAppContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("uk-UA"),
                    new CultureInfo("en-US"),
                    new CultureInfo("ru-RU")
                };

                options.DefaultRequestCulture = new RequestCulture("uk-UA");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseCulture();
            
            //var supportedCultures = new[]
            //{
            //    new CultureInfo("uk-UA"),
            //    new CultureInfo("en-US"),
            //    new CultureInfo("ru-RU")
            //};
            //app.UseRequestLocalization(new RequestLocalizationOptions
            //{
            //    DefaultRequestCulture = new RequestCulture("uk-UA"),
            //    SupportedCultures = supportedCultures,
            //    SupportedUICultures = supportedCultures
            //});
            
            app.UseRequestLocalization();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            var dirName = "products";
            var dirServer = Path.Combine(Directory.GetCurrentDirectory(), dirName);
            if (!Directory.Exists(dirServer))
            {
                Directory.CreateDirectory(dirServer);
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(dirServer),
                RequestPath = "/images"
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
