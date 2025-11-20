using System.ComponentModel.DataAnnotations;

namespace PENTDRIVEApi.DTOs;

public class ItemVendaDTO
{
    [Required(ErrorMessage = "O Código de Barras é obrigatório.")]
    [StringLength(50, MinimumLength = 10, ErrorMessage = "O Código de Barras tem um formato inválido.")]
    public string? CodigoBarras { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage ="A quantidade deve ser de pelo menos 1.")]
    public int Quantidade { get; set; }
}

public class VendaRequest
{
    public string? CnpjCpf { get; set; }
    public string? FormaPagamento { get; set; }
    public decimal ValorPago { get; set; }

    public List<ItemVendaDTO> Itens { get; set; } = new List<ItemVendaDTO>();
}