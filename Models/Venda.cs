using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
        public string? FormaPagamento { get; set; }

        [Column("VALOR_PAGO")]
        public decimal ValorPago { get; set; }

        [Column("VALOR_TOTAL")]
        public decimal ValorTotal { get; set; }

        [Column("STATUS")]
        public string? Status { get; set; }

        [Column("DATA_HORA")]
        public DateTime DataHora { get; set; }

        public ICollection<ItemVenda>? ItensVenda { get; set; }
    }


}   