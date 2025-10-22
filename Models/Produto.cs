using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PENTDRIVEApi.Models
{
    public class Produto
    {
        [Key]
        [Column("ID_PRODUTO")]
        public int Id { get; set; }
        public string? Nome { get; set; }

        [Column("PRECO_UNI")]
        public decimal Preco { get; set; }

        [Column("QNTD_ESTOQUE")]
        public int Estoque { get; set; }

        [Column("COD_BARRAS")]
        public string? CodigoBarras { get; set; }

        public Produto() { }

        public Produto(int id, string nome, decimal preco, int estoque, string codigoBarras)
        {
            Id = id;
            Nome = nome;
            Preco = preco;
            Estoque = estoque;
            CodigoBarras = codigoBarras;

        }


    }


}


