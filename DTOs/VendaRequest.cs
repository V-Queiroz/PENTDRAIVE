namespace PENTDRIVEApi.DTOs;

    public class ItemVendaDTO
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
}

public class VendaRequest
{
    public string? CnpjCpf { get; set; }
    public string? FormaPagamento { get; set; }
    public decimal ValorPago { get; set; }

    public List<ItemVendaDTO> Itens { get; set; } = new List<ItemVendaDTO>();
}






    