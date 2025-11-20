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
using Microsoft.VisualBasic;
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
                return NotFound ($"Produto com Código de Barras {itemDTO.CodigoBarras} não encontrado.");
            }

            if(produto.Estoque < itemDTO.Quantidade)
            {
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

        venda.ValorTotal = valorTotal;
        _context.Vendas.Add(venda);

        await _context.SaveChangesAsync();

        return Ok (new
        {
            IdVenda = venda.Id,
            valorTotal = venda.ValorTotal,
            message = "Transação Aprovada. Venda e Movimentação de Estoque registradas com sucesso.",
            status = "SUCESSO"
        });

    }

        private const int UsuarioResponsavelId = 1;

        [HttpPost]
        public async Task<IActionResult> PostVenda(VendaRequest request)
        {

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    decimal valorTotalVenda = 0;
                    var itensVenda = new List<ItemVenda>();


                    var produtosParaAtualizar = new List<Produto>();

                    foreach (var item in request.Itens)
                    {
                        // Carrega o produto, e o EF Core começa a rastreá-lo
                        var produto = await _context.Produtos
                            .FirstOrDefaultAsync(p => p.CodigoBarras == item.CodigoBarras);

                        if (produto == null)
                        {
                            return NotFound($"Produto com Código de Barras {item.CodigoBarras} não encontrado");
                        }

                        if (produto.Estoque < item.Quantidade)
                        {
                            return BadRequest($"Estoque insuficiente para o produto '{produto.Nome}'. Disponivel: {produto.Estoque}");
                        }

                        decimal subtotal = produto.Preco * item.Quantidade;
                        valorTotalVenda += subtotal;

                        itensVenda.Add(new ItemVenda
                        {
                            ProdutoId = produto.Id,
                            Quantidade = item.Quantidade,
                            PrecoUnitario = produto.Preco,
                            Subtotal = subtotal
                        });


                        produto.Estoque -= item.Quantidade;


                        _context.Entry(produto!).State = EntityState.Detached;


                        produtosParaAtualizar.Add(produto);
                    }

                    var novaVenda = new Venda
                    {
                        CnpjCpf = request.CnpjCpf,
                        FormaPagamento = request.FormaPagamento,
                        ValorPago = request.ValorPago,
                        ValorTotal = valorTotalVenda,
                        Status = "PAGO",
                        DataHora = DateTime.Now
                    };

                    _context.Vendas.Add(novaVenda);


                    await _context.SaveChangesAsync();


                    foreach (var produto in produtosParaAtualizar)
                    {

                        _context.Produtos.Update(produto);
                    }


                    foreach (var itemVenda in itensVenda)
                    {
                        itemVenda.VendaId = novaVenda.Id;
                        _context.ItensVenda.Add(itemVenda);
                        _context.MovimentacoesEstoque.Add(new MovimentacaoEstoque
                        {
                            IdUsuario = UsuarioResponsavelId,
                            IdProduto = itemVenda.ProdutoId,
                            Quantidade = -itemVenda.Quantidade,
                            DataHora = DateTime.Now,
                            TipoMovimentacao = "SAIDA",
                            IdVenda = novaVenda.Id
                        });
                    }


                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();


                    return CreatedAtAction(nameof(PostVenda), new { id = novaVenda.Id }, novaVenda);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Erro ao processar a venda. A transação foi desfeita.");
                }
            }
        }

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

