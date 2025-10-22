using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PENTDRIVEApi.Models
{
    [Table("itens_venda")]
    public class ItemVenda
    {
        [Key]
        [Column("ID_ITEM")]
        public int Id { get; set; }

        [Column("QUANTIDADE")]
        public int Quantidade { get; set; }

        [Column("PRECO_UNI")]
        public decimal PrecoUnitario { get; set; }

        [Column("SUBTOTAL")]
        public decimal Subtotal { get; set; }

        [Column("ID_PRODUTO")]
        public int ProdutoId { get; set; }
        public Produto? Produto { get; set; }

        [Column("ID_VENDA")]
        public int VendaId { get; set; }
        public Venda? Venda { get; set; }

    }


}