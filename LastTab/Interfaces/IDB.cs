using LastTab.Entities;
using System.Data;
using System.Threading.Tasks;

namespace LastTab.Interfaces
{
    public interface IDB
    {
        public IDbConnection CreateConnection();
        public Task<int> InsertBookmarkAsync(IDbConnection connection, Bookmark bookmark);

        public Task<Bookmark[]> GetBookmarkListAsync(IDbConnection connection,string user);

        public Task<int> RemoveBookmarkByIdAsync(IDbConnection connection,string id, string user);
    }
}
