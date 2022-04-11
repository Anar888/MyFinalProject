using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MilanoFashion.Dal;
using MilanoFashion.Models;
using MilanoFashion.Services;
using MilanoFashion.Utils;

namespace MilanoFashion
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews()
                 .AddNewtonsoftJson(options =>
     options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
 );

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Default"));

            });
            services.AddIdentity<AppUser, IdentityRole>(c =>
            {
                c.Password.RequireDigit = true;
                c.Password.RequiredLength = 8;
                c.Password.RequireUppercase = true;
                c.Password.RequireNonAlphanumeric = false;
                c.User.RequireUniqueEmail = true;
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<DataContext>();
            services.AddScoped<LayoutService>();
            Constants.EmailAddress = Configuration["Gmail:MailAddress"];
            Constants.Password = Configuration["Gmail:Password"];
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "areas",
                   pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
                   );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}
