using Dapper;
using LastTab.Entities;
using LastTab.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastTab.Implementations
{
    public class Sqlite3 : IDB
    {
        private readonly ILogger<Sqlite3> logger;
        private readonly IConfiguration configuration;
        private static bool first = true;

        public Sqlite3(IServiceProvider services)
        {
            logger = services.GetService<ILogger<Sqlite3>>();
            configuration = services.GetRequiredService<IConfiguration>();
            if(first)
            {
                first = false;
                using var connection = CreateConnection();
                var sql = new StringBuilder();
                if (configuration.GetValue("db:reset", false))
                {
                    sql.Append(@"DROP TABLE IF EXISTS ""bookmarks"";");
                }
                sql.Append(@"
CREATE TABLE IF NOT EXISTS ""bookmarks"" (
    ""id"" text NOT NULL,
    ""title"" text NOT NULL,
    ""url"" text NOT NULL,
    ""timestamp"" integer NOT NULL,
    ""user"" text NOT NULL,
    PRIMARY KEY (""id"")
);");
                sql.Append(@"
CREATE INDEX IF NOT EXISTS ""bookmarks_index_id_user"" ON ""bookmarks"" (
    ""id"" COLLATE BINARY ASC,
    ""user"" COLLATE BINARY ASC
);");
                connection.Execute(sql.ToString());                
            }
        }
        public IDbConnection CreateConnection()
        {
            return new SqliteConnection($"Data Source={configuration.GetValue("db:sqlite3", "db.sqlite3")}");
        }

        public async Task<int> InsertBookmarkAsync(IDbConnection connection,Bookmark bookmark)
        {
            var sql = @"
INSERT INTO ""bookmarks"" (
    ""id"",""title"",""url"",""timestamp"",""user""
) VALUES (
    @Id,@Title,@Url,@Timestamp,@User
);";
            return await connection.ExecuteAsync(sql,bookmark);
        }

        public async Task<Bookmark[]> GetBookmarkListAsync(IDbConnection connection, string user)
        {
            var sql = @"
SELECT 
    ""id"" AS ""Id"",
    ""title"" AS ""Title"",
    ""url"" AS ""Url"",
    ""timestamp"" AS ""Timestamp"",
    ""user"" AS ""User""
FROM ""bookmarks""
WHERE ""user""=@user;";
            var bookmarks = await connection.QueryAsync<Bookmark>(sql, new { user });
            return bookmarks.ToArray();
        }

        public async Task<int> RemoveBookmarkByIdAsync(IDbConnection connection, string id, string user)
        {
            var sql = @"DELETE FROM ""bookmarks"" WHERE ""id""=@id AND ""user""=@user;";
            return await connection.ExecuteAsync(sql, new { id ,user} );
        }

    }
}
