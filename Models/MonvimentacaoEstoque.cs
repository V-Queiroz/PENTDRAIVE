using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PENTDRIVEApi.Models
{
    // Mapeia para o nome da tabela no MySQL: mov_estoque
    [Table("mov_estoque")]
    public class MovimentacaoEstoque
    {
        [Key]
        [Column("ID_MOVIMENTO")] // Chave primária da movimentação
        public int Id { get; set; }

        [Column("TIPO")]
        public string TipoMovimentacao { get; set; } = string.Empty;

        [Column("QUANTIDADE")]
        public int Quantidade { get; set; }

        [Column("DATA_HORA")]
        public DateTime DataHora { get; set; }

        [Column("ID_PRODUTO")]
        public int IdProduto { get; set; }

        [Column("ID_USUARIO")]
        public int IdUsuario { get; set; }

        [Column("ID_VENDA")]
        public int? IdVenda { get; set; }





    }
}