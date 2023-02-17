using LastTab.Entities;
using LastTab.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace LastTab.Endpoints
{
    public class API
    {        
        public static async Task NewBookmark(HttpContext context)
        {
            
            if (!context.Request.Query.TryGetValue("url", out var url) || string.IsNullOrWhiteSpace(url))
            {
                await context.Response.WriteAsJsonAsync(new { ok = false, message = "Illegal Query String: url" });
                return;
            }
            var no_title = !context.Request.Query.TryGetValue("title", out var title) || string.IsNullOrWhiteSpace(title);
            if (no_title) { title = url; }

            if (!context.Request.Query.TryGetValue("user", out var user) || string.IsNullOrWhiteSpace(user))
            {
                await context.Response.WriteAsJsonAsync(new { ok = false, message = "Illegal Query String: User" });
                return;
            }
            if (!context.Request.Query.TryGetValue("appcode", out var appcode) || string.IsNullOrWhiteSpace(appcode))
            {
                await context.Response.WriteAsJsonAsync(new { ok = false, message = "Illegal Query String: AppCode" });
                return;
            }
            var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
            if (!AppCodeToken.IsMatch(user, appcode, configuration))
            {
                await context.Response.WriteAsJsonAsync(new { ok = false, message = "Invalid User Or AppCode" });
                return;
            }
            
            var service = context.RequestServices.GetRequiredService<BookmarkService>();
            var bookmark = await service.InsertBookmarkAsync(title, url, user);
            if(bookmark == null)
            {
                await context.Response.WriteAsJsonAsync(new { ok = false, message= "Not Available: BookmarkService.InsertBookmarkAsync" });
                return;
            }
            await context.Response.WriteAsJsonAsync(new { ok = true, message = "Ok", bookmark });
        }
    }
}
