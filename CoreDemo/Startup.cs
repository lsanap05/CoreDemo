using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using BAL.Models;
using Core_WebApp.CustomFilters;
using CoreDemo.CustomeFilters;
using CoreDemo.CustoMiddlewares;
using CoreDemo.Data;
using DAL.DBContext;
using DAL.Repositories;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecAuthDBContext = CoreDemo.Data.SecAuthDBContext;

namespace CoreDemo
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
            services.AddDbContext<ShoppingDbContext>(option =>
            option.UseSqlServer(Configuration.GetConnectionString("eShoppingDbConnection"))
                );

                services.AddDbContext<SecAuthDBContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("SecAuthDBContextConnection")));
            //it's for Identityuser Sign In and Login
            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<SecAuthDBContext>();

            //it's for Identityuser Sign In, Login, and Role Managment
            services.AddIdentity<IdentityUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<SecAuthDBContext>();

            services.AddAuthentication();

            services.AddAuthorization(options => {
                options.AddPolicy("readonlypolicy",
                    builder => builder.RequireRole("Administration", "Seller", "Buyer"));
                options.AddPolicy("writepolicy",
                    builder => builder.RequireRole("Administration", "Seller"));
            });

            /// services.AddAntiforgery(o => o.HeaderName = CrossSiteAntiForgery.Header);
            services.AddAntiforgery(options =>
            {
                // Set Cookie properties using CookieBuilder properties†.
                options.FormFieldName = "AntiforgeryFieldname";
                options.HeaderName = "X-CSRF-TOKEN-HEADERNAME";
                options.SuppressXFrameOptionsHeader = false;
            });

            // regiter Category and Product Repositories
            services.AddScoped<IRepository<Category, int>, CategoryRepository>();
            services.AddScoped<IRepository<Product, int>, ProductRepository>();

            //Define session 
            // session will be stored in cache memmory
            services.AddDistributedMemoryCache();
            services.AddSession(session =>
            {
                session.IdleTimeout = TimeSpan.FromMinutes(20);
                session.Cookie.HttpOnly = true;
                session.Cookie.IsEssential = true;
            });

            services.AddControllersWithViews(options =>
            {
                //options.Filters.Add(new LogFilter());
                //options.Filters.Add(typeof(AppExceptionFilter));
                // options.Filters.Add(typeof(CrossSiteAntiForgery));
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

            }
            ).AddJsonOptions(option => { option.JsonSerializerOptions.PropertyNamingPolicy = null; }); 
            services.AddRazorPages();

            //services.AddMvc().AddRazorPagesOptions(options =>
            //{
            //    options.Conventions.Clear();
            //    options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "");
            //}).SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAntiforgery antiforgery)
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
            app.Use(next => context =>
            {
                string path = context.Request.Path.Value;

                if (
                    string.Equals(path, "/", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(path, "/index.html", StringComparison.OrdinalIgnoreCase))
                {
                    // The request token can be sent as a JavaScript-readable cookie, 
                    // and Angular uses it by default.
                    var tokens = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
                        new CookieOptions() { HttpOnly = false });
                }

                return next(context);
            });
            // add session
            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // apply the custom middleware
            app.UseCustomeException();
         
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages(); //this is use for map account razor web pages
            });
           
        }
    }
}
