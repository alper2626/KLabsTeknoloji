using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using KLabs.Business.Abstract;
using KLabs.Business.Concrete;
using KLabs.Business.Constants.Statics;
using KLabs.Core.ServiceInjector.DependencyResolvers;
using KLabs.Core.ServiceInjector.Extension;
using KLabs.Core.ServiceInjector.Utilities.IoC;
using KLabs.WebUI.Helpers.ImageHelper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace KLabs.WebUI
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
            services.AddControllersWithViews().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
            {
                config.Cookie.Name = "KLabs.Authentication";
                config.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                config.AccessDeniedPath = new PathString("/Panel/Account/Login");
                config.LoginPath = new PathString("/Panel/Account/Login");
            });

            services.AddDependencyResolvers(new ICoreModule[]
            {
                new CoreModule(),
            });
            ImageConfig.CreateBaseDirectories();
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
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    OnPrepareResponse = ctx =>
            //    {
            //        const int durationInSeconds = 60 * 60 * 24 * 30;
            //        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
            //            "public,max-age=" + durationInSeconds;
            //    }
            //});

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            });

        }

    }
}
