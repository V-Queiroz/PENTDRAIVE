using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PENTDRIVEApi.Models;

namespace PENTDRIVEApi.Models
{
    [Table("venda")]
    public class Venda
    {
        [Key]
        [Column("ID_VENDA")]
        public int Id { get; set; }

        [Column("CNPJ_CPF")]
        public string? CnpjCpf { get; set; }

        [Column("FORMA_PAG")]
        [StringLength(10)]
        public string? FormaPagamento { get; set; }

        [Column("VALOR_PAGO")]
        public decimal ValorPago { get; set; }

        [Column("VALOR_TOTAL")]
        public decimal ValorTotal { get; set; }

        [Column("STATUS")]
        public string? Status { get; set; }

        [Column("DATA_HORA")]
        public DateTime DataHora { get; set; }

        [Column("ID_USUARIO")]
        public int? IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }

        public ICollection<ItemVenda>? ItensVenda { get; set; }
        public ICollection<MovimentacaoEstoque>? Movimentacoes { get; set; }
    }


}   