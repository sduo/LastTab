using AntDesign;
using LastTab.Endpoints;
using LastTab.Entities;
using LastTab.Implementations;
using LastTab.Interfaces;
using LastTab.Providers;
using LastTab.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace LastTab
{
    public class Program
    {
        public static void Main(params string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddAntDesign();
            builder.Services.AddScoped<AuthenticationStateProvider>((serivces) => { return new AppCodeAuthenticationStateProvider(serivces); });
            builder.Services.AddScoped<IDB>((services) =>
            {
                return new Sqlite3(services);
            });
            builder.Services.AddScoped<BookmarkService>();

            var app = builder.Build();

            app.UseStaticFiles();

            app.UseRouting();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");
            app.MapGet("/api/new", API.NewBookmark);

            app.Run();
        }
    }
}

