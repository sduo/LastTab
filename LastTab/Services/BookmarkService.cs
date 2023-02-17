using LastTab.Entities;
using LastTab.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LastTab.Services
{
    public class BookmarkService
    {
        private readonly ILogger<BookmarkService> logger;
        private readonly IConfiguration configuration;
        private readonly IDB db;
        private AuthenticationStateProvider auth;

        public BookmarkService(IServiceProvider services)
        {
            logger =services.GetService<ILogger<BookmarkService>>();
            configuration = services.GetRequiredService<IConfiguration>();
            db = services.GetRequiredService<IDB>();
            auth = services.GetRequiredService<AuthenticationStateProvider>();
        }

        public async Task<string> GetUserAsync()
        {
            var state = await auth.GetAuthenticationStateAsync();
            if (!state.User.Identity.IsAuthenticated) { return null; }
            return state.User.Identity.Name;
        }

        public async Task<Bookmark[]> GetBookmarkListAsync()
        {
            return await GetBookmarkListAsync(await GetUserAsync());
        }

        public async Task<Bookmark[]> GetBookmarkListAsync(string user)
        {
            if (string.IsNullOrWhiteSpace(user)) { return Array.Empty<Bookmark>(); }
            using var connection = db.CreateConnection();
            return await db.GetBookmarkListAsync(connection, user);            
        }

        public async Task<Bookmark> InsertBookmarkAsync(string title, string url)
        {
            return await InsertBookmarkAsync(title, url, await GetUserAsync());
        }

        public async Task<Bookmark> InsertBookmarkAsync(string title,string url,string user)
        {
            if (string.IsNullOrWhiteSpace(title)) { return null; }
            if (string.IsNullOrWhiteSpace(url)) { return null; }
            if (string.IsNullOrWhiteSpace(user)) { return null; }
            var bookmark = new Bookmark()
            {
                Id = Guid.NewGuid().ToString(),
                Title = title,
                Url = url,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                User = user
            };
            using var connection = db.CreateConnection();
            var rows = await db.InsertBookmarkAsync(connection, bookmark);
            if (rows == 0) { return null; }
            return bookmark;
        }

        public async Task<int> RemoveBookmarkAsync(string id)
        {
            return await RemoveBookmarkAsync(id, await GetUserAsync());
        }

        public async Task<int> RemoveBookmarkAsync(string id,string user)
        {
            if (string.IsNullOrWhiteSpace(id)) { return 0; }
            if (string.IsNullOrWhiteSpace(user)) { return 0; }
            using var connection = db.CreateConnection();
            return await db.RemoveBookmarkByIdAsync(connection, id, user);
        }
    }
}
