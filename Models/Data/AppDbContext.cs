using Microsoft.EntityFrameworkCore;
using PENTDRIVEApi.Models;

namespace PENTDRIVEApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>().ToTable("produto");
        }

        public DbSet<Produto> Produtos { get; set; }


    }

}