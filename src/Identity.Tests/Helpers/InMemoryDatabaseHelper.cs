using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Identity.Tests.Helpers
{
    public static class InMemoryDatabaseHelper
    {
        public static DbContextOptions<T> CreateOptions<T>() where T : DbContext
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = ":memory:"
            };

            var connection = new SqliteConnection(connectionStringBuilder.ToString());
            connection.Open();

            var builder = new DbContextOptionsBuilder<T>();
            builder.UseSqlite(connection);

            return builder.Options;
        }
    }
}
