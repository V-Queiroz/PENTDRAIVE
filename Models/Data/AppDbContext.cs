using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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
            modelBuilder.Entity<Venda>().ToTable("venda");
            modelBuilder.Entity<ItemVenda>().ToTable("itens_venda");

            // 1. Mapeamento da Relação ItemVenda -> Venda
            modelBuilder.Entity<ItemVenda>()
                .HasOne(iv => iv.Venda)            
                .WithMany(v => v.ItensVenda)       
                .HasForeignKey(iv => iv.VendaId)   
                .IsRequired();

            // 2. Mapeamento da Relação ItemVenda -> Produto
            modelBuilder.Entity<ItemVenda>()
                .HasOne(iv => iv.Produto)
                .WithMany()
                .HasForeignKey(iv => iv.ProdutoId)
                .IsRequired();
                
                modelBuilder.Entity<ItemVenda>()
                .Property(iv => iv.Subtotal)
                .ValueGeneratedOnAddOrUpdate()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }

        public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }


    }

}