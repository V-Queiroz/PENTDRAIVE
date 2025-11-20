using PENTDRIVEApi.DTOs;
using PENTDRIVEApi.Models;

public class PagamentoRequestDTO : VendaRequest
{
    public DadosCartaoDTO DadosCartao { get; set; } = new DadosCartaoDTO();
}