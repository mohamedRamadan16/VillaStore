using Magic_Villa_MVC.Services;
using Magic_Villa_MVC.Services.IServices;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Magic_Villa_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddAutoMapper(typeof(MappingConfig));

            builder.Services.AddHttpClient<IVillaService, VillaService>();
            builder.Services.AddScoped<IVillaService, VillaService>();

            builder.Services.AddHttpClient<IVillaNumberService, VillaNumberService>();
            builder.Services.AddScoped<IVillaNumberService, VillaNumberService>();

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddHttpClient<IAuthService, AuthService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                          .AddCookie(options =>
                          {
                              options.Cookie.HttpOnly = true;
                              options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                              options.LoginPath = "/Auth/Login";
                              options.AccessDeniedPath = "/Auth/AccessDenied";
                              options.SlidingExpiration = true;
                          });
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(100);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            //app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
