using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PENTDRIVEApi.Data;
using PENTDRIVEApi.DTOs;
using PENTDRIVEApi.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;

namespace PENTDRIVEApi.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VendasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VendasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("ProcessarPagamento")]
        public async Task<IActionResult> ProcessarPagamento([FromBody] PagamentoRequestDTO request)
        {
            // Iniciando a Transação Atômica (garante a integridade Venda+Estoque)
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    //simulação do Gateway de pagamento
                    if(request.DadosCartao.Cvv == "000" || string.IsNullOrEmpty(request.DadosCartao.Cvv))
                    {
                        return BadRequest (new
                        {
                            message = "Transação negada pela administradora. Código de erro: CVV INVÁLIDO ou vazio.",
                            status = "FALHA"

                        });
                    }

                //preparação da venda, so executa se o pagamento for aprovado
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if(string.IsNullOrEmpty (userIdString)) return Unauthorized("Usuário não autenticado ou token inválido.");

                int userId = int.Parse(userIdString!);  
                decimal valorTotal = 0;

                var venda = new Venda
                {
                    CnpjCpf = request.CnpjCpf,
                    FormaPagamento = request.FormaPagamento,
                    DataHora = DateTime.Now,
                    Status = "PAGO",
                    IdUsuario = userId,
                    ItensVenda = new List<ItemVenda>(),
                    Movimentacoes = new List<MovimentacaoEstoque>()
                };

                // isso aq pra garantir que a lista de itens não seja alterada durante o loop
                foreach (var itemDTO in request.Itens.ToList())
                {
                    var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.CodigoBarras == itemDTO.CodigoBarras);
                    
                    // Validação de estoque e existência do produto
                    if(produto == null) 
                    {
                        // Rollback se produto não for encontrado
                        await transaction.RollbackAsync(); 
                        return NotFound ($"Produto com Código de Barras {itemDTO.CodigoBarras} não encontrado.");
                    }

                    if(produto.Estoque < itemDTO.Quantidade)
                    {
                        // Rollback se estoque for insuficiente
                        await transaction.RollbackAsync();
                        return BadRequest($"Estoque insulficiente para {produto.Nome}. Disponivel: {produto.Estoque}");
                    }

                    produto.Estoque -= itemDTO.Quantidade;

                    var itemVenda = new ItemVenda
                    {
                        ProdutoId = produto.Id,
                        Quantidade = itemDTO.Quantidade,
                        PrecoUnitario = produto.Preco,
                        Subtotal = produto.Preco * itemDTO.Quantidade,
                        Venda = venda
                    };
                    venda.ItensVenda.Add(itemVenda);

                    var movimentacao = new MovimentacaoEstoque
                    {
                        TipoMovimentacao = "SAIDA",
                        Quantidade = itemDTO.Quantidade,
                        DataHora = venda.DataHora,
                        IdProduto = produto.Id,
                        IdUsuario = userId,
                        Venda = venda
                    };

                    venda.Movimentacoes.Add(movimentacao);

                    valorTotal += itemVenda.Subtotal;
                } 

                // Salva todas as alterações (Venda, Itens, Movimentações e atualização do Estoque)
                venda.ValorTotal = valorTotal;
                _context.Vendas.Add(venda);

                await _context.SaveChangesAsync();

                // Confirma a transação
                await transaction.CommitAsync();

                return Ok (new
                {
                    IdVenda = venda.Id,
                    valorTotal = venda.ValorTotal,
                    message = "Transação Aprovada. Venda e Movimentação de Estoque registradas com sucesso.",
                    status = "SUCESSO"
                });
            }
            catch (Exception)
            {
                // Reverte a transação em caso de qualquer erro
                await transaction.RollbackAsync();
                return StatusCode(500, "Erro ao processar a venda. A transação foi desfeita.");
            }
        }
    }

        // O MÉTODO [HttpPost] PostVenda FOI REMOVIDO DAQUI para resolver a duplicação e ambiguidade.
        
        [HttpGet("{id}")]
        
    public async Task<IActionResult> GetVenda (int id)
            {
                var venda = await _context.Vendas
                .Include(v => v.ItensVenda!)
                        .ThenInclude(i => i.Produto)
                    .FirstOrDefaultAsync(v => v.Id == id);

            if (venda == null)
            {
                return NotFound($"Venda do item com Id {id} não encontrada.");
            } 
                return Ok(venda);
            }
        [HttpGet]
         public async Task<ActionResult<IEnumerable<Venda>>> GetTodasVendas()
        {
            return await _context.Vendas.ToListAsync();
        }
    }
}