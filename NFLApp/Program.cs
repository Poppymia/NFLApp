using Microsoft.EntityFrameworkCore;
using NFLApp.Models;

namespace NFLApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // must be called before AddControllersWithViews()
            builder.Services.AddMemoryCache();
            builder.Services.AddSession();

            builder.Services.AddSession(options =>
            {
                // change idle timeout to 5 minutes - default is 20 minutes
                options.IdleTimeout = TimeSpan.FromSeconds(60 * 5);
                options.Cookie.HttpOnly = false;     // default is true
                options.Cookie.IsEssential = true;   // default is false
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<TeamContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString(
                    "NFLTeams")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // must be called before routes are mapped
            app.UseSession();

            //these are the custom URLs shown
            app.MapControllerRoute(
                name: "custom",
                pattern:
                    "{controller}/{action}/conf-{activeConf}/div-{activeDiv}"); 

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
