using ApiCrudEF.Estudantes;
using Microsoft.EntityFrameworkCore;

namespace ApiCrudEF.Data
{
    // dbcontext precisa do pacote EF core
    public class AppDBContext : DbContext
    {
        public DbSet<Estudante> estudantes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Banco.sqlite");
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }
    }
}