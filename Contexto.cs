using Forum.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Forum
{
    public class ContextoDb : DbContext
    {
        public DbSet<Arquivo> Arquivos { get; set; }

        public DbSet<Postagem> Postagens { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder {DataSource = "Database.db"};
            optionsBuilder.UseSqlite(connectionStringBuilder.ToString());
        }
    }
}